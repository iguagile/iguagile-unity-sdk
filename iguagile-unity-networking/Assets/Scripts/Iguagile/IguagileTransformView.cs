namespace Iguagile
{
    public class IguagileTransformView : IguagileBehaviour
    {
        internal IguagileTransform SyncTransform;

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

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            SyncTransform = iguagileTransform;
            _update = true;
        }
    }
}