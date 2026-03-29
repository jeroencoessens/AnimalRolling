using UnityEngine;

/// <summary>
/// Manages farmer encounter UI and Chicken Flock encounter decisions.
/// </summary>
public class EncounterManager : MonoBehaviour
{
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
    }

    public void ShowFarmerEncounter(System.Action onResolved)
    {
        var run = gm.run;
        int bribeCost = gm.GetBribeCost();

        var choices = new OverlayManager.ChoiceData[]
        {
            new OverlayManager.ChoiceData
            {
                label = "Bribe the farmer (" + bribeCost + " coins)",
                enabled = run.cash >= bribeCost,
                action = () =>
                {
                    run.cash -= bribeCost;
                    run.bribeCount++;
                    gm.overlayManager.CloseFarmerOverlay();
                    onResolved?.Invoke();
                }
            },
            new OverlayManager.ChoiceData
            {
                label = "Use " + (run.animalDef != null ? run.animalDef.abilityName : "ability"),
                enabled = !run.abilityUsed,
                action = () =>
                {
                    run.abilityUsed = true;
                    gm.overlayManager.CloseFarmerOverlay();
                    onResolved?.Invoke();
                }
            },
            new OverlayManager.ChoiceData
            {
                label = "Sacrifice a Powerup (" + run.purchasedUpgrades.Count + " owned)",
                enabled = run.purchasedUpgrades.Count > 0,
                action = () =>
                {
                    SacrificePowerup();
                    gm.overlayManager.CloseFarmerOverlay();
                    gm.hud.ShowFeedback("Dropped a powerup as a distraction!");
                    onResolved?.Invoke();
                }
            },
            new OverlayManager.ChoiceData
            {
                label = "Run for it! (4-6 success)",
                enabled = true,
                action = () =>
                {
                    int roll = Random.Range(1, 7);
                    gm.hud.ShowDiceResult(roll);
                    gm.overlayManager.CloseFarmerOverlay();
                    if (roll >= 4)
                    {
                        gm.overlayManager.ShowEventPopup("SUCCESS!", "You outran the farmer!", () => onResolved?.Invoke());
                    }
                    else
                    {
                        gm.overlayManager.ShowEventPopup("FAILURE!", "The farmer caught you!", () =>
                        {
                            run.cash = 0;
                            gm.ReturnToSanctuary();
                        });
                    }
                }
            }
        };

        gm.overlayManager.ShowFarmerOverlay("FARMER AHEAD!", "A farmer is looking for you! How do you escape?", choices);
    }

    public void ShowFlockFarmerEncounter(System.Action onResolved)
    {
        var run = gm.run;
        int total = run.frontChickens + run.rearChickens;

        var choices = new OverlayManager.ChoiceData[]
        {
            new OverlayManager.ChoiceData
            {
                label = "Sacrifice a Chicken (" + total + " left)",
                enabled = total > 1,
                action = () =>
                {
                    gm.playerController.SacrificeRandomChicken();
                    gm.overlayManager.CloseFarmerOverlay();
                    gm.hud.ShowFeedback("Left a chicken behind as bait!");
                    gm.hud.RefreshRibbon(run);
                    onResolved?.Invoke();
                }
            },
            new OverlayManager.ChoiceData
            {
                label = "Sacrifice a Powerup (" + run.purchasedUpgrades.Count + " owned)",
                enabled = run.purchasedUpgrades.Count > 0,
                action = () =>
                {
                    SacrificePowerup();
                    gm.overlayManager.CloseFarmerOverlay();
                    gm.hud.ShowFeedback("Dropped a powerup as a distraction!");
                    onResolved?.Invoke();
                }
            },
            new OverlayManager.ChoiceData
            {
                label = "Run for it! (4-6 success)",
                enabled = true,
                action = () =>
                {
                    int roll = Random.Range(1, 7);
                    gm.hud.ShowDiceResult(roll);
                    gm.overlayManager.CloseFarmerOverlay();
                    if (roll >= 4)
                    {
                        gm.overlayManager.ShowEventPopup("SUCCESS!", "The flock outran the farmer!", () => onResolved?.Invoke());
                    }
                    else
                    {
                        gm.playerController.SacrificeRandomChicken();
                        int remaining = run.frontChickens + run.rearChickens;
                        if (remaining <= 0)
                        {
                            gm.overlayManager.ShowEventPopup("ALL LOST!", "The farmer caught all your chickens!", () =>
                            {
                                run.cash = 0;
                                gm.ReturnToSanctuary();
                            });
                        }
                        else
                        {
                            gm.overlayManager.ShowEventPopup("CAUGHT!", remaining + " chickens remaining.", () =>
                            {
                                gm.hud.RefreshRibbon(run);
                                onResolved?.Invoke();
                            });
                        }
                    }
                }
            }
        };

        gm.overlayManager.ShowFarmerOverlay("FARMER AHEAD!", "A farmer is looking for your flock!", choices);
    }

    void SacrificePowerup()
    {
        var run = gm.run;
        if (run.purchasedUpgrades.Count == 0) return;
        int idx = Random.Range(0, run.purchasedUpgrades.Count);
        run.purchasedUpgrades.RemoveAt(idx);
    }
}
