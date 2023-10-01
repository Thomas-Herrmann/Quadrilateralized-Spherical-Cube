using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class QuadrilateralizedMesh<TSuper, TData> : MonoBehaviour where TSuper : QuadrilateralizedMesh<TSuper, TData>
{
    private State state;
    private TSuper nwChild;
    private TSuper neChild;
    private TSuper seChild;
    private TSuper swChild;
    private TData data;
    private MeshWorker meshWorker;

    public bool IsUpdating { get; private set; }

    // Unity message
    private void Start()
    {
        meshWorker = GetMeshWorker();
    }

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
        State.Initial => TrySchedule(),
        _ => false,
    };

    private bool TrySchedule()
    {
        meshWorker.ScheduleMeshData(this as TSuper);

        state = State.Generating;

        return true;
    }

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
    protected abstract MeshWorker GetMeshWorker();
    protected abstract bool CanRecurse();
    protected abstract TSuper CreateChild(Quadrant quadrant);
    public abstract void ToggleVisibility(bool visible);

    private enum State
    {
        Split, Splitting, Active, Waiting, Ready, Generating, Initial
    }

    public class MeshWorker : IDisposable
    {
        private Queue<TSuper> meshQueue;
        private Thread workingThread;
        private CancellationTokenSource cancellationTokenSource;
        private bool isDisposed;

        public MeshWorker()
        {
            meshQueue = new Queue<TSuper>();
            cancellationTokenSource = new CancellationTokenSource();
            workingThread = new Thread(GenerateMeshDataContinuously);

            workingThread.Start();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;

            cancellationTokenSource.Cancel();
            workingThread.Join();
            cancellationTokenSource.Dispose();
        }

        public void ScheduleMeshData(TSuper node) => meshQueue.Enqueue(node);

        private void GenerateMeshDataContinuously()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                if (meshQueue.TryDequeue(out TSuper node))
                {
                    node.data = node.CreateMeshData();
                    node.state = State.Ready;

                    continue;
                }

                Thread.Sleep(100);
            }
        }
    }
}