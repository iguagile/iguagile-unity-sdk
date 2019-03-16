using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace Iguagile
{
    public class IguagileManager : MonoBehaviour
    {
        public IguagileClient Client = new IguagileClient();
        public string ServerUrl;
        public bool ConnectOnStart;
        public bool IsAlive => Client.IsAlive;

        private Dictionary<int, Dictionary<TrackerTypes, IguagileTracker>> players = new Dictionary<int, Dictionary<TrackerTypes, IguagileTracker>>();
        private Dictionary<string, IguagileBehaviour> rpcMethods = new Dictionary<string, IguagileBehaviour>();
        private Queue<int> deleteQueue = new Queue<int>();

        void Start()
        {
            Client.Manager = this;
            if (ConnectOnStart)
            {
                Connect();
            }
        }

        void Update()
        {
            if (deleteQueue.Count == 0)
            {
                return;
            }

            var playerId = deleteQueue.Dequeue();
            var trackers = players[playerId];
            foreach (var tracker in trackers)
            {
                Destroy(tracker.Value.gameObject);
            }

            players.Remove(playerId);
        }

        public void Connect()
        {
            Connect(ServerUrl);
        }

        public void Connect(string url)
        {
            Client.Connect(url);
        }

        public void DisConnect()
        {
            Client.Disconnect();
        }

        public void AppendTracker(int playerId, params IguagileTracker[] trackers)
        {
            var player = players[playerId];
            foreach (var tracker in trackers)
            {
                player[tracker.TrackerType] = tracker;
            }
        }

        public void AppendPlayer(int playerId, params IguagileTracker[] trackers)
        {
            var player = new Dictionary<TrackerTypes, IguagileTracker>();
            foreach (var tracker in trackers)
            {
                player[tracker.TrackerType] = tracker;
            }
            players[playerId] = player;
        }

        public void RemovePlayer(int playerId)
        {
            players.Remove(playerId);
        }

        public void AddRpc(string methodName, IguagileBehaviour iguagileBehaviour)
        {
            rpcMethods[methodName] = iguagileBehaviour;
        }

        public void RemoveRpc(string methodName)
        {
            rpcMethods.Remove(methodName);
        }

        public void Rpc(string methodName, RpcTargets target, params object[] args)
        {
            object[] objects = new object[] { methodName };
            objects = objects.Concat(args).ToArray();
            var serialized = LZ4MessagePackSerializer.Serialize(objects);
            byte[] data = { (byte)target, (byte)MessageTypes.Rpc };
            data = data.Concat(serialized).ToArray();
            Client.Send(data);
        }

        internal void ReceivedTracker(int playerId, byte[] data)
        {
            if (!players.ContainsKey(playerId))
            {
                return;
            }
            var trackers = TransformSerializer.Deserialize(data);
            var player = players[playerId];
            foreach (var tracker in trackers)
            {
                player[tracker.TrackerType].UpdateTransform(tracker);
            }
        }

        internal void ReceivedRpcAsync(int playerId, byte[] data)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data);
            var methodName = (string)objects[0];
            if (!rpcMethods.ContainsKey(methodName))
            {
                return;
            }
            var args = new object[] { playerId };
            args = args.Concat(objects.Skip(1).ToArray()).ToArray();
            var monoBehaviour = rpcMethods[methodName];
            monoBehaviour.OnRpc(methodName, args);
        }

        internal void ReceivedOpenMessage(int playerId)
        {
        }

        internal void ReceivedCloseMessage(int playerId)
        {
            deleteQueue.Enqueue(playerId);
        }
    }
}