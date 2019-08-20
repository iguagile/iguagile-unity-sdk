using System.Collections.Generic;

namespace Iguagile
{
    public class IguagileUserManager
    {
        private static Dictionary<int, IguagileUser> _users = new Dictionary<int, IguagileUser>();

        public static int UserId { get; private set; }
        public static bool IsHost { get; private set; }
        
        internal static void AddUser(int userId)
        {
            var user = new IguagileUser(userId);
            _users[userId] = user;
            IguagileEventManager.InvokeNewConnection(user);
        }

        internal static void RemoveUser(int userId)
        {
            if (_users.ContainsKey(userId))
            {
                _users.Remove(userId);
            }
        }

        internal static void MigrateHost()
        {
            IsHost = true;
            IguagileEventManager.InvokeReceivedHostAuthority();
        }

        internal static void Register(int id)
        {
            UserId = id;
        }

        internal static void Clear()
        {
            _users.Clear();
        }
    }
}
