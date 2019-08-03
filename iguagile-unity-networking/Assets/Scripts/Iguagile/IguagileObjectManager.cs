using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileObjectManager
    {
        private static Dictionary<int, IguagileView> _syncObjects = new Dictionary<int, IguagileView>();
        private static Dictionary<int, IguagileView> _mySyncObjects = new Dictionary<int, IguagileView>();

        internal static IguagileTransform[] SyncTransforms { get; private set; } = new IguagileTransform[0];
        
        internal static void Instantiate(int userId, int objectId, string name)
        {
            var prefab = Resources.Load(name, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                return;
            }

            var obj = Object.Instantiate(prefab);
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

            Object.Destroy(view.gameObject);
        }

        internal static void TransferObjectControlAuthority(int objectId)
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
            SyncTransforms = _mySyncObjects.Select(x => x.Value.TransformView.Transform).ToArray();
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
