using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scatters decorative vegetation (trees, conifers, bushes, grass) around the planet.
/// </summary>
public class PropScatterer : MonoBehaviour
{
    [Header("Config")]
    public GameConfig config;

    [Header("Prop Counts")]
    public int roundTrees = 55;
    public int conifers = 35;
    public int bushes = 48;
    public int grassTufts = 75;

    [Header("Colors")]
    public Color treeColor = new Color(0.15f, 0.65f, 0.10f);
    public Color trunkColor = new Color(0.30f, 0.15f, 0.05f);
    public Color coniferColor = new Color(0.05f, 0.42f, 0.06f);
    public Color bushColor = new Color(0.10f, 0.52f, 0.07f);
    public Color grassColor = new Color(0.28f, 0.74f, 0.12f);

    [Header("Materials")]
    public Material propMaterialTemplate;

    private GameObject propRoot;
    private List<Material> materials = new List<Material>();

    public void ScatterProps()
    {
        ClearProps();
        propRoot = new GameObject("PropRoot");
        propRoot.transform.SetParent(transform);

        float boardRadius = config.BoardRadius;

        Material treeMat = MakeFlatMat(treeColor);
        Material trunkMat = MakeFlatMat(trunkColor);
        Material coniferMat = MakeFlatMat(coniferColor);
        Material bushMat = MakeFlatMat(bushColor);
        Material grassMat = MakeFlatMat(grassColor);

        // Round-canopy trees
        for (int i = 0; i < roundTrees; i++)
        {
            float side = RandomSide(config.tileSize + 2f, 8f);
            float angle = Random.value * Mathf.PI * 2f;
            Transform anchor = CreateAnchor(side, angle, boardRadius);

            float h = 0.7f + Random.value * 0.9f;
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(anchor);
            trunk.transform.localPosition = new Vector3(0, h / 2f, 0);
            trunk.transform.localScale = new Vector3(0.15f, h / 2f, 0.15f);
            trunk.GetComponent<Renderer>().material = trunkMat;
            DisableCollider(trunk);

            var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.transform.SetParent(anchor);
            canopy.transform.localPosition = new Vector3(0, h, 0);
            float canopySize = 0.6f + Random.value * 0.5f;
            canopy.transform.localScale = Vector3.one * canopySize;
            canopy.GetComponent<Renderer>().material = treeMat;
            DisableCollider(canopy);
        }

        // Conifers
        for (int i = 0; i < conifers; i++)
        {
            float side = RandomSide(config.tileSize + 1.5f, 9f);
            float angle = Random.value * Mathf.PI * 2f;
            Transform anchor = CreateAnchor(side, angle, boardRadius);

            float h = 1.3f + Random.value * 1.0f;
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(anchor);
            trunk.transform.localPosition = new Vector3(0, h * 0.22f, 0);
            trunk.transform.localScale = new Vector3(0.12f, h * 0.225f, 0.12f);
            trunk.GetComponent<Renderer>().material = trunkMat;
            DisableCollider(trunk);

            // Bottom cone
            var cone1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone1.transform.SetParent(anchor);
            cone1.transform.localPosition = new Vector3(0, h * 0.50f, 0);
            cone1.transform.localScale = new Vector3(1.1f, h * 0.325f, 1.1f);
            cone1.GetComponent<Renderer>().material = coniferMat;
            DisableCollider(cone1);

            // Top cone
            var cone2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone2.transform.SetParent(anchor);
            cone2.transform.localPosition = new Vector3(0, h * 0.87f, 0);
            cone2.transform.localScale = new Vector3(0.65f, h * 0.25f, 0.65f);
            cone2.GetComponent<Renderer>().material = coniferMat;
            DisableCollider(cone2);
        }

        // Bushes
        for (int i = 0; i < bushes; i++)
        {
            float side = RandomSide(config.tileSize + 1f, 5.5f);
            float angle = Random.value * Mathf.PI * 2f;
            Transform anchor = CreateAnchor(side, angle, boardRadius);

            float s = 0.32f + Random.value * 0.38f;
            var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bush.transform.SetParent(anchor);
            bush.transform.localPosition = new Vector3(0, s * 0.42f, 0);
            bush.transform.localScale = new Vector3(s * 1.4f, s, s * 1.3f);
            bush.GetComponent<Renderer>().material = bushMat;
            DisableCollider(bush);

            if (Random.value > 0.45f)
            {
                var bush2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bush2.transform.SetParent(anchor);
                bush2.transform.localPosition = new Vector3(
                    (Random.value - 0.5f) * 0.55f,
                    s * 0.28f,
                    (Random.value - 0.5f) * 0.3f);
                bush2.transform.localScale = new Vector3(s * 0.9f, s * 0.72f, s * 0.9f);
                bush2.GetComponent<Renderer>().material = bushMat;
                DisableCollider(bush2);
            }
        }

        // Grass tufts
        for (int i = 0; i < grassTufts; i++)
        {
            float side = RandomSide(config.tileSize * 0.4f, 11f);
            float angle = Random.value * Mathf.PI * 2f;
            Transform anchor = CreateAnchor(side, angle, boardRadius);

            int blades = 2 + (Random.value > 0.5f ? 1 : 0);
            for (int g = 0; g < blades; g++)
            {
                var blade = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                blade.transform.SetParent(anchor);
                blade.transform.localPosition = new Vector3(
                    (Random.value - 0.5f) * 0.5f,
                    0.18f,
                    (Random.value - 0.5f) * 0.5f);
                blade.transform.localScale = new Vector3(0.04f, 0.19f, 0.14f);
                blade.transform.localEulerAngles = new Vector3(
                    0, Random.value * 180f, (Random.value - 0.5f) * 16f);
                blade.GetComponent<Renderer>().material = grassMat;
                DisableCollider(blade);
            }
        }
    }

    public void ClearProps()
    {
        foreach (var mat in materials)
            if (mat != null) Destroy(mat);
        materials.Clear();

        if (propRoot != null)
            Destroy(propRoot);
    }

    Transform CreateAnchor(float side, float angle, float boardRadius)
    {
        var anchor = new GameObject("PropAnchor").transform;
        anchor.SetParent(propRoot.transform);
        float r = boardRadius - config.propSurfaceOffset;
        anchor.localPosition = new Vector3(
            side,
            r * Mathf.Cos(angle) - boardRadius,
            r * Mathf.Sin(angle));
        anchor.localEulerAngles = new Vector3(angle * Mathf.Rad2Deg, 0, 0);
        return anchor;
    }

    float RandomSide(float minGap, float spread)
    {
        return (Random.value < 0.5f ? -1f : 1f) * (minGap + Random.value * spread);
    }

    Material MakeFlatMat(Color color)
    {
        Material mat = new Material(propMaterialTemplate);
        mat.color = color;
        materials.Add(mat);
        return mat;
    }

    void DisableCollider(GameObject go)
    {
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }
}
