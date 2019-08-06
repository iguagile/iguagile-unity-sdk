namespace Iguagile
{
    public class IguagileTransformView : IguagileBehaviour
    {
        internal IguagileTransform NextTransform { get; set; }

        private bool _update;

        void Update()
        {
            if (View.IsMine)
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