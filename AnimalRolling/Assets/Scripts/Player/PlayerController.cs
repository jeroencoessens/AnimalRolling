using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles player spawning, procedural model generation, and hop-by-hop movement animation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    public GameConfig config;
    public BoardBuilder boardBuilder;

    [Header("Movement")]
    public float hopDuration = 0.15f; // seconds per tile hop

    [Header("Materials")]
    public Material playerMaterialTemplate;

    // Runtime references
    [HideInInspector] public Transform playerRoot;
    [HideInInspector] public Transform rearPlayerRoot; // flock rear group
    [HideInInspector] public List<GameObject> frontChickenMeshes = new List<GameObject>();
    [HideInInspector] public List<GameObject> rearChickenMeshes = new List<GameObject>();

    private List<Material> materials = new List<Material>();

    public void SpawnPlayer(AnimalDefinition animal)
    {
        DespawnPlayer();

        playerRoot = new GameObject("PlayerRoot").transform;
        playerRoot.SetParent(transform);

        Material mat = new Material(playerMaterialTemplate);
        mat.color = animal.color;
        materials.Add(mat);

        if (animal.modelPrefab != null)
        {
            var model = Instantiate(animal.modelPrefab, playerRoot);
            model.transform.localPosition = Vector3.up * 0.75f;
        }
        else
        {
            BuildProceduralModel(animal, mat);
        }

        // Place at tile 0
        Vector3 startPos = boardBuilder.GetTileWorldPosition(0);
        Quaternion startRot = boardBuilder.GetTileRotation(0);
        playerRoot.position = startPos;
        playerRoot.rotation = startRot;

        // Place rear flock
        if (GameManager.Instance.run.flockMode && rearPlayerRoot != null)
        {
            int lastTile = config.boardSize - 1;
            rearPlayerRoot.position = boardBuilder.GetTileWorldPosition(lastTile);
            rearPlayerRoot.rotation = boardBuilder.GetTileRotation(lastTile);
        }
    }

    public void DespawnPlayer()
    {
        frontChickenMeshes.Clear();
        rearChickenMeshes.Clear();

        if (playerRoot != null) Destroy(playerRoot.gameObject);
        if (rearPlayerRoot != null) Destroy(rearPlayerRoot.gameObject);
        playerRoot = null;
        rearPlayerRoot = null;

        foreach (var mat in materials)
            if (mat != null) Destroy(mat);
        materials.Clear();
    }

    void BuildProceduralModel(AnimalDefinition animal, Material mat)
    {
        switch (animal.id)
        {
            case "quick_chick":   BuildChick(mat); break;
            case "gentle_cow":    BuildCow(mat); break;
            case "woolly_sheep":  BuildSheep(mat); break;
            case "swift_rabbit":  BuildRabbit(mat); break;
            case "lucky_duck":    BuildDuck(mat); break;
            case "chicken_flock": BuildChickenFlock(mat); break;
            default:              BuildPig(mat); break;
        }
    }

    #region Procedural Animal Builders

    void BuildPig(Material mat)
    {
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.75f,
            new Vector3(1.2f, 1.0f, 1.8f), mat);
        CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.6f, 0.8f),
            new Vector3(0.8f, 0.8f, 0.6f), mat);
        CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.5f, 1.1f),
            new Vector3(0.4f, 0.3f, 0.2f), mat);
        float[][] legs = { new[]{-0.4f, 0.6f}, new[]{0.4f, 0.6f}, new[]{-0.4f, -0.6f}, new[]{0.4f, -0.6f} };
        foreach (var l in legs)
            CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(l[0], -0.5f, l[1]),
                new Vector3(0.3f, 0.5f, 0.3f), mat);
    }

    void BuildChick(Material mat)
    {
        Material beakMat = MakeMat(new Color(1f, 0.5f, 0f));
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.6f,
            new Vector3(0.8f, 0.8f, 1.0f), mat);
        CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.6f, 0.4f),
            new Vector3(0.5f, 0.5f, 0.5f), mat);
        CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.6f, 0.7f),
            new Vector3(0.2f, 0.1f, 0.2f), beakMat);
        foreach (float x in new[]{-0.2f, 0.2f})
            CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(x, -0.4f, 0),
                new Vector3(0.1f, 0.4f, 0.1f), beakMat);
    }

    void BuildCow(Material mat)
    {
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.9f,
            new Vector3(1.5f, 1.2f, 2.2f), mat);
        CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.8f, 1.0f),
            new Vector3(1.0f, 1.0f, 0.8f), mat);
        float[][] legs = { new[]{-0.6f, 0.8f}, new[]{0.6f, 0.8f}, new[]{-0.6f, -0.8f}, new[]{0.6f, -0.8f} };
        foreach (var l in legs)
            CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(l[0], -0.6f, l[1]),
                new Vector3(0.4f, 0.6f, 0.4f), mat);
    }

    void BuildSheep(Material mat)
    {
        Material earMat = MakeMat(new Color(0.8f, 0.75f, 0.7f));
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.78f,
            new Vector3(1.2f, 1.0f, 1.7f), mat);
        var wool = CreatePrimitive(PrimitiveType.Sphere, body.transform, new Vector3(0, 0.2f, 0),
            new Vector3(1.05f * 1.5f, 0.85f * 1.5f, 1.25f * 1.5f), mat);
        var head = CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.4f, 0.95f),
            new Vector3(0.55f, 0.6f, 0.55f), mat);
        foreach (float x in new[]{-0.32f, 0.32f})
            CreatePrimitive(PrimitiveType.Cube, head.transform, new Vector3(x, 0.18f, 0),
                new Vector3(0.22f, 0.14f, 0.08f), earMat);
        float[][] legs = { new[]{-0.38f, 0.55f}, new[]{0.38f, 0.55f}, new[]{-0.38f, -0.55f}, new[]{0.38f, -0.55f} };
        foreach (var l in legs)
            CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(l[0], -0.52f, l[1]),
                new Vector3(0.22f, 0.52f, 0.22f), earMat);
    }

    void BuildRabbit(Material mat)
    {
        Material innerEarMat = MakeMat(new Color(1f, 0.72f, 0.72f));
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.62f,
            new Vector3(0.85f, 0.9f, 1.25f), mat);
        var head = CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.5f, 0.72f),
            new Vector3(0.62f, 0.62f, 0.58f), mat);
        foreach (float x in new[]{-0.16f, 0.16f})
        {
            var ear = CreatePrimitive(PrimitiveType.Cube, head.transform, new Vector3(x, 0.68f, 0.08f),
                new Vector3(0.14f, 0.72f, 0.1f), mat);
            CreatePrimitive(PrimitiveType.Cube, ear.transform, new Vector3(0, 0, 0.06f),
                new Vector3(0.07f, 0.52f, 0.06f), innerEarMat);
        }
        CreatePrimitive(PrimitiveType.Sphere, body.transform, new Vector3(0, 0.22f, -0.68f),
            Vector3.one * 0.28f, mat);
        float[][] legs = { new[]{-0.28f, 0.44f}, new[]{0.28f, 0.44f}, new[]{-0.28f, -0.38f}, new[]{0.28f, -0.38f} };
        foreach (var l in legs)
            CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(l[0], -0.44f, l[1]),
                new Vector3(0.18f, 0.38f, 0.24f), mat);
    }

    void BuildDuck(Material mat)
    {
        Material billMat = MakeMat(new Color(1f, 0.62f, 0.05f));
        var body = CreatePrimitive(PrimitiveType.Cube, playerRoot, Vector3.up * 0.65f,
            new Vector3(1.0f, 0.9f, 1.4f), mat);
        var neck = CreatePrimitive(PrimitiveType.Cylinder, body.transform, new Vector3(0, 0.55f, 0.55f),
            new Vector3(0.34f, 0.26f, 0.34f), mat);
        var head = CreatePrimitive(PrimitiveType.Sphere, neck.transform, new Vector3(0, 0.46f, 0.12f),
            Vector3.one * 0.5f, mat);
        CreatePrimitive(PrimitiveType.Cube, head.transform, new Vector3(0, -0.03f, 0.27f),
            new Vector3(0.3f, 0.08f, 0.3f), billMat);
        var tail = CreatePrimitive(PrimitiveType.Cube, body.transform, new Vector3(0, 0.26f, -0.78f),
            new Vector3(0.25f, 0.35f, 0.12f), mat);
        tail.transform.localEulerAngles = new Vector3(25.8f, 0, 0);
        foreach (float x in new[]{-0.25f, 0.25f})
        {
            var leg = CreatePrimitive(PrimitiveType.Cylinder, body.transform, new Vector3(x, -0.52f, 0),
                new Vector3(0.13f, 0.19f, 0.13f), billMat);
            CreatePrimitive(PrimitiveType.Cube, leg.transform, new Vector3(0, -0.22f, 0.1f),
                new Vector3(0.3f, 0.07f, 0.35f), billMat);
        }
    }

    void BuildChickenFlock(Material mat)
    {
        Material accentMat = MakeMat(new Color(1f, 0.5f, 0f));

        // Front group: 3 chickens
        float[][] frontOffsets = { new[]{-0.7f, 0f}, new[]{0f, 0.38f}, new[]{0.7f, 0f} };
        foreach (var o in frontOffsets)
            frontChickenMeshes.Add(BuildSmallChicken(playerRoot, o[0], o[1], mat, accentMat));

        // Rear group: 2 chickens
        rearPlayerRoot = new GameObject("RearFlockRoot").transform;
        rearPlayerRoot.SetParent(transform);
        float[][] rearOffsets = { new[]{-0.45f, 0f}, new[]{0.45f, 0f} };
        foreach (var o in rearOffsets)
            rearChickenMeshes.Add(BuildSmallChicken(rearPlayerRoot, o[0], o[1], mat, accentMat));
    }

    GameObject BuildSmallChicken(Transform parent, float offsetX, float offsetZ, Material mat, Material accentMat)
    {
        var root = new GameObject("SmallChicken");
        root.transform.SetParent(parent);
        root.transform.localPosition = new Vector3(offsetX, 0.62f, offsetZ);

        CreatePrimitive(PrimitiveType.Cube, root.transform, Vector3.zero,
            new Vector3(0.6f, 0.5f, 0.76f), mat);
        CreatePrimitive(PrimitiveType.Cube, root.transform, new Vector3(0, 0.4f, 0.3f),
            new Vector3(0.38f, 0.38f, 0.38f), mat);
        CreatePrimitive(PrimitiveType.Cube, root.transform, new Vector3(0, 0.38f, 0.5f),
            new Vector3(0.15f, 0.1f, 0.15f), accentMat);
        foreach (float x in new[]{-0.17f, 0.17f})
            CreatePrimitive(PrimitiveType.Cube, root.transform, new Vector3(x, -0.37f, 0),
                new Vector3(0.1f, 0.3f, 0.1f), accentMat);

        return root;
    }

    #endregion

    #region Movement

    public void MoveAlongPath(List<Vector3> positions, List<Quaternion> rotations,
        List<Vector3> rearPositions, List<Quaternion> rearRotations, System.Action onComplete)
    {
        StartCoroutine(MoveCoroutine(positions, rotations, rearPositions, rearRotations, onComplete));
    }

    IEnumerator MoveCoroutine(List<Vector3> positions, List<Quaternion> rotations,
        List<Vector3> rearPositions, List<Quaternion> rearRotations, System.Action onComplete)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 startPos = playerRoot.position;
            Quaternion startRot = playerRoot.rotation;
            Vector3 endPos = positions[i];
            Quaternion endRot = rotations[i];

            Vector3 rearStartPos = Vector3.zero;
            Quaternion rearStartRot = Quaternion.identity;
            bool hasRear = rearPositions != null && rearPlayerRoot != null && i < rearPositions.Count;
            if (hasRear)
            {
                rearStartPos = rearPlayerRoot.position;
                rearStartRot = rearPlayerRoot.rotation;
            }

            float elapsed = 0f;
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hopDuration);
                // Smooth ease in-out
                float smooth = t * t * (3f - 2f * t);

                playerRoot.position = Vector3.Lerp(startPos, endPos, smooth);
                playerRoot.rotation = Quaternion.Slerp(startRot, endRot, smooth);

                if (hasRear)
                {
                    rearPlayerRoot.position = Vector3.Lerp(rearStartPos, rearPositions[i], smooth);
                    rearPlayerRoot.rotation = Quaternion.Slerp(rearStartRot, rearRotations[i], smooth);
                }

                yield return null;
            }

            playerRoot.position = endPos;
            playerRoot.rotation = endRot;
            if (hasRear)
            {
                rearPlayerRoot.position = rearPositions[i];
                rearPlayerRoot.rotation = rearRotations[i];
            }
        }

        onComplete?.Invoke();
    }

    #endregion

    #region Flock helpers

    public void SacrificeRandomChicken()
    {
        var run = GameManager.Instance.run;
        var candidates = new List<string>();
        if (run.frontChickens > 0) candidates.Add("front");
        if (run.rearChickens > 0) candidates.Add("rear");
        if (candidates.Count == 0) return;

        string group = candidates[Random.Range(0, candidates.Count)];
        if (group == "front")
        {
            run.frontChickens--;
            if (frontChickenMeshes.Count > 0)
            {
                var mesh = frontChickenMeshes[frontChickenMeshes.Count - 1];
                frontChickenMeshes.RemoveAt(frontChickenMeshes.Count - 1);
                if (mesh != null) Destroy(mesh);
            }
        }
        else
        {
            run.rearChickens--;
            if (rearChickenMeshes.Count > 0)
            {
                var mesh = rearChickenMeshes[rearChickenMeshes.Count - 1];
                rearChickenMeshes.RemoveAt(rearChickenMeshes.Count - 1);
                if (mesh != null) Destroy(mesh);
            }
        }
    }

    #endregion

    #region Helpers

    GameObject CreatePrimitive(PrimitiveType type, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(type);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().material = mat;
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);
        return go;
    }

    Material MakeMat(Color c)
    {
        var mat = new Material(playerMaterialTemplate);
        mat.color = c;
        materials.Add(mat);
        return mat;
    }

    #endregion
}
