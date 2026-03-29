using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the in-game HUD: stats display, dice timer, progress track, feedback text.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Stats")]
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI mealsText;
    public TextMeshProUGUI alertText;

    [Header("Dice Timer")]
    public Image diceTimerFill;

    [Header("Progress Track")]
    public Image trackFill;
    public RectTransform trackMarker;
    public TextMeshProUGUI trackMarkerText;
    public TextMeshProUGUI trackProgressText;
    public RectTransform trackCitiesContainer;
    public GameObject cityMarkerPrefab; // prefab with TextMeshProUGUI child

    [Header("Ribbon")]
    public TextMeshProUGUI ribbonAnimalText;
    public TextMeshProUGUI ribbonSpdText;
    public TextMeshProUGUI ribbonArmText;
    public TextMeshProUGUI ribbonStlText;

    [Header("Bottom Controls")]
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI rollCostText;
    public Button rollButton;
    public Button multiplierButton;
    public Button upgradeButton;

    [Header("Feedback")]
    public TextMeshProUGUI diceResultText;
    public CanvasGroup diceResultGroup;
    public TextMeshProUGUI feedbackText;
    public CanvasGroup feedbackGroup;
    public float feedbackDuration = 2f;
    public float diceResultDuration = 1.5f;

    private float feedbackTimer;
    private float diceResultTimer;

    private static readonly string[] DICE_FACES = { "\u2680", "\u2681", "\u2682", "\u2683", "\u2684", "\u2685" };

    void Update()
    {
        // Fade out feedback
        if (feedbackTimer > 0)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0 && feedbackGroup != null)
                feedbackGroup.alpha = 0;
        }
        if (diceResultTimer > 0)
        {
            diceResultTimer -= Time.deltaTime;
            if (diceResultTimer <= 0 && diceResultGroup != null)
                diceResultGroup.alpha = 0;
        }
    }

    public void RefreshAll(RunState run)
    {
        if (diceText != null) diceText.text = run.dice.ToString();
        if (cashText != null) cashText.text = run.cash.ToString();

        string animalId = run.animalDef != null ? run.animalDef.id : "brave_pig";
        int totalMeals = GameManager.Instance.GetAnimalMeals(animalId);
        if (mealsText != null) mealsText.text = totalMeals.ToString();
        if (alertText != null) alertText.text = run.heat.ToString();

        // Progress track
        int boardSize = GameManager.Instance.config.boardSize;
        float pct = ((run.tileIndex % boardSize) / (float)boardSize);
        if (trackFill != null) trackFill.fillAmount = pct;
        if (trackMarker != null)
        {
            var parent = trackMarker.parent as RectTransform;
            if (parent != null)
            {
                float width = parent.rect.width;
                trackMarker.anchoredPosition = new Vector2(pct * width, trackMarker.anchoredPosition.y);
            }
        }
        if (trackMarkerText != null && run.animalDef != null)
            trackMarkerText.text = run.animalDef.emoji;
        if (trackProgressText != null)
            trackProgressText.text = (run.tileIndex % boardSize) + "/" + boardSize;

        // Update multiplier display
        UpdateMultiplier(run.multiplier);
    }

    public void RefreshRibbon(RunState run)
    {
        if (run.animalDef == null) return;
        if (ribbonAnimalText != null)
            ribbonAnimalText.text = run.animalDef.emoji + " " + run.animalDef.animalName;
        if (ribbonSpdText != null)
            ribbonSpdText.text = "SPD " + run.GetSpeed();

        if (run.flockMode)
        {
            if (ribbonArmText != null) ribbonArmText.text = "Front " + run.frontChickens;
            if (ribbonStlText != null) ribbonStlText.text = "Rear " + run.rearChickens;
        }
        else
        {
            if (ribbonArmText != null) ribbonArmText.text = "RES " + run.GetArmor();
            if (ribbonStlText != null) ribbonStlText.text = "STH " + run.GetStealth();
        }
    }

    public void UpdateMultiplier(int multiplier)
    {
        if (multiplierText != null) multiplierText.text = "x" + multiplier;
        if (rollCostText != null) rollCostText.text = "Cost: " + multiplier + " dice";
    }

    public void UpdateDiceTimerBar(float progress)
    {
        if (diceTimerFill != null)
            diceTimerFill.fillAmount = progress;
    }

    public void ShowDiceResult(int roll)
    {
        if (diceResultText != null && diceResultGroup != null)
        {
            diceResultText.text = DICE_FACES[Mathf.Clamp(roll - 1, 0, 5)];
            diceResultGroup.alpha = 1f;
            diceResultTimer = diceResultDuration;
        }
    }

    public void ShowFeedback(string text)
    {
        if (feedbackText != null && feedbackGroup != null)
        {
            feedbackText.text = text;
            feedbackGroup.alpha = 1f;
            feedbackTimer = feedbackDuration;
        }
    }

    public void RenderTrackCities(TrackCheckpoint[] checkpoints)
    {
        if (trackCitiesContainer == null || cityMarkerPrefab == null) return;

        // Clear existing
        foreach (Transform child in trackCitiesContainer)
            Destroy(child.gameObject);

        float width = trackCitiesContainer.rect.width;
        foreach (var cp in checkpoints)
        {
            var marker = Instantiate(cityMarkerPrefab, trackCitiesContainer);
            var rt = marker.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2((cp.percent / 100f) * width, 0);
            var label = marker.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = cp.name;
        }
    }
}
