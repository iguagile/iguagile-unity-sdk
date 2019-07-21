using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Iguagile
{
    public class IguagileManager
    {
        private static Dictionary<int, Dictionary<IguagileTransformTypes, IguagileView>> users =
            new Dictionary<int, Dictionary<IguagileTransformTypes, IguagileView>>();

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

        public static void AddUser(int userId)
        {
            users[userId] = new Dictionary<IguagileTransformTypes, IguagileView>();
        }

        public static void RemoveUser(int userId)
        {
            foreach (var view in users[userId])
            {
                view.Value.DestroyView();
            }

            users.Remove(userId);
        }

        public static void AddSyncObject(int userId, IguagileView view, IguagileTransformTypes transformType)
        {
            users[userId][transformType] = view;
        }

        // TODO Implement Instantiate method
        public static void Instantiate(string name)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateTransform(int userId, byte[] data)
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

        internal static void InvokeRpc(int userId, byte[] data)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data);
            var methodName = (string) objects[0];
            if (behaviours.ContainsKey(methodName))
            {
                var args = new object[0];
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

        // TODO Implement Instantiate method
        internal static void Instantiate(int userId, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}