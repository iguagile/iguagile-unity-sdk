namespace Iguagile
{
    public class IguagileView : IguagileBehaviour
    {
        public IguagileTransformView TransformView;
        public bool IsMine { get; internal set; }
        public int ObjectId { get; internal set; }

        private bool disconnected;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (disconnected)
            {
                Destroy(gameObject);
            }
        }

        public void DestroyView()
        {
            disconnected = true;
        }

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            TransformView.UpdateTransform(iguagileTransform);
        }
    }
}