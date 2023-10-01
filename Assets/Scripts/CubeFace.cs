using System;
using UnityEngine;

public enum CubeFace
{
    Top, Bottom, Front, Back, Left, Right
}

public static class CubeFaceExtensions
{
    public static Vector3 GetNormal(this CubeFace face) => face switch
    {
        CubeFace.Top => Vector3.up,
        CubeFace.Bottom => Vector3.down,
        CubeFace.Front => Vector3.forward,
        CubeFace.Back => Vector3.back,
        CubeFace.Left => Vector3.left,
        CubeFace.Right => Vector3.right,
        _ => throw new InvalidOperationException(nameof(CubeFace))
    };
}