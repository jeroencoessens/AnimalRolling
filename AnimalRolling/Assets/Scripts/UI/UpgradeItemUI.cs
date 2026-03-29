using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Individual upgrade item in the shop grid.
/// Attach to the UpgradeItem prefab.
/// </summary>
public class UpgradeItemUI : MonoBehaviour
{
    public TextMeshProUGUI iconText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI priceText;
    public Button button;
    public CanvasGroup canvasGroup;

    public void Setup(UpgradeDefinition up, bool canAfford)
    {
        if (iconText != null) iconText.text = up.icon;
        if (nameText != null) nameText.text = up.upgradeName;
        if (descText != null) descText.text = up.description;
        if (priceText != null) priceText.text = up.cost.ToString("N0") + " coins";

        if (button != null) button.interactable = canAfford;
        if (canvasGroup != null) canvasGroup.alpha = canAfford ? 1f : 0.35f;
    }
}
