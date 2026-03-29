using UnityEngine;

/// <summary>
/// Wires up button clicks to the TurnController and UpgradeShop at runtime.
/// Attach to the same GameObject as GameManager.
/// </summary>
public class GameWiring : MonoBehaviour
{
    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        var turn = FindAnyObjectByType<TurnController>();
        var shop = FindAnyObjectByType<UpgradeShop>();

        // HUD buttons
        if (gm.hud != null)
        {
            if (gm.hud.rollButton != null && turn != null)
                gm.hud.rollButton.onClick.AddListener(turn.DoRoll);

            if (gm.hud.multiplierButton != null && turn != null)
                gm.hud.multiplierButton.onClick.AddListener(turn.CycleMultiplier);

            if (gm.hud.upgradeButton != null && shop != null)
                gm.hud.upgradeButton.onClick.AddListener(shop.OpenShop);
        }
    }
}
