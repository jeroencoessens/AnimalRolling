using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages farmer/tractor obstacles on the board.
/// </summary>
public class FarmerSpawner : MonoBehaviour
{
    [Header("Config")]
    public GameConfig config;
    public BoardBuilder boardBuilder;

    [Header("Appearance")]
    public Material farmerMaterialTemplate;
    public Color tractorColor = new Color(0.8f, 0.1f, 0.1f);
    public Color wheelColor = new Color(0.1f, 0.1f, 0.1f);

    private Dictionary<int, GameObject> farmersOnTiles = new Dictionary<int, GameObject>();
    private List<Material> materials = new List<Material>();

    public void Init()
    {
        ClearAll();
    }

    public bool HasFarmer(int tileIndex)
    {
        return farmersOnTiles.ContainsKey(tileIndex);
    }

    public void SpawnFarmer(int physIdx)
    {
        if (farmersOnTiles.ContainsKey(physIdx)) return;

        GameObject root = new GameObject("Farmer_" + physIdx);
        root.transform.position = boardBuilder.GetTileWorldPosition(physIdx);
        root.transform.rotation = boardBuilder.GetTileRotation(physIdx);

        Material tMat = new Material(farmerMaterialTemplate);
        tMat.color = tractorColor;
        materials.Add(tMat);

        Material wMat = new Material(farmerMaterialTemplate);
        wMat.color = wheelColor;
        materials.Add(wMat);

        // Tractor body
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(2.5f, 0.5f, 0f);
        body.transform.localScale = new Vector3(1.2f, 0.8f, 1.6f);
        body.GetComponent<Renderer>().material = tMat;
        DisableCollider(body);

        // Cab
        var cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cab.transform.SetParent(body.transform);
        cab.transform.localPosition = new Vector3(0f, 0.8f, -0.2f);
        cab.transform.localScale = new Vector3(0.667f, 1f, 0.5f); // relative to parent
        cab.GetComponent<Renderer>().material = tMat;
        DisableCollider(cab);

        // Engine
        var engine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        engine.transform.SetParent(body.transform);
        engine.transform.localPosition = new Vector3(0f, 0.2f, 0.7f);
        engine.transform.localScale = new Vector3(0.583f, 0.625f, 0.375f);
        engine.GetComponent<Renderer>().material = tMat;
        DisableCollider(engine);

        // Rear wheels
        foreach (var offset in new Vector3[] {
            new Vector3(-0.6f, -0.1f, -0.4f),
            new Vector3(0.6f, -0.1f, -0.4f)})
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            w.transform.SetParent(body.transform);
            w.transform.localPosition = offset;
            w.transform.localScale = new Vector3(0.667f, 0.15f, 0.667f); // diameter 0.8 relative
            w.transform.localEulerAngles = new Vector3(0, 0, 90f);
            w.GetComponent<Renderer>().material = wMat;
            DisableCollider(w);
        }

        // Front wheels
        foreach (var offset in new Vector3[] {
            new Vector3(-0.5f, -0.3f, 0.5f),
            new Vector3(0.5f, -0.3f, 0.5f)})
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            w.transform.SetParent(body.transform);
            w.transform.localPosition = offset;
            w.transform.localScale = new Vector3(0.333f, 0.1f, 0.333f);
            w.transform.localEulerAngles = new Vector3(0, 0, 90f);
            w.GetComponent<Renderer>().material = wMat;
            DisableCollider(w);
        }

        farmersOnTiles[physIdx] = root;
    }

    public void RemoveFarmer(int physIdx)
    {
        if (farmersOnTiles.TryGetValue(physIdx, out GameObject root))
        {
            Destroy(root);
            farmersOnTiles.Remove(physIdx);
        }
    }

    public void ClearAll()
    {
        foreach (var kvp in farmersOnTiles)
            if (kvp.Value != null) Destroy(kvp.Value);
        farmersOnTiles.Clear();

        foreach (var mat in materials)
            if (mat != null) Destroy(mat);
        materials.Clear();
    }

    void DisableCollider(GameObject go)
    {
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }
}
