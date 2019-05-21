using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessagePack;
using UnityEngine;

namespace Iguagile
{
    public class IguagileManager
    {
        private static Dictionary<string, Dictionary<IguagileTransformTypes, IguagileView>> users = new Dictionary<string, Dictionary<IguagileTransformTypes, IguagileView>>();

        private static Dictionary<string, IguagileBehaviour> behaviours = new Dictionary<string, IguagileBehaviour>();

        public static void AddRpc(IguagileBehaviour behaviour, string methodName)
        {
            behaviours[methodName] = behaviour;
        }

        public static void RemoveRpc(string methodName)
        {
            behaviours.Remove(methodName);
        }

        public static void RemoveRpc(IguagileBehaviour iguagileBehaviour)
        {
            var removeList = new List<string>();
            foreach (var behaviour in behaviours)
            {
                if (ReferenceEquals(behaviour.Value, iguagileBehaviour))
                {
                    removeList.Add(behaviour.Key);
                }
            }

            foreach (var method in removeList)
            {
                behaviours.Remove(method);
            }
        }

        public static void AddUser(string userId)
        {
            users[userId] = new Dictionary<IguagileTransformTypes, IguagileView>();
        }

        public static void RemoveUser(string userId)
        {
            foreach (var view in users[userId])
            {
                view.Value.DestroyView();
            }

            users.Remove(userId);
        }

        public static void AddSyncObject(string userId, IguagileView view, IguagileTransformTypes transformType)
        {
            users[userId][transformType] = view;
        }

        public static void UpdateTransform(string userId, byte[] data)
        {
            if (!users.ContainsKey(userId))
            {
                return;
            }

            var views = users[userId];
            var transforms = IguagileTransformSerializer.Deserialize(data);
            foreach (var iguagileTransform in transforms)
            {
                if (views.ContainsKey(iguagileTransform.TransformType))
                {
                    views[iguagileTransform.TransformType].UpdateTransform(iguagileTransform);
                }
            }
        }

        public static void InvokeRpc(string userId, byte[] data)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data);
            var methodName = (string) objects[0];
            if (behaviours.ContainsKey(methodName))
            {
                var args = new object[] {userId};
                if (objects.Length > 1)
                {
                    args = args.Concat(objects.Skip(1)).ToArray();
                }

                var behaviour = behaviours[methodName];
                var type = behaviour.GetType();
                var flag = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;
                var method = type.GetMethod(methodName, flag);
                method?.Invoke(behaviour, args);
            }
        }
    }
}