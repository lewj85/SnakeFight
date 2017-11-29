﻿using UnityEngine;
using System;

public class Head : MonoBehaviour
{
    [SerializeField]  // puts the variable below in the Inspector
    private float updateInterval;
    private float elapsed;
    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private GameObject tail;
    public Material m;

    //Transform View;
    Vector3 rotation;
    Vector3 newXYZ;

    protected Head target;
    public Head head;

    void Start()
    {
        if (isPlayer)
        {
            //m = Materials.Load("Player", typeof(Material)) as Material;
            //GetComponent<Renderer>().material = m;
            //View = transform.Find("Camera");
        }
        target = this;
        head = this;
        
    }

    public virtual void move()
    {
        transform.Rotate(rotation);
        // round values
        //Vector3 tmp = transform.forward;
        //transform.position += new Vector3(Mathf.Round(tmp.x), Mathf.Round(tmp.y), Mathf.Round(tmp.z));
        transform.position += transform.forward;
        rotation = Vector3.zero;  // reset immediately after moving
    }

    public void extend()
    {
        Head tmp = target;
        target = Instantiate(tail, target.transform).GetComponent<Tail>();
        target.updateInterval = updateInterval;
        target.transform.parent = null;
        target.head = this;
        target.target = tmp;
    }


    // OnCollisionEnter wasn't working so I went with OnTriggerEnter and used RigidBody
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Player Head TriggerEnter");
        if (collision.gameObject.GetComponent<Tail>())
        {
            // if you ran into an enemy tail
            if (collision.gameObject.GetComponent<Tail>().head != this)
            {
                // do stuff
                Debug.Log("Ran into enemy Tail");
            }
            // if you ran into your own tail
            else
            {
                Debug.Log("Ran into own Tail");
            }
        }
    }

    void Update()
    {
        // update time
        elapsed += Time.deltaTime;

        // player controls go here
        if (isPlayer)
        {
            if (elapsed >= updateInterval)
            {
                target.move();
                elapsed = 0;
            }
        
            if (Input.GetButtonDown("Right")) rotation = new Vector3(0, 90, 0);
            else if (Input.GetButtonDown("Left")) rotation = new Vector3(0, -90, 0);
            // else if (Input.GetButtonDown("Up")) rotation = new Vector3(-90, 0, 0);
            // else if (Input.GetButtonDown("Down")) rotation = new Vector3(90, 0, 0);

            // TODO: remove, this is for testing only
            if (Input.GetKeyDown(KeyCode.Space))
            {
                extend();
            }
        }
        // AI pathing goes here
        else
        {
            if (elapsed >= updateInterval)
            {
                target.move();
                elapsed = 0;
            }

            // compare food locations to head location - order them by distance
            Vector3 foodLoc = GameObject.FindObjectOfType<Food>().transform.position;
            // Debug.Log(foodLoc);

            // loop
                // pop next closest food
                // create path array (adjust X first, then Z)
                // check for obstruction along the way
            // adjust position based on shortest path
        }
    }
}

