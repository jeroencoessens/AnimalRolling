using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sanctuary (start screen) UI: animal selection grid, purchase flow, and run start.
/// </summary>
public class SanctuaryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject sanctuaryRoot;

    [Header("Bank")]
    public TextMeshProUGUI bankText;

    [Header("Animal Grid")]
    public Transform animalGridContainer;
    public GameObject animalCardPrefab; // Prefab with: Image bg, TextMeshProUGUI nameText, TextMeshProUGUI priceText, Button

    [Header("Selected Panel")]
    public TextMeshProUGUI selectedNameText;
    public TextMeshProUGUI selectedDescText;
    public TextMeshProUGUI selectedStatsText;
    public Image selectedColorImage;
    public Button startRunButton;
    public TextMeshProUGUI startRunButtonText;

    [Header("Debug")]
    public Button debugResetButton;
    public Button debugCoinsButton;

    private GameManager gm;

    void Awake()
    {
        if (debugResetButton != null)
            debugResetButton.onClick.AddListener(() => GameManager.Instance.DebugReset());
        if (debugCoinsButton != null)
            debugCoinsButton.onClick.AddListener(() => GameManager.Instance.DebugAddCoins());
    }

    void Start()
    {
        gm = GameManager.Instance;
    }

    public void Show()
    {
        sanctuaryRoot.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        sanctuaryRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (gm == null) gm = GameManager.Instance;
        if (gm == null) return;

        // Bank
        if (bankText != null)
            bankText.text = gm.persist.totalCash.ToString("N0") + " coins";

        // Build card grid
        BuildAnimalGrid();

        // Update selected panel
        UpdateSelectedPanel();
    }

    void BuildAnimalGrid()
    {
        // Clear existing
        foreach (Transform child in animalGridContainer)
            Destroy(child.gameObject);

        foreach (var animal in gm.animals)
        {
            bool owned = gm.persist.unlockedAnimals.Contains(animal.id);
            bool selected = animal.id == gm.selectedAnimalId;
            int meals = gm.GetAnimalMeals(animal.id);
            bool isGolden = meals >= gm.config.mealGoldenThreshold;

            var cardGO = Instantiate(animalCardPrefab, animalGridContainer);
            cardGO.SetActive(true);
            var card = cardGO.GetComponent<AnimalCardUI>();
            if (card != null)
            {
                card.Setup(animal, owned, selected, isGolden, meals);
                string animalId = animal.id;
                card.button.onClick.AddListener(() => OnAnimalCardClicked(animalId));
            }
        }
    }

    void OnAnimalCardClicked(string animalId)
    {
        var animal = gm.GetAnimalById(animalId);
        if (animal == null) return;

        bool owned = gm.persist.unlockedAnimals.Contains(animalId);

        if (owned)
        {
            gm.selectedAnimalId = animalId;
            Refresh();
        }
        else if (gm.persist.totalCash >= animal.price)
        {
            // Purchase
            gm.persist.totalCash -= animal.price;
            gm.persist.unlockedAnimals.Add(animalId);
            gm.selectedAnimalId = animalId;
            gm.WriteSave();
            Refresh();
        }
    }

    void UpdateSelectedPanel()
    {
        var animal = gm.GetAnimalById(gm.selectedAnimalId);
        if (animal == null) return;

        int meals = gm.GetAnimalMeals(animal.id);
        bool isGolden = meals >= gm.config.mealGoldenThreshold;
        bool owned = gm.persist.unlockedAnimals.Contains(animal.id);

        if (selectedNameText != null)
            selectedNameText.text = animal.emoji + " " + animal.animalName;
        if (selectedDescText != null)
            selectedDescText.text = animal.description;
        if (selectedStatsText != null)
        {
            string stats = "SPD " + animal.speed + "  RES " + animal.armor + "  STH " + animal.stealth;
            stats += "\nMeals: " + meals;
            if (isGolden) stats += "  GOLDEN";
            selectedStatsText.text = stats;
        }
        if (selectedColorImage != null)
            selectedColorImage.color = animal.color;

        if (startRunButton != null)
        {
            startRunButton.interactable = owned;
            startRunButton.onClick.RemoveAllListeners();
            if (owned)
            {
                if (startRunButtonText != null) startRunButtonText.text = "BEGIN ESCAPE";
                startRunButton.onClick.AddListener(() => gm.StartRun(animal));
            }
            else
            {
                if (startRunButtonText != null) startRunButtonText.text = "UNLOCK FOR " + animal.price + " coins";
            }
        }
    }
}
