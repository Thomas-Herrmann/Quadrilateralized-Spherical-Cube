using System;
using UnityEngine;

public class QuadrilateralizedSphericalCubeFaceMesh : QuadrilateralizedMesh<QuadrilateralizedSphericalCubeFaceMesh, QuadrilateralizedSphericalCubeFaceMesh.MeshData>
{
    private QuadrilateralizedSphericalCubeMesh.Parameters meshParameters;
    private QuadTreeConfiguration configuration;
    private Vector3 unnormalizedCenter;
    private int depth;

    public override void ToggleVisibility(bool visible)
    {
        throw new NotImplementedException();
    }

    protected override bool CanRecurse() => configuration.MaximumDepth >= depth && configuration.MaximumDistance >= Vector3.Distance(meshParameters.RenderOrigin, unnormalizedCenter.normalized);

    protected override QuadrilateralizedSphericalCubeFaceMesh CreateChild(Quadrant quadrant)
    {
        throw new NotImplementedException();
    }

    public static QuadrilateralizedSphericalCubeFaceMesh CreateInstance(QuadrilateralizedSphericalCubeMesh.Parameters meshParameters, QuadTreeConfiguration configuration, CubeFace face)
    {
        QuadrilateralizedSphericalCubeFaceMesh faceMesh = new GameObject(nameof(QuadrilateralizedSphericalCubeFaceMesh)).AddComponent<QuadrilateralizedSphericalCubeFaceMesh>();

        faceMesh.meshParameters = meshParameters;
        faceMesh.configuration = configuration;

        return faceMesh;
    }

    protected override MeshData CreateMeshData()
    {
        throw new NotImplementedException();
    }

    protected override bool TryCreateMesh(MeshData data)
    {
        throw new NotImplementedException();
    }

    protected override MeshWorker GetMeshWorker()
    {
        throw new NotImplementedException();
    }

    public class MeshData
    {

    }
}
