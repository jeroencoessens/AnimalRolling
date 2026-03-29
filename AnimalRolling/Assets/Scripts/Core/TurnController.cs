using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Orchestrates dice rolls, movement sequencing, farmer encounters, and lap completion.
/// </summary>
public class TurnController : MonoBehaviour
{
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
    }

    public void DoRoll()
    {
        var run = gm.run;
        if (run.isMoving || run.dice < run.multiplier) return;
        if (run.flockMode && run.frontChickens + run.rearChickens <= 0) return;

        // Consume dice
        run.dice -= run.multiplier;
        gm.WriteSave();

        // Roll 1-6, add speed bonus
        int roll = Random.Range(1, 7);
        roll += Mathf.FloorToInt(run.GetSpeed() / 2f);

        gm.hud.ShowDiceResult(Mathf.Min(roll, 6));
        gm.hud.ShowFeedback("Moved " + roll + " fields!");

        // Build path
        int currentTile = run.tileIndex;
        var positions = new List<Vector3>();
        var rotations = new List<Quaternion>();

        for (int i = 1; i <= roll; i++)
        {
            int next = (currentTile + i) % gm.config.boardSize;
            positions.Add(gm.boardBuilder.GetTileWorldPosition(next));
            rotations.Add(gm.boardBuilder.GetTileRotation(next));
        }

        // Rear flock group
        List<Vector3> rearPositions = null;
        List<Quaternion> rearRotations = null;
        if (run.flockMode)
        {
            rearPositions = new List<Vector3>();
            rearRotations = new List<Quaternion>();
            for (int i = 1; i <= roll; i++)
            {
                int rearNext = (currentTile + i - 1) % gm.config.boardSize;
                rearPositions.Add(gm.boardBuilder.GetTileWorldPosition(rearNext));
                rearRotations.Add(gm.boardBuilder.GetTileRotation(rearNext));
            }
        }

        run.isMoving = true;

        gm.playerController.MoveAlongPath(positions, rotations, rearPositions, rearRotations, () =>
        {
            run.isMoving = false;
            int prevIndex = run.tileIndex;
            run.tileIndex += roll;
            int physIdx = run.tileIndex % gm.config.boardSize;
            int rearPhysIdx = run.flockMode
                ? (run.tileIndex - 1 + gm.config.boardSize) % gm.config.boardSize
                : -1;

            // Check farmer encounters
            var passedFarmerIndices = CollectFarmerEncounters(currentTile, roll, physIdx, rearPhysIdx);

            System.Action finishTurn = () =>
            {
                // Lap completion
                if (run.tileIndex / gm.config.boardSize > prevIndex / gm.config.boardSize)
                {
                    run.laps++;
                    int lapCoins = gm.config.coinLapBonus * run.multiplier;
                    run.cash += lapCoins;
                    gm.AwardMeals(gm.config.mealLapBonus * run.multiplier);
                    gm.overlayManager.ShowLapOverlay(run.laps, lapCoins);
                }

                // Handle tile landing effects
                if (!run.flockMode || run.frontChickens > 0)
                    gm.tileEffectHandler.HandleTileLanding(physIdx);
                if (run.flockMode && run.rearChickens > 0)
                    gm.tileEffectHandler.HandleTileLanding(rearPhysIdx);

                // Possibly spawn farmers ahead
                if (!run.flockMode || run.frontChickens > 0)
                {
                    if (Random.value < gm.config.farmerSpawnChance)
                        gm.farmerSpawner.SpawnFarmer((physIdx + 40) % gm.config.boardSize);
                }
                if (run.flockMode && run.rearChickens > 0)
                {
                    if (Random.value < gm.config.farmerSpawnChance)
                        gm.farmerSpawner.SpawnFarmer((rearPhysIdx + 40) % gm.config.boardSize);
                }

                gm.hud.RefreshAll(run);
            };

            if (passedFarmerIndices.Count > 0)
            {
                int clearBonus = gm.config.coinTractorClear * passedFarmerIndices.Count;
                run.cash += clearBonus;
                gm.AwardMeals(2 * passedFarmerIndices.Count);
                gm.hud.ShowFeedback("Escaped the farmer! +" + clearBonus + " coins");

                bool frontLandedOnFarmer = (!run.flockMode || run.frontChickens > 0) && gm.farmerSpawner.HasFarmer(physIdx);
                bool rearLandedOnFarmer = run.flockMode && run.rearChickens > 0 && gm.farmerSpawner.HasFarmer(rearPhysIdx);

                var passedThrough = new List<int>();
                foreach (int idx in passedFarmerIndices)
                {
                    if (idx != physIdx && !(run.flockMode && idx == rearPhysIdx))
                        passedThrough.Add(idx);
                }

                foreach (int idx in passedFarmerIndices)
                    gm.farmerSpawner.RemoveFarmer(idx);

                if (frontLandedOnFarmer || rearLandedOnFarmer)
                {
                    finishTurn();
                }
                else if (passedThrough.Count > 0)
                {
                    if (run.flockMode)
                        gm.encounterManager.ShowFlockFarmerEncounter(finishTurn);
                    else
                        gm.encounterManager.ShowFarmerEncounter(finishTurn);
                }
                else
                {
                    finishTurn();
                }
            }
            else
            {
                finishTurn();
            }
        });

        gm.cameraController.AnimateAlongPath(positions, rotations);
    }

    List<int> CollectFarmerEncounters(int currentTile, int roll, int physIdx, int rearPhysIdx)
    {
        var indices = new List<int>();
        var run = gm.run;

        if (!run.flockMode || run.frontChickens > 0)
        {
            for (int i = 1; i <= roll; i++)
            {
                int checkIdx = (currentTile + i) % gm.config.boardSize;
                if (gm.farmerSpawner.HasFarmer(checkIdx) && !indices.Contains(checkIdx))
                    indices.Add(checkIdx);
            }
        }

        if (run.flockMode && run.rearChickens > 0)
        {
            int rearCheck = currentTile % gm.config.boardSize;
            if (gm.farmerSpawner.HasFarmer(rearCheck) && !indices.Contains(rearCheck))
                indices.Add(rearCheck);
        }

        return indices;
    }

    public void CycleMultiplier()
    {
        var run = gm.run;
        run.multiplier = run.multiplier == 1 ? 5 : (run.multiplier == 5 ? 20 : 1);
        gm.hud.UpdateMultiplier(run.multiplier);
    }
}
