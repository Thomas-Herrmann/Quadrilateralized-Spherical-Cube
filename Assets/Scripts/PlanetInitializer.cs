using UnityEngine;

public sealed class PlanetInitializer : MonoBehaviour
{
    [Header("Planet Configuration")]
    [SerializeField] private int maximumDepth = 5;
    [SerializeField] private float maximumDistance = 5f;
    [SerializeField] private int meshResolution = 16;

    public QuadTreeConfiguration configuration = new();
    private QuadrilateralizedSphericalCubeMesh planetMesh;

    private int lastMaximumDepth;
    private float lastMaximumDistance;
    private int lastMeshResolution;

    // Unity Message
    private void Start()
    {
        configuration.MaximumDepth = maximumDepth;
        configuration.MaximumDistance = maximumDistance;
        configuration.MeshResolution = meshResolution;

        lastMaximumDepth = maximumDepth;
        lastMaximumDistance = maximumDistance;
        lastMeshResolution = meshResolution;

        planetMesh = QuadrilateralizedSphericalCubeMesh.CreateInstance(configuration);
        planetMesh.transform.SetParent(transform, false);
    }

    private void Update()
    {
        if (lastMaximumDepth != maximumDepth)
        {
            configuration.MaximumDepth = maximumDepth;
            lastMaximumDepth = maximumDepth;
        }

        if (lastMaximumDistance != maximumDistance)
        {
            configuration.MaximumDistance = maximumDistance;
            lastMaximumDistance = maximumDistance;
        }

        if (lastMeshResolution != meshResolution)
        {
            configuration.MeshResolution = meshResolution;
            lastMeshResolution = meshResolution;
        }
    }

    private void OnDestroy()
    {
        if (planetMesh != null)
        {
            Destroy(planetMesh.gameObject);
        }
    }
}