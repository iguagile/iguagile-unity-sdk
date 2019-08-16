using System;
using System.Linq;
using System.Timers;

namespace Iguagile
{
    public class IguagileObjectSynchronizer
    {
        private static Timer _timer;

        public static double SyncInterval => _timer.Interval;
        public static bool EnableSyncObjects => _timer.Enabled;

        public static bool EnableThreshold = true;
        public static float ThresholdPositionSquare = 0.001f * 0.001f;
        public static float ThresholdRotation = 1f;

        static IguagileObjectSynchronizer()
        {
            _timer = new Timer(30);
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
            var transforms = IguagileObjectManager.SyncTransforms;
            if (EnableThreshold)
            {
                transforms = transforms.Where(x => x.IsMove(ThresholdPositionSquare, ThresholdRotation)).ToArray();
            }

            if (transforms.Length == 0)
            {
                return;
            }

            var data = IguagileTransformSerializer.Serialize(transforms);
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