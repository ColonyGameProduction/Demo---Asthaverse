using UnityEngine;

public class LODEnemy : MonoBehaviour
{
    public float lod0Distance = 5f; // Jarak dalam meter untuk LOD 0
    public float lod1Distance = 10f; // Jarak dalam meter untuk LOD 1
    public float lod2Distance = 20f; // Jarak dalam meter untuk LOD 2

    private LODGroup lodGroup;
    private Camera mainCamera;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        mainCamera = Camera.main;

        if (lodGroup != null && mainCamera != null)
        {
            UpdateLODThresholds();
        }
    }

    void UpdateLODThresholds()
    {
        float objectSize = lodGroup.size;

        if (objectSize <= 0)
        {
            Debug.LogError("Object size is invalid. Ensure the LODGroup has a valid size.");
            return;
        }

        LOD[] lods = lodGroup.GetLODs();

        float lod0ScreenSize = ConvertMetersToScreenSize(lod0Distance, objectSize);
        float lod1ScreenSize = ConvertMetersToScreenSize(lod1Distance, objectSize);
        float lod2ScreenSize = ConvertMetersToScreenSize(lod2Distance, objectSize);

        if (lod0ScreenSize <= lod1ScreenSize) lod0ScreenSize = lod1ScreenSize + 0.05f;
        if (lod1ScreenSize <= lod2ScreenSize) lod1ScreenSize = lod2ScreenSize + 0.05f;

        lods[0].screenRelativeTransitionHeight = lod0ScreenSize;
        lods[1].screenRelativeTransitionHeight = lod1ScreenSize;
        lods[2].screenRelativeTransitionHeight = lod2ScreenSize;

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        Debug.Log($"LOD Set: LOD0={lod0ScreenSize}, LOD1={lod1ScreenSize}, LOD2={lod2ScreenSize} (Converted from meters)");
    }

    float ConvertMetersToScreenSize(float distance, float objectSize)
    {
        if (mainCamera == null) return 0.5f; // Default jika tidak ada kamera

        float screenHeight = 2.0f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        return objectSize / screenHeight;
    }
}
