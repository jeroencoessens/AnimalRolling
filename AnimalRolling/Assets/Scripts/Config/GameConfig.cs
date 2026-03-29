using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "AnimalEscape/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Board")]
    public int boardSize = 80;
    public float tileSize = 3f;
    public float tileSpacing = 6f;

    [Header("Dice / Rolls")]
    public int startingDice = 100;
    public int maxDice = 250;
    public int diceRefillAmount = 10;
    public float diceRefillTime = 30f; // seconds

    [Header("3D Positioning")]
    public float playerTileOffset = 0.5f;
    public float propSurfaceOffset = 1.0f;

    [Header("Meals")]
    public int mealTileBase = 3;
    public int mealRoadBase = 0;
    public int mealLapBonus = 8;
    public int mealGoldenThreshold = 250;

    [Header("Meal Passive Bonuses")]
    public float mealPassiveCheap = 0.4f;
    public float mealPassiveMid = 0.8f;
    public float mealPassiveExpensive = 0f;

    [Header("Meal Tile Bonuses")]
    public int mealTileBonusCheap = 0;
    public int mealTileBonusMid = 2;
    public int mealTileBonusExpensive = 0;

    [Header("Meal Instant Grants")]
    public int mealInstantCheap = 5;
    public int mealInstantMid = 3;
    public int mealInstantExpensive = 0;

    [Header("Tile Alteration")]
    public float tileAlterChance = 0.35f;

    [Header("Coins")]
    public int coinRoad = 30;
    public int coinCashLow = 80;
    public int coinCashMid = 120;
    public int coinCashHigh = 180;
    public int coinLapBonus = 500;
    public int coinTractorClear = 120;

    [Header("Farmer / Tractor")]
    public int farmerBribeBase = 400;
    public int farmerBribeEscalation = 200;
    public float farmerSpawnChance = 0.25f;

    [Header("Alert")]
    public int alertCashTile = 2;
    public int alertDangerFail = 5;
    public int alertFarmerSpotted = 15;
    public int alertPitReduce = 10;
    public int alertFlockReduce = 10;
    public int alertSneakReduce = 10;

    [Header("Track Checkpoints")]
    public TrackCheckpoint[] trackCheckpoints = new TrackCheckpoint[]
    {
        new TrackCheckpoint { name = "Muddy Path",   percent = 12 },
        new TrackCheckpoint { name = "Creek Cross",  percent = 25 },
        new TrackCheckpoint { name = "Wild Fields",  percent = 40 },
        new TrackCheckpoint { name = "Dark Woods",   percent = 55 },
        new TrackCheckpoint { name = "Hill Top",     percent = 72 },
        new TrackCheckpoint { name = "Nature Haven", percent = 88 },
    };

    [Header("Tile Pattern (repeats every 20 tiles)")]
    public TileDefinition[] tilePattern = new TileDefinition[]
    {
        new TileDefinition { type = TileType.Start,  color = new Color(0.133f, 0.545f, 0.133f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Cash,   color = new Color(1f, 0.843f, 0f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Danger, color = new Color(1f, 0.271f, 0f) },
        new TileDefinition { type = TileType.Fuel,   color = new Color(0.196f, 0.804f, 0.196f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Farmer, color = new Color(0.902f, 0.224f, 0.275f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Bonus,  color = new Color(0f, 0.98f, 0.604f) },
        new TileDefinition { type = TileType.Cash,   color = new Color(1f, 0.843f, 0f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Event,  color = new Color(0.255f, 0.412f, 0.882f) },
        new TileDefinition { type = TileType.Fuel,   color = new Color(0.196f, 0.804f, 0.196f) },
        new TileDefinition { type = TileType.Rival,  color = new Color(1f, 0.412f, 0.706f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Road,   color = new Color(0.545f, 0.271f, 0.075f) },
        new TileDefinition { type = TileType.Cash,   color = new Color(1f, 0.843f, 0f) },
        new TileDefinition { type = TileType.Pit,    color = new Color(0.596f, 0.984f, 0.596f) },
        new TileDefinition { type = TileType.Fuel,   color = new Color(0.196f, 0.804f, 0.196f) },
    };

    // Computed
    public float BoardRadius => (boardSize * tileSpacing) / (2f * Mathf.PI);
}

[System.Serializable]
public struct TrackCheckpoint
{
    public string name;
    public float percent;
}
