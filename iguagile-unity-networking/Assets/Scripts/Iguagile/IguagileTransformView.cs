using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace Iguagile
{
    [RequireComponent(typeof(IguagileView))]
    public class IguagileTransformView : IguagileBehaviour
    {
        private static Dictionary<IguagileTransformTypes, IguagileTransformView> syncObjects =
            new Dictionary<IguagileTransformTypes, IguagileTransformView>();

        private static Timer timer;

        private Queue<IguagileTransform> transformQueue = new Queue<IguagileTransform>();
        private IguagileView view;

        public IguagileTransformTypes TransformType;
        public IguagileTransform Transform { get; private set; }

        private bool update;

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<IguagileView>();
            view.TransformView = this;
            Transform = new IguagileTransform(transform, view.ObjectId);
        }

        // Update is called once per frame
        void Update()
        {
            if (view.IsMine)
            {
                Transform.Position = transform.position;
                Transform.Rotation = transform.rotation;
            }
            else
            {
                while (transformQueue.Count > 0)
                {
                    var iguagileTransform = transformQueue.Dequeue();
                    transform.position = iguagileTransform.Position;
                    transform.rotation = iguagileTransform.Rotation;
                }
            }
        }

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            transformQueue.Enqueue(iguagileTransform);
        }

        public static void AddSycnObject(IguagileTransformView syncObject)
        {
            syncObjects[syncObject.TransformType] = syncObject;
        }

        public static void SyncStart(double interval)
        {
            timer = new Timer(interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public static void SyncStop()
        {
            timer.Stop();
            timer.Dispose();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var transforms = syncObjects.Select(x => x.Value.Transform).ToArray();
            var data = IguagileTransformSerializer.Serialize(transforms);
            IguagileNetwork.Send(data);
        }
    }
}