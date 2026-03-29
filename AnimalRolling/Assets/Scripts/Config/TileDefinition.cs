public enum TileType
{
    Start,
    Road,
    Cash,
    Danger,
    Fuel,
    Farmer,
    Bonus,
    Event,
    Rival,
    Pit
}

[System.Serializable]
public struct TileDefinition
{
    public TileType type;
    public UnityEngine.Color color;
}
