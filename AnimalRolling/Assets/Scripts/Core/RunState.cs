using System.Collections.Generic;

/// <summary>
/// Runtime state for the current run. Reset each time a new run starts.
/// </summary>
[System.Serializable]
public class RunState
{
    // Current animal
    public AnimalDefinition animalDef;
    public int runSpeed;
    public int runArmor;
    public int runStealth;

    // Resources
    public int dice;
    public int cash;
    public int fuel; // displayed meals
    public int heat; // alert

    // Meal system
    public float runMeals;
    public float passiveMealBonus;
    public int mealTileBonus;
    public int tileAlterCount;
    public HashSet<int> alteredTiles = new HashSet<int>();

    // Farmer
    public int bribeCount;

    // Progress
    public int laps;
    public int tileIndex;
    public int multiplier = 1;
    public bool isMoving;

    // Run flags
    public bool abilityUsed;
    public List<string> purchasedUpgrades = new List<string>();

    // Flock mode
    public bool flockMode;
    public int frontChickens;
    public int rearChickens;

    // Computed
    public int GetSpeed() => (animalDef != null ? animalDef.speed : 0) + runSpeed;
    public int GetArmor() => (animalDef != null ? animalDef.armor : 0) + runArmor;
    public int GetStealth() => (animalDef != null ? animalDef.stealth : 0) + runStealth;
}
