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

        private static float ThresholdPositionSquare = 1e-6f;
        private static float ThresholdRotation = 1f;

        static IguagileObjectSynchronizer()
        {
            _timer = new Timer(20);
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

        public static void SetPositionThreshold(float threshold)
        {
            ThresholdPositionSquare = threshold * threshold;
        }

        public static void SetRotationThreshold(float threshold)
        {
            ThresholdRotation = threshold;
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