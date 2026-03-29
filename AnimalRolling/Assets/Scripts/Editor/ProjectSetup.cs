#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility to auto-generate all ScriptableObject assets and set up the scene.
/// Run from menu: AnimalEscape > Setup Project.
/// </summary>
public class ProjectSetup : EditorWindow
{
    [MenuItem("AnimalEscape/Setup Project")]
    static void SetupProject()
    {
        CreateDirectories();
        CreateGameConfig();
        CreateAnimalAssets();
        CreateUpgradeAssets();
        Debug.Log("AnimalEscape: Project setup complete! ScriptableObjects created in Assets/ScriptableObjects/");
        Debug.Log("AnimalEscape: Now create a scene and add the GameSetup prefab, or use AnimalEscape > Setup Scene.");
    }

    [MenuItem("AnimalEscape/Setup Scene")]
    static void SetupScene()
    {
        SetupSceneObjects();
        Debug.Log("AnimalEscape: Scene setup complete!");
    }

    static void CreateDirectories()
    {
        string[] dirs = {
            "Assets/ScriptableObjects",
            "Assets/ScriptableObjects/Animals",
            "Assets/ScriptableObjects/Upgrades",
            "Assets/Prefabs",
            "Assets/Prefabs/UI",
        };
        foreach (var dir in dirs)
        {
            if (!AssetDatabase.IsValidFolder(dir))
            {
                string parent = Path.GetDirectoryName(dir).Replace("\\", "/");
                string folder = Path.GetFileName(dir);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }

    static void CreateGameConfig()
    {
        string path = "Assets/ScriptableObjects/GameConfig.asset";
        if (AssetDatabase.LoadAssetAtPath<GameConfig>(path) != null) return;

        var config = ScriptableObject.CreateInstance<GameConfig>();
        AssetDatabase.CreateAsset(config, path);
    }

    static void CreateAnimalAssets()
    {
        var animalData = new[]
        {
            new { id = "brave_pig",     name = "Brave Piggy",   emoji = "P",  color = HexColor("FFB6C1"), speed = 0, armor = 1, stealth = 0, price = 0,    ability = "Mud Slide",    desc = "Loves the mud. Sturdy and reliable." },
            new { id = "quick_chick",   name = "Swift Chick",   emoji = "C",  color = HexColor("FFF380"), speed = 2, armor = 0, stealth = 0, price = 1500, ability = "Wing Flutter",  desc = "Fast but fragile." },
            new { id = "gentle_cow",    name = "Gentle Cow",    emoji = "W",  color = HexColor("F0F0F0"), speed = 0, armor = 2, stealth = 0, price = 3000, ability = "Stampede",      desc = "Very tough to stop." },
            new { id = "woolly_sheep",  name = "Woolly Sheep",  emoji = "S",  color = HexColor("EFEFEF"), speed = 0, armor = 1, stealth = 1, price = 2000, ability = "Fleece Veil",   desc = "Blends in anywhere. Quiet and steady." },
            new { id = "swift_rabbit",  name = "Swift Rabbit",  emoji = "R",  color = HexColor("D4C5B2"), speed = 3, armor = 0, stealth = 1, price = 4000, ability = "Burrow",        desc = "Lightning fast. Gone in a flash." },
            new { id = "lucky_duck",    name = "Lucky Duck",    emoji = "D",  color = HexColor("6B9E5E"), speed = 1, armor = 0, stealth = 2, price = 5500, ability = "Wing Splash",   desc = "Master of disguise. Slips away unseen." },
            new { id = "chicken_flock", name = "Chicken Flock",  emoji = "F", color = HexColor("FFF380"), speed = 1, armor = 0, stealth = 0, price = 8000, ability = "Scatter",       desc = "5 chickens. Two tiles, double trouble!" },
        };

        foreach (var d in animalData)
        {
            string path = "Assets/ScriptableObjects/Animals/" + d.id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<AnimalDefinition>(path) != null) continue;

            var asset = ScriptableObject.CreateInstance<AnimalDefinition>();
            asset.id = d.id;
            asset.animalName = d.name;
            asset.emoji = d.emoji;
            asset.color = d.color;
            asset.speed = d.speed;
            asset.armor = d.armor;
            asset.stealth = d.stealth;
            asset.price = d.price;
            asset.abilityName = d.ability;
            asset.description = d.desc;
            AssetDatabase.CreateAsset(asset, path);
        }
    }

    static void CreateUpgradeAssets()
    {
        var upgradeData = new[]
        {
            new { id = "dice30",    name = "30 Rolls",     icon = "D", desc = "Ancient luck stones",                          stat = UpgradeStat.Dice,    cost = 1000, tier = UpgradeTier.Mid,       meal = MealEffect.Instant,   statAmt = 0, diceAmt = 30 },
            new { id = "berries",   name = "Wild Berries",  icon = "B", desc = "+1 Speed, passive meals every tile",           stat = UpgradeStat.Speed,   cost = 500,  tier = UpgradeTier.Cheap,     meal = MealEffect.Passive,   statAmt = 1, diceAmt = 0 },
            new { id = "clover",    name = "Lucky Clover",  icon = "L", desc = "+1 Stealth, passive meals every tile",         stat = UpgradeStat.Stealth, cost = 600,  tier = UpgradeTier.Cheap,     meal = MealEffect.Passive,   statAmt = 1, diceAmt = 0 },
            new { id = "bark",      name = "Oak Bark",      icon = "T", desc = "+1 Resilience, instant meals & small passive", stat = UpgradeStat.Armor,   cost = 700,  tier = UpgradeTier.Cheap,     meal = MealEffect.Instant,   statAmt = 1, diceAmt = 0 },
            new { id = "apple",     name = "Sweet Apple",   icon = "A", desc = "Instant meals for your animal",                stat = UpgradeStat.Fuel,    cost = 400,  tier = UpgradeTier.Cheap,     meal = MealEffect.Instant,   statAmt = 0, diceAmt = 0 },
            new { id = "mushrooms", name = "Magic Fungi",   icon = "M", desc = "+2 Speed, boosts meal tiles",                  stat = UpgradeStat.Speed,   cost = 1500, tier = UpgradeTier.Mid,       meal = MealEffect.Passive,   statAmt = 2, diceAmt = 0 },
            new { id = "feathers",  name = "Hawk Feather",  icon = "H", desc = "+2 Stealth, boosts meal tiles",                stat = UpgradeStat.Stealth, cost = 1200, tier = UpgradeTier.Mid,       meal = MealEffect.Passive,   statAmt = 2, diceAmt = 0 },
            new { id = "stones",    name = "River Stones",  icon = "S", desc = "+2 Resilience, boosts meal tiles",             stat = UpgradeStat.Armor,   cost = 1400, tier = UpgradeTier.Mid,       meal = MealEffect.Passive,   statAmt = 2, diceAmt = 0 },
            new { id = "vines",     name = "Wild Vines",    icon = "V", desc = "Road tiles now sometimes yield meals",         stat = UpgradeStat.None,    cost = 1800, tier = UpgradeTier.Expensive, meal = MealEffect.TileAlter, statAmt = 0, diceAmt = 0 },
            new { id = "flowers",   name = "Blossom Path",  icon = "F", desc = "Even more tiles become meal sources",          stat = UpgradeStat.None,    cost = 2500, tier = UpgradeTier.Expensive, meal = MealEffect.TileAlter, statAmt = 0, diceAmt = 0 },
        };

        foreach (var d in upgradeData)
        {
            string path = "Assets/ScriptableObjects/Upgrades/" + d.id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(path) != null) continue;

            var asset = ScriptableObject.CreateInstance<UpgradeDefinition>();
            asset.id = d.id;
            asset.upgradeName = d.name;
            asset.icon = d.icon;
            asset.description = d.desc;
            asset.stat = d.stat;
            asset.cost = d.cost;
            asset.tier = d.tier;
            asset.mealEffect = d.meal;
            asset.statAmount = d.statAmt;
            asset.diceAmount = d.diceAmt;
            AssetDatabase.CreateAsset(asset, path);
        }
    }

