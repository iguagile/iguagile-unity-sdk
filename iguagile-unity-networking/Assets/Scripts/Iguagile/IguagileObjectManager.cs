using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Iguagile
{
    public class IguagileObjectManager
    {
        private static ObjectIdGenerator _generator = new ObjectIdGenerator(short.MaxValue);
        private static Dictionary<int, IguagileView> _syncObjects = new Dictionary<int, IguagileView>();
        private static Dictionary<int, IguagileView> _mySyncObjects = new Dictionary<int, IguagileView>();

        internal static IguagileTransform[] SyncTransforms { get; private set; } = new IguagileTransform[0];

        /// <summary>
        /// Instantiate an object using Resources.Load over the network.
        /// </summary>
        /// <param name="name">resource name</param>
        public static void Instantiate(string name)
        {
            var id = _generator.Generate() | IguagileUserManager.UserId;
            var idByte = BitConverter.GetBytes(id);
            var nameByte = Encoding.UTF8.GetBytes(name);
            var data = new byte[] {(byte) RpcTargets.Server, (byte) MessageTypes.Instantiate};
            data = data.Concat(idByte).Concat(nameByte).ToArray();
            IguagileNetwork.Send(data);
        }
        
        public static void TransferObjectControlAuthority(int objectId)
        {
            if (_mySyncObjects.ContainsKey(objectId))
            {
                _mySyncObjects[objectId].IsMine = false;
                _mySyncObjects.Remove(objectId);
                UpdateSyncObjects();
            }
        }

        internal static void Instantiate(int userId, int objectId, string name)
        {
            var prefab = Resources.Load(name, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                return;
            }

            var obj = GameObject.Instantiate(prefab);
            var view = obj.GetComponent<IguagileView>();
            _syncObjects.Add(objectId, view);

            if (userId == IguagileUserManager.UserId)
            {
                _mySyncObjects.Add(objectId, view);
                UpdateSyncObjects();
            }
        }

        internal static void Destroy(int objectId)
        {
            if (!_syncObjects.ContainsKey(objectId))
            {
                return;
            }

            var view = _syncObjects[objectId];
            _syncObjects.Remove(objectId);

            if (_mySyncObjects.ContainsKey(objectId))
            {
                _mySyncObjects.Remove(objectId);
            }

            GameObject.Destroy(view.gameObject);
        }

        internal static void ReceiveObjectControlAuthority(int objectId)
        {
            if (!_syncObjects.ContainsKey(objectId))
            {
                return;
            }

            var view = _syncObjects[objectId];
            view.IsMine = true;
            _mySyncObjects.Add(objectId, view);
            UpdateSyncObjects();
        }

        internal static void UpdateSyncObjects()
        {
            SyncTransforms = _mySyncObjects.Select(x => x.Value.TransformView.NextTransform).ToArray();
        }

        internal static IguagileView GetView(int objectId)
        {
            if (!_syncObjects.ContainsKey(objectId))
            {
                return null;
            }

            return _syncObjects[objectId];
        }
        
        internal static void Clear()
        {
            _mySyncObjects.Clear();
            foreach (var syncObject in _syncObjects)
            {
                Destroy(syncObject.Key);
            }

            _syncObjects.Clear();
        }
    }
}
