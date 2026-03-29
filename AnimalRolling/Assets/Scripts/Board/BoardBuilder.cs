using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Builds the circular planet track: tiles arranged in a ring plus a core sphere.
/// </summary>
public class BoardBuilder : MonoBehaviour
{
    [Header("References")]
    public GameConfig config;
    public Material tileMaterialTemplate; // assign a URP Lit material as template

    [Header("Tile Visuals")]
    public Vector3 tileScale = new Vector3(7.5f, 0.5f, 5.4f); // width*2.5, height, depth*1.8

    private List<GameObject> tileObjects = new List<GameObject>();
    private List<TileDefinition> tileDefs = new List<TileDefinition>();
    private List<Material> tileMaterials = new List<Material>();
    private GameObject boardRoot;
    private GameObject coreObject;

    public float BoardRadius => config.BoardRadius;

    public void BuildBoard()
    {
        ClearBoard();

        boardRoot = new GameObject("BoardRoot");
        boardRoot.transform.SetParent(transform);
        float boardRadius = config.BoardRadius;

        // Material cache
        var matCache = new Dictionary<Color, Material>();

        for (int i = 0; i < config.boardSize; i++)
        {
            TileDefinition tileDef = config.tilePattern[i % config.tilePattern.Length];
            tileDefs.Add(tileDef);

            // Create tile
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + i;
            tile.transform.SetParent(boardRoot.transform);
            tile.transform.localScale = tileScale;

            // Position on circular track
            float angle = (i * config.tileSpacing) / boardRadius;
            float y = boardRadius * Mathf.Cos(angle) - boardRadius;
            float z = boardRadius * Mathf.Sin(angle);
            tile.transform.localPosition = new Vector3(0f, y, z);
            tile.transform.localEulerAngles = new Vector3(angle * Mathf.Rad2Deg, 0f, 0f);

            // Material
            if (!matCache.TryGetValue(tileDef.color, out Material mat))
            {
                mat = new Material(tileMaterialTemplate);
                mat.color = tileDef.color;
                matCache[tileDef.color] = mat;
            }
            tile.GetComponent<Renderer>().material = mat;
            tileMaterials.Add(mat);

            tileObjects.Add(tile);
        }

        // Core sphere (planet body)
        coreObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        coreObject.name = "PlanetCore";
        coreObject.transform.SetParent(boardRoot.transform);
        float coreDiameter = (boardRadius - 0.5f) * 2f;
        coreObject.transform.localScale = Vector3.one * coreDiameter;
        coreObject.transform.localPosition = new Vector3(0f, -boardRadius, 0f);

        Material coreMat = new Material(tileMaterialTemplate);
        coreMat.color = new Color(0.05f, 0.15f, 0.05f);
        coreObject.GetComponent<Renderer>().material = coreMat;
    }

    public void ClearBoard()
    {
        foreach (var mat in tileMaterials)
            if (mat != null) Destroy(mat);
        tileMaterials.Clear();

        tileObjects.Clear();
        tileDefs.Clear();

        if (boardRoot != null)
            Destroy(boardRoot);
        if (coreObject != null)
            Destroy(coreObject);
    }

    public TileDefinition GetTileDef(int index)
    {
        return tileDefs[index % tileDefs.Count];
    }

    public Vector3 GetTileWorldPosition(int index)
    {
        int idx = index % config.boardSize;
        if (idx < 0) idx += config.boardSize;
        var tile = tileObjects[idx];

        // Offset along the tile's local up direction so the player sits on top
        return tile.transform.position + tile.transform.up * config.playerTileOffset;
    }

    public Quaternion GetTileRotation(int index)
    {
        int idx = index % config.boardSize;
        if (idx < 0) idx += config.boardSize;
        return tileObjects[idx].transform.rotation;
    }

    public void TintTile(int index, Color color)
    {
        int idx = index % config.boardSize;
        if (idx < 0) idx += config.boardSize;
        Material mat = new Material(tileMaterialTemplate);
        mat.color = color;
        tileObjects[idx].GetComponent<Renderer>().material = mat;
        tileMaterials.Add(mat);
    }
}
