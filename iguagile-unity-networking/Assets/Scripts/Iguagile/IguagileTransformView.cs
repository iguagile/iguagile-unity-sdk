using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace Iguagile
{
    [RequireComponent(typeof(IguagileView))]
    public class IguagileTransformView : IguagileBehaviour
    {
        internal IguagileTransform NextTransform { get; private set; }

        private IguagileView _view;
        private bool _update;
        
        void Start()
        {
            _view = GetComponent<IguagileView>();
            _view.TransformView = this;
            NextTransform = new IguagileTransform(transform, _view.ObjectId);
        }

        void Update()
        {
            if (_view.IsMine)
            {
                NextTransform.Position = transform.position;
                NextTransform.Rotation = transform.rotation;
            }
            else
            {
                if (_update)
                {
                    transform.position = NextTransform.Position;
                    transform.rotation = NextTransform.Rotation;
                    _update = false;
                }
            }
        }

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            NextTransform = iguagileTransform;
            _update = true;
        }
    }
}