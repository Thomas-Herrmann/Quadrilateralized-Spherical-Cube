using System;
using UnityEngine;

public class QuadrilateralizedSphericalCubeMesh : MonoBehaviour
{
    private QuadrilateralizedSphericalCubeFaceMesh[] faceMeshes;
    private QuadTreeConfiguration configuration;
    private Parameters meshParameters;
    private new Camera camera;
    private QuadrilateralizedSphericalCubeFaceMesh.MeshWorker meshWorker;

    public static QuadrilateralizedSphericalCubeMesh CreateInstance(QuadTreeConfiguration configuration)
    {
        QuadrilateralizedSphericalCubeMesh cubeMesh = new GameObject(nameof(QuadrilateralizedSphericalCubeMesh)).AddComponent<QuadrilateralizedSphericalCubeMesh>();

        cubeMesh.configuration = configuration;

        return cubeMesh;
    }

    // Unity Message
    private void Start()
    {
        var faces = Enum.GetValues(typeof(CubeFace));
        var index = 0;

        camera = GetComponent<Camera>();

        if (camera == null) camera = Camera.main;

        meshParameters = new Parameters();
        faceMeshes = new QuadrilateralizedSphericalCubeFaceMesh[faces.Length];
        meshWorker = new QuadrilateralizedSphericalCubeFaceMesh.MeshWorker();

        foreach (CubeFace face in faces)
        {
            var faceMesh = QuadrilateralizedSphericalCubeFaceMesh.CreateInstance(meshParameters, configuration, face, meshWorker);

            faceMesh.transform.SetParent(transform);

            faceMeshes[index++] = faceMesh;
        }
    }

    // Unity Message
    private void Update()
    {
        meshParameters.RenderOrigin = camera.transform.position;

        foreach (QuadrilateralizedSphericalCubeFaceMesh faceMesh in faceMeshes) faceMesh.ActivateMesh();
    }

    // Unity Message
    private void OnDestroy()
    {
        meshWorker?.Dispose();

        if (faceMeshes is null) return;

        foreach (QuadrilateralizedSphericalCubeFaceMesh faceMesh in faceMeshes) Destroy(faceMesh);
    }

    public record Parameters
    {
        public Vector3 RenderOrigin { get; set; }
    }
}
