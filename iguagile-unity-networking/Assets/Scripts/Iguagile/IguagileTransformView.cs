using UnityEngine;

namespace Iguagile
{
    public class IguagileTransformView : IguagileBehaviour
    {
        private IguagileTransform _syncTransform;

        internal IguagileTransform SyncTransform
        {
            get
            {
                if (_syncTransform == null)
                {
                    _syncTransform = new IguagileTransform(Vector3.zero, Quaternion.identity, ObjectId);
                }

                return _syncTransform;
            }

            set
            {
                _syncTransform = value;
            }
        }

        private bool _update;

        void Awake()
        {
            View.TransformView = this;
        }

        void Update()
        {
            if (View.IsMine)
            {
                SyncTransform.SetNext(transform);
            }
            else
            {
                if (_update)
                {
                    transform.position = SyncTransform.NextPosition;
                    transform.rotation = SyncTransform.NextRotation;
                    _update = false;
                }
            }
        }

        internal void UpdateTransform(IguagileTransform iguagileTransform)
        {
            SyncTransform = iguagileTransform;
            _update = true;
        }
    }
}