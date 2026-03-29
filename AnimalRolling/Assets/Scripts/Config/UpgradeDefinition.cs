using UnityEngine;

public enum UpgradeStat { Dice, Speed, Stealth, Armor, Fuel, None }
public enum MealEffect { Passive, Instant, TileAlter }
public enum UpgradeTier { Cheap, Mid, Expensive }

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "AnimalEscape/UpgradeDefinition")]
public class UpgradeDefinition : ScriptableObject
{
    public string id;
    public string upgradeName;
    public string icon;  // emoji or sprite name

    [TextArea]
    public string description;

    public UpgradeStat stat;
    public int cost;
    public UpgradeTier tier;
    public MealEffect mealEffect;

    [Tooltip("Stat amount for speed/stealth/armor upgrades")]
    public int statAmount = 1;

    [Tooltip("Dice amount for dice upgrades")]
    public int diceAmount = 30;
}
