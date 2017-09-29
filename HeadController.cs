using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    public float forwardVel = 12;

    void Update()
    {
        transform.position += transform.forward * forwardVel * Time.deltaTime;

        if (Input.GetButtonDown("Right")) transform.Rotate(new Vector3(0, 90, 0));
        else if (Input.GetButtonDown("Left")) transform.Rotate(new Vector3(0, -90, 0));
        else if (Input.GetButtonDown("Up")) transform.Rotate(new Vector3(-90, 0, 0));
        else if (Input.GetButtonDown("Down")) transform.Rotate(new Vector3(90, 0, 0));
    }

}

