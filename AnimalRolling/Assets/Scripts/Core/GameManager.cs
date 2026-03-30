using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central singleton that owns game state, config, and orchestrates transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Config")]
    public GameConfig config;
    public AnimalDefinition[] animals;
    public UpgradeDefinition[] upgrades;

    [Header("References")]
    public BoardBuilder boardBuilder;
    public PlayerController playerController;
    public CameraController cameraController;
    public FarmerSpawner farmerSpawner;
    public PropScatterer propScatterer;
    public TileEffectHandler tileEffectHandler;
    public EncounterManager encounterManager;
    public TurnController turnController;
    public UpgradeShop upgradeShop;
    public HUDController hud;
    public SanctuaryUI sanctuaryUI;
    public OverlayManager overlayManager;

    // State
    [HideInInspector] public PersistentData persist;
    [HideInInspector] public RunState run;

    public string selectedAnimalId = "brave_pig";

    // Events
    public UnityEvent onRunStarted;
    public UnityEvent onRunEnded;
    public UnityEvent onStateChanged;

    // Dice timer
    private float diceTimerElapsed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        persist = SaveManager.Load();
        run = new RunState();

        Application.targetFrameRate = 120;
    }

    void Start()
    {
        sanctuaryUI.Show();
    }

    void Update()
    {
        if (run.animalDef != null)
            UpdateDiceTimer();
    }

    void UpdateDiceTimer()
    {
        diceTimerElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(diceTimerElapsed / config.diceRefillTime);
        hud.UpdateDiceTimerBar(progress);

        if (diceTimerElapsed >= config.diceRefillTime)
        {
            diceTimerElapsed = 0f;
            if (run.dice < config.maxDice)
            {
                run.dice += config.diceRefillAmount;
                run.dice = Mathf.Min(run.dice, config.maxDice);
                hud.RefreshAll(run);
            }
            WriteSave();
        }
    }

    public AnimalDefinition GetAnimalById(string id)
    {
        foreach (var a in animals)
            if (a.id == id) return a;
        return animals.Length > 0 ? animals[0] : null;
    }

    public UpgradeDefinition GetUpgradeById(string id)
    {
        foreach (var u in upgrades)
            if (u.id == id) return u;
        return null;
    }

    public int GetAnimalMeals(string animalId)
    {
        return Mathf.FloorToInt(persist.GetAnimalMeals(animalId));
    }

    public void AwardMeals(float amount)
    {
        if (amount <= 0) return;
        run.runMeals += amount;
        run.fuel += Mathf.FloorToInt(amount);
        string id = run.animalDef != null ? run.animalDef.id : selectedAnimalId;
        persist.AddAnimalMeals(id, amount);
        WriteSave();
    }

    public int GetBribeCost()
    {
        return config.farmerBribeBase + run.bribeCount * config.farmerBribeEscalation;
    }

    public bool IsTileAlteredToMeal(int physIdx)
    {
        return run.alteredTiles.Contains(physIdx);
    }

    public void ApplyTileAlterations()
    {
        float chance = config.tileAlterChance * run.tileAlterCount;
        var eligibleTypes = new HashSet<TileType> { TileType.Road, TileType.Start, TileType.Bonus, TileType.Event };

        for (int i = 0; i < config.boardSize; i++)
        {
            var def = boardBuilder.GetTileDef(i);
            if (eligibleTypes.Contains(def.type) && !run.alteredTiles.Contains(i))
            {
                if (Random.value < chance)
                {
                    run.alteredTiles.Add(i);
                    boardBuilder.TintTile(i, new Color(0.365f, 0.733f, 0.388f));
                }
            }
        }
    }

    public void StartRun(AnimalDefinition animal)
    {
        run = new RunState
        {
            animalDef = animal,
            dice = persist.dice,
            multiplier = 1,
            flockMode = animal.id == "chicken_flock",
            frontChickens = animal.id == "chicken_flock" ? 3 : 0,
            rearChickens = animal.id == "chicken_flock" ? 2 : 0,
        };

        diceTimerElapsed = 0f;

        sanctuaryUI.Hide();

        // Build the world
        boardBuilder.BuildBoard();
        propScatterer.ScatterProps();
        farmerSpawner.Init();
        playerController.SpawnPlayer(animal);
        cameraController.Init();

        hud.gameObject.SetActive(true);
        hud.RefreshAll(run);
        hud.RefreshRibbon(run);
        hud.RenderTrackCities(config.trackCheckpoints);

        // Wire HUD buttons
        if (hud.rollButton != null)
        {
            hud.rollButton.onClick.RemoveAllListeners();
            hud.rollButton.onClick.AddListener(turnController.DoRoll);
        }
        if (hud.multiplierButton != null)
        {
            hud.multiplierButton.onClick.RemoveAllListeners();
            hud.multiplierButton.onClick.AddListener(turnController.CycleMultiplier);
        }
        if (hud.upgradeButton != null)
        {
            hud.upgradeButton.onClick.RemoveAllListeners();
            hud.upgradeButton.onClick.AddListener(upgradeShop.OpenShop);
        }

        onRunStarted?.Invoke();
    }

    public void ReturnToSanctuary()
    {
        persist.totalCash += run.cash;
        persist.dice = run.dice;
        WriteSave();

        // Cleanup world
        boardBuilder.ClearBoard();
        propScatterer.ClearProps();
        farmerSpawner.ClearAll();
        playerController.DespawnPlayer();

        hud.gameObject.SetActive(false);
        overlayManager.CloseAll();

        run = new RunState();
        sanctuaryUI.Show();

        onRunEnded?.Invoke();
    }

    public void WriteSave()
    {
        // Only sync dice from run state when a run is active,
        // otherwise we'd overwrite persist.dice with 0 from the default RunState
        if (run.animalDef != null)
            persist.dice = run.dice;
        SaveManager.Save(persist);
    }

    public void DebugReset()
    {
        SaveManager.DeleteSave();
        persist = new PersistentData();
        selectedAnimalId = "brave_pig";
        sanctuaryUI.Refresh();
    }

    public void DebugAddCoins()
    {
        persist.totalCash += 5000;
        WriteSave();
        sanctuaryUI.Refresh();
    }
}
