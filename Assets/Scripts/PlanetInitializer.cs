using UnityEngine;

public sealed class PlanetInitializer : MonoBehaviour
{
    [Header("Planet Configuration")]
    [SerializeField] private int maximumDepth = 5;
    [SerializeField] private float maximumDistance = 5f;
    [SerializeField] private int meshResolution = 16;

    [Header("Terrain Generation")]
    [SerializeField] private float minElevation = 1.0f;
    [SerializeField] private float maxElevation = 1.2f;
    [SerializeField] private float noiseScale = 2f;
    [SerializeField] private int noiseOctaves = 4;

    public QuadTreeConfiguration configuration = new();
    private QuadrilateralizedSphericalCubeMesh planetMesh;

    private int lastMaximumDepth;
    private float lastMaximumDistance;
    private int lastMeshResolution;
    private float lastMinElevation;
    private float lastMaxElevation;
    private float lastNoiseScale;
    private int lastNoiseOctaves;

    // Unity Message
    private void Start()
    {
        configuration.MaximumDepth = maximumDepth;
        configuration.MaximumDistance = maximumDistance;
        configuration.MeshResolution = meshResolution;
        configuration.MinElevation = minElevation;
        configuration.MaxElevation = maxElevation;
        configuration.NoiseScale = noiseScale;
        configuration.NoiseOctaves = noiseOctaves;

        lastMaximumDepth = maximumDepth;
        lastMaximumDistance = maximumDistance;
        lastMeshResolution = meshResolution;
        lastMinElevation = minElevation;
        lastMaxElevation = maxElevation;
        lastNoiseScale = noiseScale;
        lastNoiseOctaves = noiseOctaves;

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

        if (lastMinElevation != minElevation)
        {
            configuration.MinElevation = minElevation;
            lastMinElevation = minElevation;
        }

        if (lastMaxElevation != maxElevation)
        {
            configuration.MaxElevation = maxElevation;
            lastMaxElevation = maxElevation;
        }

        if (lastNoiseScale != noiseScale)
        {
            configuration.NoiseScale = noiseScale;
            lastNoiseScale = noiseScale;
        }

        if (lastNoiseOctaves != noiseOctaves)
        {
            configuration.NoiseOctaves = noiseOctaves;
            lastNoiseOctaves = noiseOctaves;
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