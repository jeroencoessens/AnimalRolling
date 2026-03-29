using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Individual animal card in the sanctuary grid.
/// Attach to the AnimalCard prefab.
/// </summary>
public class AnimalCardUI : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI emojiText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI mealsText;
    public GameObject lockIcon;
    public GameObject goldenBorder;
    public Image selectedBorder;
    public Button button;

    public void Setup(AnimalDefinition animal, bool owned, bool selected, bool isGolden, int meals)
    {
        if (emojiText != null) emojiText.text = animal.emoji;
        if (nameText != null) nameText.text = animal.animalName;

        if (priceText != null)
        {
            priceText.gameObject.SetActive(!owned);
            priceText.text = animal.price.ToString("N0") + " coins";
        }

        if (mealsText != null)
        {
            mealsText.gameObject.SetActive(owned);
            mealsText.text = meals + " meals";
        }

        if (lockIcon != null)
            lockIcon.SetActive(!owned);
        if (goldenBorder != null)
            goldenBorder.SetActive(isGolden && owned);
        if (selectedBorder != null)
            selectedBorder.enabled = selected;

        // Dim if locked
        if (!owned)
        {
            var cg = GetComponent<CanvasGroup>();
            if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0.45f;
        }
    }
}
