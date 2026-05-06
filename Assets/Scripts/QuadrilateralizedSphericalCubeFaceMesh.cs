using System;
using UnityEngine;

public class QuadrilateralizedSphericalCubeFaceMesh : QuadrilateralizedMesh<QuadrilateralizedSphericalCubeFaceMesh, QuadrilateralizedSphericalCubeFaceMesh.MeshData>
{
    private QuadrilateralizedSphericalCubeMesh.Parameters meshParameters;
    private QuadTreeConfiguration configuration;
    private MeshWorker meshWorker;
    private Vector3 unnormalizedCenter;
    private Vector3 normal;
    private Vector3 horizontalAxis;
    private Vector3 verticalAxis;
    private Vector2 nwCorner;
    private float sideLength;
    private int depth;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public override void ToggleVisibility(bool visible)
    {
        meshRenderer.enabled = visible;
    }

    protected override bool CanRecurse() => configuration.MaximumDepth > depth && (configuration.MaximumDistance * sideLength) >= Vector3.Distance(meshParameters.RenderOrigin, transform.TransformPoint(unnormalizedCenter.normalized));

    protected override QuadrilateralizedSphericalCubeFaceMesh CreateChild(Quadrant quadrant)
    {
        QuadrilateralizedSphericalCubeFaceMesh child = new GameObject($"{quadrant}").AddComponent<QuadrilateralizedSphericalCubeFaceMesh>();

        child.meshParameters = meshParameters;
        child.configuration = configuration;
        child.meshWorker = meshWorker;
        child.normal = normal;
        child.horizontalAxis = horizontalAxis;
        child.verticalAxis = verticalAxis;
        child.depth = depth + 1;
        child.sideLength = sideLength / 2f;

        child.nwCorner = GetNwCorner(quadrant);
        child.unnormalizedCenter = normal + (child.nwCorner.x + child.sideLength / 2f) * horizontalAxis + (child.nwCorner.y + child.sideLength / 2f) * verticalAxis;

        return child;
    }

    private Vector2 GetNwCorner(Quadrant quadrant) => quadrant switch
    {
        Quadrant.NorthWest => nwCorner,
        Quadrant.NorthEast => nwCorner + new Vector2(sideLength / 2f, 0f),
        Quadrant.SouthWest => nwCorner + new Vector2(0f, sideLength / 2f),
        Quadrant.SouthEast => nwCorner + new Vector2(sideLength / 2f, sideLength / 2f),
        _ => throw new ArgumentOutOfRangeException(nameof(Quadrant)),
    };

    public static QuadrilateralizedSphericalCubeFaceMesh CreateInstance(QuadrilateralizedSphericalCubeMesh.Parameters meshParameters, QuadTreeConfiguration configuration, CubeFace face, MeshWorker meshWorker)
    {
        QuadrilateralizedSphericalCubeFaceMesh faceMesh = new GameObject($"{face}").AddComponent<QuadrilateralizedSphericalCubeFaceMesh>();

        faceMesh.meshParameters = meshParameters;
        faceMesh.configuration = configuration;
        faceMesh.meshWorker = meshWorker;
        faceMesh.normal = face.GetNormal();
        faceMesh.horizontalAxis = new Vector3(faceMesh.normal.y, faceMesh.normal.z, faceMesh.normal.x);
        faceMesh.verticalAxis = Vector3.Cross(faceMesh.normal, faceMesh.horizontalAxis);
        faceMesh.nwCorner = new Vector2(-1, -1);
        faceMesh.sideLength = 2;
        faceMesh.unnormalizedCenter = faceMesh.normal;

        return faceMesh;
    }

    protected override MeshData CreateMeshData()
    {
        int resolution = configuration.MeshResolution;

        return new MeshData(CreateVertices(resolution), CreateTriangleVertexIndices(resolution));
    }

