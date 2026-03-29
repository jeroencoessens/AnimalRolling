using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Follow-camera that animates along the track behind the player.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Config")]
    public BoardBuilder boardBuilder;
    public PlayerController playerController;

    [Header("Camera Settings")]
    public Vector3 cameraOffset = new Vector3(0, 10, -20);
    public float hopDuration = 0.15f;

    private Transform cameraAnchor;
    private Camera mainCamera;

    public void Init()
    {
        // Unparent the camera before destroying the old anchor so Camera.main survives
        if (mainCamera != null && cameraAnchor != null && mainCamera.transform.parent == cameraAnchor)
            mainCamera.transform.SetParent(null);

        if (cameraAnchor != null) Destroy(cameraAnchor.gameObject);

        cameraAnchor = new GameObject("CameraAnchor").transform;
        cameraAnchor.position = boardBuilder.GetTileWorldPosition(0);
        cameraAnchor.rotation = boardBuilder.GetTileRotation(0);

        mainCamera = Camera.main;
        mainCamera.transform.SetParent(cameraAnchor);
        mainCamera.transform.localPosition = cameraOffset;
        mainCamera.transform.LookAt(cameraAnchor);
    }

    public void AnimateAlongPath(List<Vector3> positions, List<Quaternion> rotations)
    {
        StartCoroutine(AnimateCoroutine(positions, rotations));
    }

    IEnumerator AnimateCoroutine(List<Vector3> positions, List<Quaternion> rotations)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 startPos = cameraAnchor.position;
            Quaternion startRot = cameraAnchor.rotation;

            float elapsed = 0f;
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hopDuration);
                float smooth = t * t * (3f - 2f * t);

                cameraAnchor.position = Vector3.Lerp(startPos, positions[i], smooth);
                cameraAnchor.rotation = Quaternion.Slerp(startRot, rotations[i], smooth);
                yield return null;
            }

            cameraAnchor.position = positions[i];
            cameraAnchor.rotation = rotations[i];
        }
    }

    void OnDestroy()
    {
        // Unparent camera on cleanup so it survives scene transitions
        if (mainCamera != null && mainCamera.transform.parent == cameraAnchor)
            mainCamera.transform.SetParent(null);
    }
}
