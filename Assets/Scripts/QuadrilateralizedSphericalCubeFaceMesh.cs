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

    protected override bool CanRecurse() => configuration.MaximumDepth >= depth && configuration.MaximumDistance >= Vector3.Distance(meshParameters.RenderOrigin, transform.TransformPoint(unnormalizedCenter.normalized));

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

        return child;
    }

    private Vector2 GetNwCorner(Quadrant quadrant) => quadrant switch
    {
        Quadrant.NorthWest => nwCorner,
        Quadrant.NorthEast => nwCorner + new Vector2(sideLength / 2f, 0),
        Quadrant.SouthEast => nwCorner + new Vector2(0, sideLength / 2f),
        Quadrant.SouthWest => nwCorner + new Vector2(sideLength / 2f, sideLength / 2f),
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
        faceMesh.sideLength = 1;

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

                vertices[vertexIndex] = unitCubeCoordinate.normalized;
            }
        }

        return vertices;
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
