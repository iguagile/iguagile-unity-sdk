using System.Collections;
using System.Collections.Generic;
using Iguagile;
using UnityEngine;

public class IguagileView : IguagileBehaviour
{
    public IguagileTransformView TransformView;
    public bool IsMine;

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
