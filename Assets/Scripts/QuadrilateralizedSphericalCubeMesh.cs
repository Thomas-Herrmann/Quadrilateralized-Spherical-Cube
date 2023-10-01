using System;
using UnityEngine;

public class QuadrilateralizedSphericalCubeMesh : MonoBehaviour
{
    private QuadrilateralizedSphericalCubeFaceMesh[] faceMeshes;
    private QuadTreeConfiguration configuration;
    private Parameters meshParameters;
    private new Camera camera;

    public static QuadrilateralizedSphericalCubeMesh CreateInstance(QuadTreeConfiguration configuration)
    {
        QuadrilateralizedSphericalCubeMesh cubeMesh = new GameObject(nameof(QuadrilateralizedSphericalCubeMesh)).AddComponent<QuadrilateralizedSphericalCubeMesh>();

        cubeMesh.configuration = configuration;

        return cubeMesh;
    }

    // Unity Message
    void Start()
    {
        var faces = Enum.GetValues(typeof(CubeFace));
        var index = 0;

        camera = GetComponent<Camera>();
        meshParameters = new Parameters();
        faceMeshes = new QuadrilateralizedSphericalCubeFaceMesh[faces.Length];

        foreach (CubeFace face in faces) faceMeshes[index++] = QuadrilateralizedSphericalCubeFaceMesh.CreateInstance(meshParameters, configuration, face);
    }

    // Unity Message
    void Update()
    {
        meshParameters.RenderOrigin = camera.transform.position;
    }

    public record Parameters
    {
        public Vector3 RenderOrigin { get; set; }
    }
}
