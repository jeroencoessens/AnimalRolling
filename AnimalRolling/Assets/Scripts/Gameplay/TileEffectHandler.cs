using UnityEngine;

/// <summary>
/// Handles tile landing effects: coins, meals, danger, farmer spots, rest stops, etc.
/// </summary>
public class TileEffectHandler : MonoBehaviour
{
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
    }

    public void HandleTileLanding(int physIdx)
    {
        var run = gm.run;
        var cfg = gm.config;
        var def = gm.boardBuilder.GetTileDef(physIdx);
        int mult = run.multiplier;

        // Passive meals from powerups (every tile)
        if (run.passiveMealBonus > 0)
            gm.AwardMeals(run.passiveMealBonus * mult);

        // Tile-altered meal bonus
        if (gm.IsTileAlteredToMeal(physIdx) && def.type != TileType.Fuel)
        {
            float alteredMeals = cfg.mealTileBase * mult;
            gm.AwardMeals(alteredMeals);
            gm.hud.ShowFeedback("Altered path! +" + Mathf.FloorToInt(alteredMeals) + " meals");
            gm.hud.RefreshAll(run);
            return; // Altered tiles override original effect
        }

        switch (def.type)
        {
            case TileType.Road:
                run.cash += cfg.coinRoad * mult;
                gm.hud.ShowFeedback("Peaceful path +" + (cfg.coinRoad * mult) + " coins");
                break;

            case TileType.Cash:
            {
                int patIdx = physIdx % cfg.tilePattern.Length;
                int coinAmount;
                if (patIdx <= 5) coinAmount = cfg.coinCashLow;
                else if (patIdx <= 12) coinAmount = cfg.coinCashMid;
                else coinAmount = cfg.coinCashHigh;
                run.cash += coinAmount * mult;
                run.heat += cfg.alertCashTile;
                gm.hud.ShowFeedback("Found coins! +" + (coinAmount * mult));
                break;
            }

            case TileType.Fuel:
            {
                int baseMeals = cfg.mealTileBase + run.mealTileBonus;
                float totalMeals = baseMeals * mult;
                gm.AwardMeals(totalMeals);
                gm.hud.ShowFeedback("Found a meal! +" + Mathf.FloorToInt(totalMeals));
                break;
            }

            case TileType.Danger:
                if (Random.value > 0.3f + run.GetArmor() * 0.1f)
                {
                    run.cash = Mathf.Max(0, run.cash - 100);
                    run.heat += cfg.alertDangerFail;
                    gm.overlayManager.ShowEventPopup("TRAPPED!", "Got caught in a bramble! Lost 100 coins.");
                }
                else
                {
                    gm.hud.ShowFeedback("Resilience saved you from a trap!");
                }
                break;

            case TileType.Farmer:
                if (run.flockMode)
                {
                    run.heat = Mathf.Max(0, run.heat - cfg.alertFlockReduce);
                    gm.hud.ShowFeedback("Chickens scattered! Alert reduced.");
                }
                else if (Random.value < 0.2f + run.GetStealth() * 0.1f)
                {
                    run.heat = Mathf.Max(0, run.heat - cfg.alertSneakReduce);
                    gm.hud.ShowFeedback("Slipped away unseen!");
                }
                else
                {
                    run.heat += cfg.alertFarmerSpotted;
                    gm.overlayManager.ShowEventPopup("SPOTTED!", "A farmer saw you! Alert +" + cfg.alertFarmerSpotted + ".");
                }
                break;

            case TileType.Pit:
                run.dice = Mathf.Min(cfg.maxDice, run.dice + 15);
                run.heat = Mathf.Max(0, run.heat - cfg.alertPitReduce);
                gm.hud.ShowFeedback("Rested in a burrow. +15 Rolls");
                break;

            case TileType.Bonus:
                run.cash += 50 * mult;
                gm.hud.ShowFeedback("Bonus! +50 coins");
                break;

            case TileType.Event:
                // Random event
                float r = Random.value;
                if (r < 0.33f)
                {
                    run.dice = Mathf.Min(cfg.maxDice, run.dice + 10);
                    gm.overlayManager.ShowEventPopup("Lucky Find!", "Found extra dice in the bushes! +10 rolls.");
                }
                else if (r < 0.66f)
                {
                    run.cash += 100 * mult;
                    gm.overlayManager.ShowEventPopup("Hidden Cache!", "Discovered a stash of coins! +100.");
                }
                else
                {
                    gm.AwardMeals(5);
                    gm.overlayManager.ShowEventPopup("Feast!", "Found a bountiful meal! +5 meals.");
                }
                break;

            case TileType.Rival:
                // Rival encounter — lose or gain based on stealth
                if (Random.value < 0.4f + run.GetStealth() * 0.1f)
                {
                    run.cash += 80 * mult;
                    gm.hud.ShowFeedback("Outsmarted a rival! +80 coins");
                }
                else
                {
                    run.cash = Mathf.Max(0, run.cash - 50);
                    gm.hud.ShowFeedback("A rival stole some coins! -50");
                }
                break;
        }

        gm.hud.RefreshAll(run);
    }
}
