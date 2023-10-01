using System;
using UnityEngine;

public abstract class QuadrilateralizedMesh<TSuper, TData> : MonoBehaviour where TSuper : QuadrilateralizedMesh<TSuper, TData>
{
    private State state;
    private TSuper nwChild;
    private TSuper neChild;
    private TSuper seChild;
    private TSuper swChild;
    private TData data;

    public bool IsUpdating { get; private set; }

    // Unity Message
    private void Update()
    {
        bool changesWereMade = TryUpdate();

        if (!changesWereMade && state is State.Active) 
            IsUpdating = false;
        else if (changesWereMade) 
            IsUpdating = state is State.Split;
    }

    private bool TryUpdate() => state switch
    {
        State.Splitting => TryFinishSplitting(),
        State.Ready => TryCreateMesh(),
        State.Active => TryStartSplitting(),
        State.Split => TryMerge(),
        _ => false,
    };

    private bool TryMerge()
    {
        if (CanRecurse()) return false;

        if (nwChild.state is not State.Active || neChild.state is not State.Active || seChild.state is not State.Active || swChild.state is not State.Active) return false;

        Destroy(nwChild);
        Destroy(neChild);
        Destroy(seChild);
        Destroy(swChild);
        ToggleVisibility(true);

        state = State.Active;

        return true;
    }

    private bool TryStartSplitting()
    {
        if (!CanRecurse()) return false;

        nwChild = CreateChildInternal(Quadrant.NorthWest);
        neChild = CreateChildInternal(Quadrant.NorthEast);
        seChild = CreateChildInternal(Quadrant.SouthEast);
        swChild = CreateChildInternal(Quadrant.SouthWest);
        state = State.Splitting;

        return true;
    }

    private TSuper CreateChildInternal(Quadrant quadrant)
    {
        TSuper child = CreateChild(quadrant);

        child.state = State.Initial;

        return child;
    }

    private bool TryCreateMesh()
    {
        if (!TryCreateMesh(data)) return false;

        ToggleVisibility(false);

        state = State.Waiting;

        return true;
    }

    private bool TryFinishSplitting()
    {
        if (nwChild.state is not State.Waiting || neChild.state is not State.Waiting || seChild.state is not State.Waiting || swChild.state is not State.Waiting) return false;

        nwChild.ActivateMesh();
        neChild.ActivateMesh();
        seChild.ActivateMesh();
        swChild.ActivateMesh();
        ToggleVisibility(false);

        return true;
    }

    public void ActivateMesh()
    {
        if (state is not State.Waiting) return;

        ToggleVisibility(true);

        state = State.Active;
    }

    /// <summary>
    /// </summary>
    /// <remarks>Will be called from a non-Unity thread!</remarks>
    /// <returns></returns>
    protected abstract TData CreateMeshData();

    /// <summary>
    /// </summary>
    /// <param name="data"></param>
    /// <remarks>Will be called from the Unity thread!</remarks>
    /// <returns></returns>
    protected abstract bool TryCreateMesh(TData data);

    protected abstract bool CanRecurse();
    protected abstract TSuper CreateChild(Quadrant quadrant);
    public abstract void ToggleVisibility(bool visible);

    private enum State
    {
        Split, Splitting, Active, Waiting, Ready, Generating, Initial
    }
}
