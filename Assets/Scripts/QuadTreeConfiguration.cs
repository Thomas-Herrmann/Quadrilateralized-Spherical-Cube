using System;

public record QuadTreeConfiguration : ThreadSafeConfiguration
{
    private int maximumDepth;
    private float maximumDistance;
    private int meshResolution;

    private float minElevation;
    private float maxElevation;
    private float noiseScale;
    private int noiseOctaves;
    private float radius;

    public int MaximumDepth 
    {
        get => GetThreadSafe(ref maximumDepth); 
        set => SetThreadSafe(ref maximumDepth, value);
    }
    public float MaximumDistance 
    {
        get => GetThreadSafe(ref maximumDistance);
        set => SetThreadSafe(ref maximumDistance, value);
    }

    public int MeshResolution
    {
        get => GetThreadSafe(ref meshResolution);
        set => SetThreadSafe(ref meshResolution, value, (a, b) => a == b, MeshResolutionChanged);
    }

    public float MinElevation
    {
        get => GetThreadSafe(ref minElevation);
        set => SetThreadSafe(ref minElevation, value);
    }

    public float MaxElevation
    {
        get => GetThreadSafe(ref maxElevation);
        set => SetThreadSafe(ref maxElevation, value);
    }

    public float NoiseScale
    {
        get => GetThreadSafe(ref noiseScale);
        set => SetThreadSafe(ref noiseScale, value);
    }

    public int NoiseOctaves
    {
        get => GetThreadSafe(ref noiseOctaves);
        set => SetThreadSafe(ref noiseOctaves, value);
    }

    public float Radius
    {
        get => GetThreadSafe(ref radius);
        set => SetThreadSafe(ref radius, value);
    }

    protected override void ReleaseResources()
    {
    }

    public event EventHandler<int> MeshResolutionChanged;
}
