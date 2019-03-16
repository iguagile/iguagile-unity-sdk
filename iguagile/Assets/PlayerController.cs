using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-0.03f, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(0.03f, 0, 0);
        }
    }
}