    private int[] CreateTriangleVertexIndices(int resolution)
    {
        var triangleVertexIndices = new int[(resolution - 1) * (resolution - 1) * 6];
        var triangleVertexIndicesIndex = 0;

        for (var row = 0; row < resolution - 1; ++row)
        {
            for (var column = 0; column < resolution - 1; ++column)
            {
                int vertexIndex = column + row * resolution;
                
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex;
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex + resolution + 1;
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex + resolution;
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex;
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex + 1;
                triangleVertexIndices[triangleVertexIndicesIndex++] = vertexIndex + 1 + resolution;
            }
        }

        return triangleVertexIndices;
    }

    private Vector3[] CreateVertices(int resolution)
    {
        var vertices = new Vector3[resolution * resolution];

        for (var row = 0; row < resolution; ++row)
        {
            for (var column = 0; column < resolution; ++column)
            {
                int vertexIndex = column + row * resolution;
                float x = column / (float)(resolution - 1);
                float y = row / (float)(resolution - 1);
                Vector3 unitCubeCoordinate = normal + (sideLength * x + nwCorner.x) * horizontalAxis + (sideLength * y + nwCorner.y) * verticalAxis;
                Vector3 dir = unitCubeCoordinate.normalized;
                float noiseVal = EvaluateNoise(dir);
                float elevation = Mathf.Lerp(configuration.MinElevation, configuration.MaxElevation, noiseVal);

                vertices[vertexIndex] = dir * elevation;
            }
        }

        return vertices;
    }

    private float EvaluateNoise(Vector3 point)
    {
        float noise = 0f;
        float amplitude = 1f;
        float frequency = configuration.NoiseScale;
        float maxValue = 0f;

        for (int i = 0; i < configuration.NoiseOctaves; i++)
        {
            noise += SimpleValueNoise3D(point * frequency) * amplitude;
            maxValue += amplitude;

            amplitude *= 0.5f;
            frequency *= 2.0f;
        }

        return noise / maxValue;
    }

    private static float SimpleValueNoise3D(Vector3 p)
    {
        int xi = Mathf.FloorToInt(p.x);
        int yi = Mathf.FloorToInt(p.y);
        int zi = Mathf.FloorToInt(p.z);

        float xf = p.x - xi;
        float yf = p.y - yi;
        float zf = p.z - zi;

        float u = xf * xf * (3f - 2f * xf);
        float v = yf * yf * (3f - 2f * yf);
        float w = zf * zf * (3f - 2f * zf);

        float x0 = Lerp(Hash(xi, yi, zi), Hash(xi + 1, yi, zi), u);
        float x1 = Lerp(Hash(xi, yi + 1, zi), Hash(xi + 1, yi + 1, zi), u);
        float x2 = Lerp(Hash(xi, yi, zi + 1), Hash(xi + 1, yi, zi + 1), u);
        float x3 = Lerp(Hash(xi, yi + 1, zi + 1), Hash(xi + 1, yi + 1, zi + 1), u);

        float y0 = Lerp(x0, x1, v);
        float y1 = Lerp(x2, x3, v);

        return Lerp(y0, y1, w);
    }

    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static float Hash(int x, int y, int z)
    {
        uint h = (uint)(x * 73856093 ^ y * 19349663 ^ z * 83492791);
        h = (h ^ (h >> 16)) * 0x85ebca6b;
        h = (h ^ (h >> 13)) * 0xc2b2ae35;
        h ^= h >> 16;
        return (h & 0xFFFFFF) / (float)0xFFFFFF;
    }

    protected override bool TryCreateMesh(MeshData data)
    {
        var mesh = new Mesh
        {
            vertices = data.Vertices,
            triangles = data.TriangleVertexIndices
        };

        mesh.RecalculateNormals();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        return true;
    }

    protected override MeshWorker GetMeshWorker() => meshWorker;

    protected override void DestroySelf()
    {
        if (meshRenderer is not null) Destroy(meshRenderer);
        if (meshFilter is not null) Destroy(meshFilter);

        meshRenderer = null;
        meshFilter = null;

        Destroy(gameObject);
    }

    public readonly struct MeshData
    {
        public MeshData(Vector3[] vertices, int[] triangleVertexIndices)
        {
            Vertices = vertices;
            TriangleVertexIndices = triangleVertexIndices;
        }

        public Vector3[] Vertices { get; }
        public int[] TriangleVertexIndices { get; }
    }
}
