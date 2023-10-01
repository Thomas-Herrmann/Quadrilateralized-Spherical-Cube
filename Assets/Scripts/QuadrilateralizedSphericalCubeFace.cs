using System;

public class QuadrilateralizedSphericalCubeFace : QuadrilateralizedMesh<QuadrilateralizedSphericalCubeFace, QuadrilateralizedSphericalCubeFace.MeshData>
{
    public override void ToggleVisibility(bool visible)
    {
        throw new NotImplementedException();
    }

    protected override bool CanRecurse()
    {
        throw new NotImplementedException();
    }

    protected override QuadrilateralizedSphericalCubeFace CreateChild(Quadrant quadrant)
    {
        throw new NotImplementedException();
    }

    protected override MeshData CreateMeshData()
    {
        throw new NotImplementedException();
    }

    protected override bool TryCreateMesh(MeshData data)
    {
        throw new NotImplementedException();
    }

    public class MeshData
    {

    }
}
