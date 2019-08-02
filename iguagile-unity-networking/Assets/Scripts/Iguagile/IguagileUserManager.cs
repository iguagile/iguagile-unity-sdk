using System.Collections.Generic;

namespace Iguagile
{
    public class IguagileUserManager
    {
        private static Dictionary<int, IguagileUser> _users = new Dictionary<int, IguagileUser>();

        private static int _userId;
        private static bool _isHost;
        
        internal static void AddUser(int userId)
        {
            _users[userId] = new IguagileUser(userId);
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
            _isHost = true;
        }

        internal static void Register(int id)
        {
            _userId = id;
        }

        internal static void Clear()
        {
            _users.Clear();
        }
    }
}
