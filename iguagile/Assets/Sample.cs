using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Iguagile;

public class Sample : IguagileBehaviour
{
    public GameObject Prefab;
    private Queue<Trackers> rpcQueue = new Queue<Trackers>();
    private bool instantiated;

    void Start()
    {
        AddRpc("InstantiateObject");
    }

    void Update()
    {
        if (IsAlive && !instantiated)
        {
            Rpc("InstantiateObject", RpcTargets.OtherClientsBuffered, new byte[] { (byte)TrackerTypes.Main });
            instantiated = true;
        }

        while (rpcQueue.Count > 0)
        {
            var trackers = rpcQueue.Dequeue();
            var playerId = trackers.Id;
            var iguagileTrackers = new List<IguagileTracker>();
            foreach (var trackerType in trackers.trackerTypes)
            {
                var obj = Instantiate(Prefab);
                var tracker = obj.AddComponent<IguagileTracker>();
                tracker.TrackerType = (TrackerTypes)trackerType;
                iguagileTrackers.Add(tracker);
            }

            AppendPlayer(playerId, iguagileTrackers.ToArray());
        }
    }

    private void InstantiateObject(string sender, byte[] trackerTypes)
    {
        var trackers = new Trackers { Id = sender, trackerTypes = trackerTypes };
        rpcQueue.Enqueue(trackers);
    }

    private class Trackers
    {
        public string Id { get; set; }
        public byte[] trackerTypes { get; set; }
    }
}
