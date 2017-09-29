using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    //public float inputDelay = 0.2f; // give a slight delay so you don't move constantly if you hold a key
    public float forwardVel = 12;
    //public float rotateVel = 100;

    Quaternion targetRotation;
    Rigidbody rBody;
    float forwardInput, turnInput;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("The character needs a Rigidbody.");

        forwardInput = turnInput = 0;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        GetInput();
        Turn();
    }

    void FixedUpdate()
    {
        Run();
    }

    void Run()
    {
        // move
        rBody.velocity = transform.forward * forwardInput * forwardVel;
    }

    void Turn()
    {
        if (Input.GetKey(KeyCode.D))
            targetRotation *= Quaternion.Euler(0, 90, 0);

        if (Input.GetKey(KeyCode.A))
            targetRotation *= Quaternion.Euler(0, -90, 0);
        
        transform.rotation = targetRotation;
    }
}
