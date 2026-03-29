using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Manages all overlay popups: events, farmer encounters, lap completion, upgrade shop.
/// </summary>
public class OverlayManager : MonoBehaviour
{
    [Serializable]
    public class ChoiceData
    {
        public string label;
        public bool enabled;
        public Action action;
    }

    [Header("Event Overlay")]
    public GameObject eventOverlay;
    public TextMeshProUGUI eventTitleText;
    public TextMeshProUGUI eventDescText;
    public Transform eventChoicesContainer;
    public Button eventCloseButton;
    public GameObject choiceButtonPrefab;

    [Header("Farmer Overlay")]
    public GameObject farmerOverlay;
    public TextMeshProUGUI farmerTitleText;
    public TextMeshProUGUI farmerDescText;
    public Transform farmerChoicesContainer;

    [Header("Lap Overlay")]
    public GameObject lapOverlay;
    public TextMeshProUGUI lapTitleText;
    public TextMeshProUGUI lapDescText;
    public TextMeshProUGUI lapRewardsText;
    public Button lapKeepRunningButton;
    public Button lapSanctuaryButton;

    [Header("Upgrade Overlay")]
    public GameObject upgradeOverlay;
    public TextMeshProUGUI upgradeCashText;
    public Transform upgradeGridContainer;
    public Button upgradeCloseButton;
    public GameObject upgradeItemPrefab;

    private Action eventCloseCallback;
    private bool upgradeScrollInitialized;

    void Start()
    {
        if (eventCloseButton != null)
            eventCloseButton.onClick.AddListener(() =>
            {
                eventOverlay.SetActive(false);
                eventCloseCallback?.Invoke();
                eventCloseCallback = null;
            });

        if (upgradeCloseButton != null)
            upgradeCloseButton.onClick.AddListener(() => upgradeOverlay.SetActive(false));
    }

    public void ShowEventPopup(string title, string desc, Action onClose = null)
    {
        eventCloseCallback = onClose;
        if (eventTitleText != null) eventTitleText.text = title;
        if (eventDescText != null) eventDescText.text = desc;

        // Clear choices
        if (eventChoicesContainer != null)
            foreach (Transform child in eventChoicesContainer)
                Destroy(child.gameObject);

        if (eventCloseButton != null) eventCloseButton.gameObject.SetActive(true);
        eventOverlay.SetActive(true);
    }

    public void ShowEventPopupWithChoices(string title, string desc, ChoiceData[] choices)
    {
        if (eventTitleText != null) eventTitleText.text = title;
        if (eventDescText != null) eventDescText.text = desc;

        if (eventChoicesContainer != null)
        {
            foreach (Transform child in eventChoicesContainer)
                Destroy(child.gameObject);

            foreach (var choice in choices)
                CreateChoiceButton(eventChoicesContainer, choice);
        }

        if (eventCloseButton != null) eventCloseButton.gameObject.SetActive(false);
        eventOverlay.SetActive(true);
    }

    public void ShowFarmerOverlay(string title, string desc, ChoiceData[] choices)
    {
        if (farmerTitleText != null) farmerTitleText.text = title;
        if (farmerDescText != null) farmerDescText.text = desc;

        if (farmerChoicesContainer != null)
        {
            foreach (Transform child in farmerChoicesContainer)
                Destroy(child.gameObject);

            foreach (var choice in choices)
                CreateChoiceButton(farmerChoicesContainer, choice);
        }

        farmerOverlay.SetActive(true);
    }

    public void CloseFarmerOverlay()
    {
        farmerOverlay.SetActive(false);
    }

    public void ShowLapOverlay(int laps, int coinBonus)
    {
        string ordinal = laps == 1 ? "1ST" : laps == 2 ? "2ND" : laps == 3 ? "3RD" : laps + "TH";

        if (lapTitleText != null) lapTitleText.text = ordinal + " ESCAPE!";
        if (lapDescText != null)
            lapDescText.text = laps == 1 ? "You reached the wild for the first time!" : "Keep running - nature awaits!";

        int lapMeals = GameManager.Instance.config.mealLapBonus * GameManager.Instance.run.multiplier;
        if (lapRewardsText != null)
            lapRewardsText.text = "Coins: +" + coinBonus + "\nMeals: +" + lapMeals + "\nTotal laps: " + laps;

        if (lapKeepRunningButton != null)
        {
            lapKeepRunningButton.onClick.RemoveAllListeners();
            lapKeepRunningButton.onClick.AddListener(() => lapOverlay.SetActive(false));
        }

        if (lapSanctuaryButton != null)
        {
            lapSanctuaryButton.onClick.RemoveAllListeners();
            lapSanctuaryButton.onClick.AddListener(() =>
            {
                lapOverlay.SetActive(false);
                GameManager.Instance.ReturnToSanctuary();
            });
        }

        lapOverlay.SetActive(true);
    }

