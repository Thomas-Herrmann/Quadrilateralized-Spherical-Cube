using System;

public record QuadTreeConfiguration : ThreadSafeConfiguration
{
    private int maximumDepth;
    private float maximumDistance;
    private int meshResolution;

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

    protected override void ReleaseResources()
    {
    }

    public event EventHandler<int> MeshResolutionChanged;
}
