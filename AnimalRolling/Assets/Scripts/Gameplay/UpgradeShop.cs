using UnityEngine;

/// <summary>
/// Handles in-run upgrade shop: purchasing and applying powerup effects.
/// </summary>
public class UpgradeShop : MonoBehaviour
{
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
    }

    public void OpenShop()
    {
        gm.overlayManager.ShowUpgradeShop(gm.upgrades, gm.run, OnPurchase);
    }

    void OnPurchase(UpgradeDefinition up)
    {
        var run = gm.run;
        if (run.cash < up.cost) return;

        run.cash -= up.cost;
        run.purchasedUpgrades.Add(up.id);
        ApplyUpgrade(up);
        gm.hud.RefreshAll(run);
        gm.hud.RefreshRibbon(run);

        // Refresh the shop display
        gm.overlayManager.ShowUpgradeShop(gm.upgrades, gm.run, OnPurchase);
    }

    void ApplyUpgrade(UpgradeDefinition up)
    {
        var run = gm.run;
        var cfg = gm.config;

        // Stat bonuses
        switch (up.stat)
        {
            case UpgradeStat.Dice:
                run.dice = Mathf.Min(cfg.maxDice, run.dice + up.diceAmount);
                break;
            case UpgradeStat.Speed:
                run.runSpeed += up.statAmount;
                break;
            case UpgradeStat.Stealth:
                run.runStealth += up.statAmount;
                break;
            case UpgradeStat.Armor:
                run.runArmor += up.statAmount;
                break;
            case UpgradeStat.Fuel:
                // Meals handled below
                break;
            case UpgradeStat.None:
                break;
        }

        // Meal effects
        switch (up.mealEffect)
        {
            case MealEffect.Passive:
                float passiveAmount = up.tier == UpgradeTier.Cheap ? cfg.mealPassiveCheap
                                    : up.tier == UpgradeTier.Mid   ? cfg.mealPassiveMid
                                    :                                cfg.mealPassiveExpensive;
                run.passiveMealBonus += passiveAmount;

                int tileBonus = up.tier == UpgradeTier.Cheap ? cfg.mealTileBonusCheap
                              : up.tier == UpgradeTier.Mid   ? cfg.mealTileBonusMid
                              :                                cfg.mealTileBonusExpensive;
                run.mealTileBonus += tileBonus;
                break;

            case MealEffect.Instant:
                int instantMeals = up.tier == UpgradeTier.Cheap ? cfg.mealInstantCheap
                                 : up.tier == UpgradeTier.Mid   ? cfg.mealInstantMid
                                 :                                cfg.mealInstantExpensive;
                if (instantMeals > 0) gm.AwardMeals(instantMeals);
                break;

            case MealEffect.TileAlter:
                run.tileAlterCount++;
                gm.ApplyTileAlterations();
                break;
        }

        gm.hud.ShowFeedback(up.upgradeName + " used!");
        gm.WriteSave();
    }
}
