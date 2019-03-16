using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagilePlayer : IguagileBehaviour
    {
        public List<IguagileTracker> Trackers = new List<IguagileTracker>();
        public float Threshold;
        private float thresholdSquare;

        void Start()
        {
            UpdateThreshold(Threshold);
        }

        void Update()
        {
            if (Trackers.Count == 0)
            {
                return;
            }
            SyncTrackers();
        }

        public void UpdateThreshold(float threshold)
        {
            Threshold = threshold;
            thresholdSquare = threshold * threshold;
        }

        public void AddTracker(IguagileTracker tracker)
        {
            Trackers.Add(tracker);
        }

        public void SyncTrackers()
        {
            byte[] data = {(byte) RpcTargets.OtherClients, (byte) MessageTypes.Trackers};
            var transforms = Trackers.Where(x => x.IsMove(thresholdSquare)).Select(x => new IguagileTransform(x.transform, x.TrackerType)).ToArray();
            if (transforms.Length == 0)
            {
                return;
            }
            var serialized = TransformSerializer.Serialize(transforms);
            data = data.Concat(serialized).ToArray();
            Manager.Client.Send(data);
        }
    }
}