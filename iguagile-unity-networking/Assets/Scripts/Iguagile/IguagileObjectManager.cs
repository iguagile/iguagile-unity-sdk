using System.Collections.Generic;
using UnityEngine;

namespace Iguagile
{
    public class IguagileObjectManager
    {
        private static Dictionary<int, IguagileView> _syncObjects = new Dictionary<int, IguagileView>();
        private static Dictionary<int, IguagileView> _mySyncObjects = new Dictionary<int, IguagileView>();
        
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
        }

        internal static void Destroy(int userId, int objectId)
        {
            if (!_syncObjects.ContainsKey(objectId))
            {
                return;
            }

            var view = _syncObjects[objectId];
            _syncObjects.Remove(objectId);
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
            _syncObjects.Clear();
            _mySyncObjects.Clear();
        }
    }
}