    public void ShowUpgradeShop(UpgradeDefinition[] upgrades, RunState run, Action<UpgradeDefinition> onPurchase)
    {
        EnsureUpgradeScrollSetup();

        if (upgradeCashText != null)
            upgradeCashText.text = run.cash.ToString("N0") + " coins";

        if (upgradeGridContainer != null)
        {
            foreach (Transform child in upgradeGridContainer)
                Destroy(child.gameObject);

            foreach (var up in upgrades)
            {
                bool canAfford = run.cash >= up.cost;
                var itemGO = Instantiate(upgradeItemPrefab, upgradeGridContainer);
                itemGO.SetActive(true);
                var item = itemGO.GetComponent<UpgradeItemUI>();
                if (item != null)
                {
                    item.Setup(up, canAfford);
                    if (canAfford)
                    {
                        var captured = up;
                        item.button.onClick.AddListener(() => onPurchase?.Invoke(captured));
                    }
                }
            }
        }

        upgradeOverlay.SetActive(true);
    }

    void EnsureUpgradeScrollSetup()
    {
        if (upgradeScrollInitialized || upgradeGridContainer == null) return;
        upgradeScrollInitialized = true;

        var gridRT = upgradeGridContainer.GetComponent<RectTransform>();
        if (gridRT == null) return;

        var contentParent = gridRT.parent;
        var contentRT = contentParent != null ? contentParent.GetComponent<RectTransform>() : null;

        // Expand the content panel to use more vertical space for portrait
        if (contentRT != null)
            contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, 900);

        // Move close button lower to accommodate taller panel
        if (upgradeCloseButton != null)
        {
            var btnRT = upgradeCloseButton.GetComponent<RectTransform>();
            if (btnRT != null)
                btnRT.anchoredPosition = new Vector2(btnRT.anchoredPosition.x, -400);
        }

        // Save grid sizing before reparenting
        float gridWidth = gridRT.sizeDelta.x;
        float viewportHeight = 600f;
        Vector2 gridPos = gridRT.anchoredPosition;

        // Add ContentSizeFitter to grid so it grows with children
        var fitter = upgradeGridContainer.gameObject.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = upgradeGridContainer.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // Create a scroll viewport that masks & clips the grid
        var viewportGO = new GameObject("UpgradeScrollViewport");
        viewportGO.transform.SetParent(contentParent, false);
        var viewportRT = viewportGO.AddComponent<RectTransform>();
        viewportRT.anchoredPosition = gridPos;
        viewportRT.sizeDelta = new Vector2(gridWidth, viewportHeight);
        viewportGO.AddComponent<RectMask2D>();

        // Reparent grid into viewport
        gridRT.SetParent(viewportRT, false);
        gridRT.anchorMin = new Vector2(0, 1);
        gridRT.anchorMax = new Vector2(1, 1);
        gridRT.pivot = new Vector2(0.5f, 1);
        gridRT.anchoredPosition = Vector2.zero;
        gridRT.sizeDelta = new Vector2(0, 0); // width stretches, height from ContentSizeFitter

        // Add ScrollRect for touch/mouse scrolling
        var scroll = viewportGO.AddComponent<ScrollRect>();
        scroll.content = gridRT;
        scroll.viewport = viewportRT;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 30f;
    }

    public void CloseAll()
    {
        if (eventOverlay != null) eventOverlay.SetActive(false);
        if (farmerOverlay != null) farmerOverlay.SetActive(false);
        if (lapOverlay != null) lapOverlay.SetActive(false);
        if (upgradeOverlay != null) upgradeOverlay.SetActive(false);
    }

    void CreateChoiceButton(Transform container, ChoiceData choice)
    {
        if (choiceButtonPrefab == null) return;
        var btnGO = Instantiate(choiceButtonPrefab, container);
        btnGO.SetActive(true);
        var label = btnGO.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = choice.label;

        var btn = btnGO.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = choice.enabled;
            if (choice.enabled)
                btn.onClick.AddListener(() => choice.action?.Invoke());
        }
    }
}