    static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }

    static void SetupSceneObjects()
    {
        // Find or load config
        var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/ScriptableObjects/GameConfig.asset");

        // Find a URP Lit material to use as template
        // User should assign a proper URP Lit material. We create a placeholder.
        Material urpLit = GetOrCreateDefaultMaterial();

        // --- Create GameManager root ---
        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        gm.config = config;

        // Load animals
        var animalGuids = AssetDatabase.FindAssets("t:AnimalDefinition", new[] { "Assets/ScriptableObjects/Animals" });
        gm.animals = new AnimalDefinition[animalGuids.Length];
        for (int i = 0; i < animalGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(animalGuids[i]);
            gm.animals[i] = AssetDatabase.LoadAssetAtPath<AnimalDefinition>(path);
        }
        // Sort by price
        System.Array.Sort(gm.animals, (a, b) => a.price.CompareTo(b.price));

        // Load upgrades
        var upgradeGuids = AssetDatabase.FindAssets("t:UpgradeDefinition", new[] { "Assets/ScriptableObjects/Upgrades" });
        gm.upgrades = new UpgradeDefinition[upgradeGuids.Length];
        for (int i = 0; i < upgradeGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(upgradeGuids[i]);
            gm.upgrades[i] = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(path);
        }
        System.Array.Sort(gm.upgrades, (a, b) => a.cost.CompareTo(b.cost));

        // --- Board ---
        var boardGO = new GameObject("BoardBuilder");
        var bb = boardGO.AddComponent<BoardBuilder>();
        bb.config = config;
        bb.tileMaterialTemplate = urpLit;
        gm.boardBuilder = bb;

        // --- Props ---
        var propsGO = new GameObject("PropScatterer");
        var ps = propsGO.AddComponent<PropScatterer>();
        ps.config = config;
        ps.propMaterialTemplate = urpLit;
        gm.propScatterer = ps;

        // --- Farmer ---
        var farmerGO = new GameObject("FarmerSpawner");
        var fs = farmerGO.AddComponent<FarmerSpawner>();
        fs.config = config;
        fs.boardBuilder = bb;
        fs.farmerMaterialTemplate = urpLit;
        gm.farmerSpawner = fs;

        // --- Player ---
        var playerGO = new GameObject("PlayerController");
        var pc = playerGO.AddComponent<PlayerController>();
        pc.config = config;
        pc.boardBuilder = bb;
        pc.playerMaterialTemplate = urpLit;
        gm.playerController = pc;

        // --- Camera ---
        var camGO = new GameObject("CameraController");
        var cc = camGO.AddComponent<CameraController>();
        cc.boardBuilder = bb;
        cc.playerController = pc;
        gm.cameraController = cc;

        // --- Gameplay ---
        var tileEffectGO = new GameObject("TileEffectHandler");
        gm.tileEffectHandler = tileEffectGO.AddComponent<TileEffectHandler>();

        var encounterGO = new GameObject("EncounterManager");
        gm.encounterManager = encounterGO.AddComponent<EncounterManager>();

        var turnGO = new GameObject("TurnController");
        var tc = turnGO.AddComponent<TurnController>();
        gm.turnController = tc;

        var upgradeGO = new GameObject("UpgradeShop");
        var us = upgradeGO.AddComponent<UpgradeShop>();
        gm.upgradeShop = us;

        // --- UI Canvas ---
        CreateUICanvas(gm, urpLit);

        // --- Lighting ---
        SetupLighting();

        EditorUtility.SetDirty(gmGO);
    }

    static Material GetOrCreateDefaultMaterial()
    {
        // Try to find existing URP Lit material
        string matPath = "Assets/Materials/URPLit.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat != null) return mat;

        // Create folder
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");

        // Create a URP Lit (Universal Render Pipeline/Lit) material
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard"); // fallback
        mat = new Material(shader);
        mat.color = Color.white;
        AssetDatabase.CreateAsset(mat, matPath);
        return mat;
    }

    static void CreateUICanvas(GameManager gm, Material urpLit)
    {
        // Create main Canvas
        var canvasGO = new GameObject("UICanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // EventSystem needed for UI
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Sanctuary Screen
        var sanctuaryGO = CreatePanel(canvasGO.transform, "SanctuaryScreen", new Color(0.04f, 0.08f, 0.04f, 1f));
        var sUI = sanctuaryGO.AddComponent<SanctuaryUI>();
        sUI.sanctuaryRoot = sanctuaryGO;

        // Title - pinned to top
        var titleTmp = CreateText(sanctuaryGO.transform, "Title", "ANIMAL ESCAPE", 60, new Color(0.44f, 0.878f, 0f),
            Vector2.zero, new Vector2(800, 80));
        PinTop(titleTmp.rectTransform, 0, -60);

        var subTmp = CreateText(sanctuaryGO.transform, "Subtitle", "RUN TO FREEDOM", 24, new Color(0.024f, 0.839f, 0.714f),
            Vector2.zero, new Vector2(800, 40));
        PinTop(subTmp.rectTransform, 0, -140);

        // Bank text - pinned to top
        sUI.bankText = CreateText(sanctuaryGO.transform, "BankText", "0 coins", 30, new Color(0.96f, 0.8f, 0.36f),
            Vector2.zero, new Vector2(400, 50));
        PinTop(sUI.bankText.rectTransform, 0, -195);

        // Animal grid container - pinned to top, below bank
        var gridGO = new GameObject("AnimalGrid");
        gridGO.transform.SetParent(sanctuaryGO.transform, false);
        var gridRT = gridGO.AddComponent<RectTransform>();
        gridRT.sizeDelta = new Vector2(960, 450);
        PinTop(gridRT, 0, -250);
        var gridLayout = gridGO.AddComponent<UnityEngine.UI.GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(125, 150);
        gridLayout.spacing = new Vector2(8, 10);
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        sUI.animalGridContainer = gridGO.transform;

        // Selected panel - centered
        sUI.selectedNameText = CreateText(sanctuaryGO.transform, "SelectedName", "Brave Piggy", 36, Color.white,
            new Vector2(0, -80), new Vector2(800, 50));
        sUI.selectedDescText = CreateText(sanctuaryGO.transform, "SelectedDesc", "", 20, new Color(0.024f, 0.839f, 0.714f),
            new Vector2(0, -125), new Vector2(800, 40));
        sUI.selectedStatsText = CreateText(sanctuaryGO.transform, "SelectedStats", "SPD 0  RES 1  STH 0", 22, Color.white,
            new Vector2(0, -165), new Vector2(800, 50));

        // Start button - pinned to bottom
        var startBtnGO = CreateButton(sanctuaryGO.transform, "StartRunBtn", "BEGIN ESCAPE",
            Vector2.zero, new Vector2(500, 80), new Color(0.44f, 0.878f, 0f));
        PinBottom(startBtnGO.GetComponent<RectTransform>(), 0, 180);
        sUI.startRunButton = startBtnGO.GetComponent<UnityEngine.UI.Button>();
        sUI.startRunButtonText = startBtnGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // Debug buttons - pinned to bottom
        var debugResetBtnGO = CreateButton(sanctuaryGO.transform, "DebugResetBtn", "Reset Progress",
            Vector2.zero, new Vector2(220, 50), new Color(0.3f, 0.3f, 0.3f));
        PinBottom(debugResetBtnGO.GetComponent<RectTransform>(), -130, 70);
        sUI.debugResetButton = debugResetBtnGO.GetComponent<UnityEngine.UI.Button>();

        var debugCoinsBtnGO = CreateButton(sanctuaryGO.transform, "DebugCoinsBtn", "+5000 Coins",
            Vector2.zero, new Vector2(220, 50), new Color(0.3f, 0.3f, 0.3f));
        PinBottom(debugCoinsBtnGO.GetComponent<RectTransform>(), 130, 70);
        sUI.debugCoinsButton = debugCoinsBtnGO.GetComponent<UnityEngine.UI.Button>();

        gm.sanctuaryUI = sUI;

        // Create animal card prefab inline (will exist in scene; user can extract to prefab)
        var cardPrefab = CreateAnimalCardPrefab(canvasGO.transform);
        cardPrefab.SetActive(false);
        sUI.animalCardPrefab = cardPrefab;

        // --- HUD ---
        var hudGO = CreatePanel(canvasGO.transform, "HUD", new Color(0, 0, 0, 0));
        var hud = hudGO.AddComponent<HUDController>();
        hudGO.SetActive(false); // starts hidden

        // HUD stats row - pinned to top-left
        hud.diceText = CreateText(hudGO.transform, "DiceText", "100", 28, Color.white,
            Vector2.zero, new Vector2(120, 40));
        PinTopLeft(hud.diceText.rectTransform, 30, -30);

        hud.cashText = CreateText(hudGO.transform, "CashText", "0", 28, new Color(0.96f, 0.8f, 0.36f),
            Vector2.zero, new Vector2(120, 40));
        PinTopLeft(hud.cashText.rectTransform, 160, -30);

        hud.mealsText = CreateText(hudGO.transform, "MealsText", "0", 28, new Color(1f, 0.62f, 0.11f),
            Vector2.zero, new Vector2(120, 40));
        PinTopLeft(hud.mealsText.rectTransform, 290, -30);

        hud.alertText = CreateText(hudGO.transform, "AlertText", "0", 28, new Color(0.9f, 0.22f, 0.27f),
            Vector2.zero, new Vector2(120, 40));
        PinTopLeft(hud.alertText.rectTransform, 420, -30);

        // Dice timer fill - pinned to top-left, below stats
        var timerBG = CreateImage(hudGO.transform, "DiceTimerBG", new Color(1, 1, 1, 0.1f),
            Vector2.zero, new Vector2(120, 4));
        PinTopLeft(timerBG.GetComponent<RectTransform>(), 30, -75);
        var timerFill = CreateImage(hudGO.transform, "DiceTimerFill", new Color(0.44f, 0.878f, 0f),
            Vector2.zero, new Vector2(120, 4));
        PinTopLeft(timerFill.GetComponent<RectTransform>(), 30, -75);
        hud.diceTimerFill = timerFill.GetComponent<UnityEngine.UI.Image>();
        hud.diceTimerFill.type = UnityEngine.UI.Image.Type.Filled;
        hud.diceTimerFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

        // Progress track - pinned to top-center
        var trackFillGO = CreateImage(hudGO.transform, "TrackFill", new Color(0.024f, 0.839f, 0.714f),
            Vector2.zero, new Vector2(900, 8));
        PinTop(trackFillGO.GetComponent<RectTransform>(), 0, -95);
        hud.trackFill = trackFillGO.GetComponent<UnityEngine.UI.Image>();
        hud.trackFill.type = UnityEngine.UI.Image.Type.Filled;
        hud.trackFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

        hud.trackProgressText = CreateText(hudGO.transform, "TrackProgress", "0/80", 16, new Color(0.024f, 0.839f, 0.714f),
            Vector2.zero, new Vector2(100, 20));
        PinTop(hud.trackProgressText.rectTransform, 0, -108);

        // Ribbon - pinned to top-center, below track
        hud.ribbonAnimalText = CreateText(hudGO.transform, "RibbonAnimal", "Brave Piggy", 18, Color.white,
            Vector2.zero, new Vector2(300, 25));
        PinTop(hud.ribbonAnimalText.rectTransform, -180, -130);

        hud.ribbonSpdText = CreateText(hudGO.transform, "RibbonSpd", "SPD 0", 16, new Color(1f, 0.62f, 0.11f),
            Vector2.zero, new Vector2(80, 25));
        PinTop(hud.ribbonSpdText.rectTransform, 80, -130);

        hud.ribbonArmText = CreateText(hudGO.transform, "RibbonArm", "RES 0", 16, new Color(0.23f, 0.53f, 1f),
            Vector2.zero, new Vector2(80, 25));
        PinTop(hud.ribbonArmText.rectTransform, 180, -130);

        hud.ribbonStlText = CreateText(hudGO.transform, "RibbonStl", "STH 0", 16, new Color(0.66f, 0.33f, 0.97f),
            Vector2.zero, new Vector2(80, 25));
        PinTop(hud.ribbonStlText.rectTransform, 280, -130);

        // Feedback texts
        hud.feedbackText = CreateText(hudGO.transform, "FeedbackText", "", 30, Color.white,
            new Vector2(0, 100), new Vector2(700, 50));
        hud.feedbackGroup = hud.feedbackText.gameObject.AddComponent<CanvasGroup>();
        hud.feedbackGroup.alpha = 0;

        hud.diceResultText = CreateText(hudGO.transform, "DiceResultText", "", 80, Color.white,
            new Vector2(0, -200), new Vector2(200, 120));
        hud.diceResultGroup = hud.diceResultText.gameObject.AddComponent<CanvasGroup>();
        hud.diceResultGroup.alpha = 0;

        // Roll button - pinned to bottom-center
        var rollBtnGO = CreateButton(hudGO.transform, "RollBtn", "RUN!",
            Vector2.zero, new Vector2(280, 80), new Color(0.44f, 0.878f, 0f));
        PinBottom(rollBtnGO.GetComponent<RectTransform>(), 40, 90);
        hud.rollButton = rollBtnGO.GetComponent<UnityEngine.UI.Button>();

        hud.rollCostText = CreateText(rollBtnGO.transform, "RollCost", "Cost: 1 dice", 14, new Color(0, 0, 0, 0.6f),
            new Vector2(0, -25), new Vector2(200, 20));

        // Multiplier button - pinned to bottom, left of roll
        var multBtnGO = CreateButton(hudGO.transform, "MultBtn", "x1",
            Vector2.zero, new Vector2(80, 80), new Color(0.44f, 0.878f, 0f, 0.15f));
        PinBottom(multBtnGO.GetComponent<RectTransform>(), -160, 90);
        hud.multiplierButton = multBtnGO.GetComponent<UnityEngine.UI.Button>();
        hud.multiplierText = multBtnGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // Upgrade button - pinned to top-right
        var upgBtnGO = CreateButton(hudGO.transform, "UpgradeBtn", "Shop",
            Vector2.zero, new Vector2(100, 50), new Color(0.44f, 0.878f, 0f, 0.08f));
        PinTopRight(upgBtnGO.GetComponent<RectTransform>(), -30, -30);
        hud.upgradeButton = upgBtnGO.GetComponent<UnityEngine.UI.Button>();

        gm.hud = hud;

        // --- Overlay Manager ---
        var omGO = new GameObject("OverlayManager");
        omGO.transform.SetParent(canvasGO.transform, false);
        var om = omGO.AddComponent<OverlayManager>();

        // Choice button prefab
        var choicePrefab = CreateButton(omGO.transform, "ChoiceButtonPrefab", "Choice",
            Vector2.zero, new Vector2(500, 50), new Color(1, 1, 1, 0.04f));
        choicePrefab.SetActive(false);
        om.choiceButtonPrefab = choicePrefab;

        // Event overlay
        om.eventOverlay = CreateOverlayPanel(canvasGO.transform, "EventOverlay", out var eventContent);
        om.eventTitleText = CreateText(eventContent.transform, "EventTitle", "Event", 36, new Color(0.44f, 0.878f, 0f),
            new Vector2(0, 80), new Vector2(500, 50));
        om.eventDescText = CreateText(eventContent.transform, "EventDesc", "Something happened!", 20, Color.white,
            new Vector2(0, 20), new Vector2(500, 60));
        var eventChoicesGO = new GameObject("EventChoices");
        eventChoicesGO.transform.SetParent(eventContent.transform, false);
        var ecRT = eventChoicesGO.AddComponent<RectTransform>();
        ecRT.anchoredPosition = new Vector2(0, -50);
        ecRT.sizeDelta = new Vector2(500, 200);
        eventChoicesGO.AddComponent<UnityEngine.UI.VerticalLayoutGroup>().spacing = 10;
        om.eventChoicesContainer = eventChoicesGO.transform;
        var eventCloseBtnGO = CreateButton(eventContent.transform, "EventCloseBtn", "CONTINUE",
            new Vector2(0, -140), new Vector2(200, 50), new Color(0.44f, 0.878f, 0f));
        om.eventCloseButton = eventCloseBtnGO.GetComponent<UnityEngine.UI.Button>();
        om.eventOverlay.SetActive(false);

        // Farmer overlay
        om.farmerOverlay = CreateOverlayPanel(canvasGO.transform, "FarmerOverlay", out var farmerContent);
        om.farmerTitleText = CreateText(farmerContent.transform, "FarmerTitle", "FARMER AHEAD!", 36, new Color(0.9f, 0.22f, 0.27f),
            new Vector2(0, 80), new Vector2(500, 50));
        om.farmerDescText = CreateText(farmerContent.transform, "FarmerDesc", "", 20, Color.white,
            new Vector2(0, 20), new Vector2(500, 60));
        var farmerChoicesGO = new GameObject("FarmerChoices");
        farmerChoicesGO.transform.SetParent(farmerContent.transform, false);
        var fcRT = farmerChoicesGO.AddComponent<RectTransform>();
        fcRT.anchoredPosition = new Vector2(0, -60);
        fcRT.sizeDelta = new Vector2(500, 250);
        farmerChoicesGO.AddComponent<UnityEngine.UI.VerticalLayoutGroup>().spacing = 10;
        om.farmerChoicesContainer = farmerChoicesGO.transform;
        om.farmerOverlay.SetActive(false);

        // Lap overlay
        om.lapOverlay = CreateOverlayPanel(canvasGO.transform, "LapOverlay", out var lapContent);
        om.lapTitleText = CreateText(lapContent.transform, "LapTitle", "FREEDOM!", 48, new Color(0.44f, 0.878f, 0f),
            new Vector2(0, 100), new Vector2(500, 60));
        om.lapDescText = CreateText(lapContent.transform, "LapDesc", "", 20, new Color(0.024f, 0.839f, 0.714f),
            new Vector2(0, 40), new Vector2(500, 40));
        om.lapRewardsText = CreateText(lapContent.transform, "LapRewards", "", 22, Color.white,
            new Vector2(0, -30), new Vector2(500, 80));
        var lapKeepBtnGO = CreateButton(lapContent.transform, "LapKeepBtn", "KEEP RUNNING",
            new Vector2(0, -120), new Vector2(400, 60), new Color(0.44f, 0.878f, 0f));
        om.lapKeepRunningButton = lapKeepBtnGO.GetComponent<UnityEngine.UI.Button>();
        var lapSanctuaryBtnGO = CreateButton(lapContent.transform, "LapSanctuaryBtn", "BACK TO SANCTUARY",
            new Vector2(0, -190), new Vector2(400, 50), new Color(0.44f, 0.878f, 0f, 0.06f));
        om.lapSanctuaryButton = lapSanctuaryBtnGO.GetComponent<UnityEngine.UI.Button>();
        om.lapOverlay.SetActive(false);

        // Upgrade overlay
        om.upgradeOverlay = CreateOverlayPanel(canvasGO.transform, "UpgradeOverlay", out var upgradeContent);
        CreateText(upgradeContent.transform, "UpgradeTitle", "NATURE'S BOUNTY", 36, new Color(0.44f, 0.878f, 0f),
            new Vector2(0, 150), new Vector2(500, 50));
        om.upgradeCashText = CreateText(upgradeContent.transform, "UpgradeCash", "0 coins", 24, new Color(0.96f, 0.8f, 0.36f),
            new Vector2(0, 100), new Vector2(500, 35));
        var upgradeGridGO = new GameObject("UpgradeGrid");
        upgradeGridGO.transform.SetParent(upgradeContent.transform, false);
        var ugRT = upgradeGridGO.AddComponent<RectTransform>();
        ugRT.anchoredPosition = new Vector2(0, -30);
        ugRT.sizeDelta = new Vector2(500, 400);
        upgradeGridGO.AddComponent<UnityEngine.UI.VerticalLayoutGroup>().spacing = 8;
        om.upgradeGridContainer = upgradeGridGO.transform;
        var upgradeCloseBtnGO = CreateButton(upgradeContent.transform, "UpgradeCloseBtn", "BACK TO PATH",
            new Vector2(0, -250), new Vector2(250, 50), new Color(0.44f, 0.878f, 0f));
        om.upgradeCloseButton = upgradeCloseBtnGO.GetComponent<UnityEngine.UI.Button>();
        om.upgradeOverlay.SetActive(false);

        // Create upgrade item prefab inline
        var upgradeItemPrefab = CreateUpgradeItemPrefab(omGO.transform);
        upgradeItemPrefab.SetActive(false);
        om.upgradeItemPrefab = upgradeItemPrefab;

        gm.overlayManager = om;
    }

    static GameObject CreateAnimalCardPrefab(Transform parent)
    {
        var go = new GameObject("AnimalCardPrefab");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 140);

        var bg = go.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.1f, 0.18f, 0.1f);

        var card = go.AddComponent<AnimalCardUI>();
        card.background = bg;
        card.button = go.AddComponent<UnityEngine.UI.Button>();

        card.emojiText = CreateText(go.transform, "Emoji", "P", 32, Color.white,
            new Vector2(0, 20), new Vector2(100, 40));
        card.nameText = CreateText(go.transform, "Name", "Animal", 14, Color.white,
            new Vector2(0, -15), new Vector2(110, 25));
        card.priceText = CreateText(go.transform, "Price", "0 coins", 12, new Color(0.96f, 0.8f, 0.36f),
            new Vector2(0, -35), new Vector2(110, 20));
        card.mealsText = CreateText(go.transform, "Meals", "0 meals", 11, new Color(1f, 0.62f, 0.11f),
            new Vector2(0, -50), new Vector2(110, 20));

        var lockGO = new GameObject("LockIcon");
        lockGO.transform.SetParent(go.transform, false);
        var lockRT = lockGO.AddComponent<RectTransform>();
        lockRT.anchoredPosition = new Vector2(40, 50);
        lockRT.sizeDelta = new Vector2(20, 20);
        var lockTxt = lockGO.AddComponent<TMPro.TextMeshProUGUI>();
        lockTxt.text = "X";
        lockTxt.fontSize = 14;
        lockTxt.alignment = TMPro.TextAlignmentOptions.Center;
        card.lockIcon = lockGO;

        card.selectedBorder = go.AddComponent<UnityEngine.UI.Outline>() == null ? null : null;
        // Use background tint for selection instead

        return go;
    }

    static GameObject CreateUpgradeItemPrefab(Transform parent)
    {
        var go = new GameObject("UpgradeItemPrefab");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 60);

        var bg = go.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(1, 1, 1, 0.03f);

        var item = go.AddComponent<UpgradeItemUI>();
        item.button = go.AddComponent<UnityEngine.UI.Button>();
        item.canvasGroup = go.AddComponent<CanvasGroup>();

        item.iconText = CreateText(go.transform, "Icon", "?", 28, Color.white,
            new Vector2(-210, 0), new Vector2(40, 40));
        item.nameText = CreateText(go.transform, "Name", "Upgrade", 18, Color.white,
            new Vector2(-80, 8), new Vector2(250, 25));
        item.descText = CreateText(go.transform, "Desc", "Description", 13, new Color(0.024f, 0.839f, 0.714f),
            new Vector2(-80, -12), new Vector2(250, 20));
        item.priceText = CreateText(go.transform, "Price", "0 coins", 20, new Color(0.96f, 0.8f, 0.36f),
            new Vector2(200, 0), new Vector2(100, 30));

        return go;
    }

    static GameObject CreateOverlayPanel(Transform parent, string name, out GameObject contentPanel)
    {
        var overlay = new GameObject(name);
        overlay.transform.SetParent(parent, false);
        var rt = overlay.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        var bg = overlay.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.88f);

        contentPanel = new GameObject("Content");
        contentPanel.transform.SetParent(overlay.transform, false);
        var crt = contentPanel.AddComponent<RectTransform>();
        crt.sizeDelta = new Vector2(600, 500);
        var cbg = contentPanel.AddComponent<UnityEngine.UI.Image>();
        cbg.color = new Color(0.1f, 0.18f, 0.1f);

        return overlay;
    }

    static GameObject CreatePanel(Transform parent, string name, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = bgColor;
        return go;
    }

    static TMPro.TextMeshProUGUI CreateText(Transform parent, string name, string text, int fontSize, Color color,
        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        var tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        return tmp;
    }

    static GameObject CreateImage(Transform parent, string name, Color color, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = bgColor;
        var btn = go.AddComponent<UnityEngine.UI.Button>();
        btn.targetGraphic = img;

        var txt = CreateText(go.transform, "Label", label, 22, Color.white, Vector2.zero, size);
        return go;
    }

    static void SetupLighting()
    {
        // Set camera clear color
        var cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.3f, 0.65f, 0.81f); // light blue sky
        }

        // Hemispheric-like light (ambient)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.5f, 0.3f);

        // Directional light (sun)
        var existingLight = Object.FindAnyObjectByType<Light>();
        if (existingLight != null)
        {
            existingLight.color = new Color(1f, 0.96f, 0.88f);
            existingLight.intensity = 1.5f;
            existingLight.shadows = LightShadows.Soft;
            existingLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }

    // --- Anchor helper methods for portrait-safe UI layout ---
    static void PinTop(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(x, y);
    }

    static void PinTopLeft(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(x, y);
    }

    static void PinTopRight(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(x, y);
    }

    static void PinBottom(RectTransform rt, float x, float y)
    {
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(x, y);
    }
}
#endif
