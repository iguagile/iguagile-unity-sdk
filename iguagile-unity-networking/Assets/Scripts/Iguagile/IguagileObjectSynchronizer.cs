using System.Timers;

namespace Iguagile
{
    public class IguagileObjectSynchronizer
    {
        private static Timer _timer;

        public static double SyncInterval => _timer.Interval;
        public static bool EnableSyncObjects => _timer.Enabled;

        static IguagileObjectSynchronizer()
        {
            _timer = new Timer(100);
            _timer.Elapsed += TimerElapsed;
        }

        public static void SyncStart()
        {
            _timer.Start();
        }

        public static void SyncStop()
        {
            _timer.Stop();
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var data = IguagileTransformSerializer.Serialize(IguagileObjectManager.SyncTransforms);
            IguagileNetwork.Send(data);
        }

        internal static void UpdateTransform(IguagileTransform[] transforms)
        {
            foreach (var transform in transforms)
            {
                var view = IguagileObjectManager.GetView(transform.ObjectId);
                view?.TransformView.UpdateTransform(transform);
            }
        }
    }
}