using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var data = new byte[] { (byte)RpcTargets.Server, (byte)MessageTypes.Instantiate };
            var transformByte = LZ4MessagePackSerializer.Serialize(new object[] { name, 0f, 0f, 0f, 0f, 0f, 0f, 0f });
            data = data.Concat(idByte).Concat(new byte[] { (byte)ObjectLifetime.OwnerExist }).Concat(transformByte).ToArray();
            IguagileNetwork.Send(data);
        }

        public static void Instantiate(string name, Vector3 position, Quaternion rotation, ObjectLifetime lifetime)
        {
            var id = _generator.Generate() | IguagileUserManager.UserId;
            var idByte = BitConverter.GetBytes(id);
            var data = new byte[] { (byte)RpcTargets.Server, (byte)MessageTypes.Instantiate };
            var transformByte = LZ4MessagePackSerializer.Serialize(new object[]
                {name, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w});
            data = data.Concat(idByte).Concat(new byte[] { (byte)lifetime }).Concat(transformByte).ToArray();
            IguagileNetwork.Send(data);
        }

        public static void Instantiate(string name, ObjectLifetime lifetime)
        {
            var id = _generator.Generate() | IguagileUserManager.UserId;
            var idByte = BitConverter.GetBytes(id);
            var data = new byte[] { (byte)RpcTargets.Server, (byte)MessageTypes.Instantiate };
            var transformByte = LZ4MessagePackSerializer.Serialize(new object[] { name, 0f, 0f, 0f, 0f, 0f, 0f, 0f });
            data = data.Concat(idByte).Concat(new byte[] { (byte)lifetime }).Concat(transformByte).ToArray();
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

        public static void Destroy(IguagileBehaviour gameObject)
        {
            var idByte = BitConverter.GetBytes(gameObject.ObjectId);
            var data = new byte[] { (byte)RpcTargets.Server, (byte)MessageTypes.Destroy };
            data = data.Concat(idByte).ToArray();
            IguagileNetwork.Send(data);
        }

        internal static void Instantiate(int userId, int objectId, string name, Vector3 position, Quaternion rotation)
        {
            var prefab = Resources.Load(name, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                return;
            }

            var obj = GameObject.Instantiate(prefab, position, rotation);
            var view = obj.GetComponent<IguagileView>();
            if (view == null)
            {
                view = obj.AddComponent<IguagileView>();
            }
            view.ObjectId = objectId;
            _syncObjects.Add(objectId, view);
            view.TransformView = view.GetComponent<IguagileTransformView>();

            if (userId == IguagileUserManager.UserId)
            {
                view.IsMine = true;
                if (view.TransformView != null)
                {
                    _mySyncObjects.Add(objectId, view);
                    view.TransformView.SyncTransform = new IguagileTransform(view.transform, objectId);
                    UpdateSyncObjects();
                }
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

        private static void UpdateSyncObjects()
        {
            SyncTransforms = _mySyncObjects.Select(x => x.Value.TransformView?.SyncTransform).ToArray();
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
                GameObject.Destroy(syncObject.Value.gameObject);
            }

            _syncObjects.Clear();
        }
    }
}
