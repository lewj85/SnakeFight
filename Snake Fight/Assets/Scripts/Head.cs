﻿using UnityEngine;
using System;

public class Head : MonoBehaviour
{
    public int livesRemaining;
    private float updateInterval;
    private float elapsed;

    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private GameObject tail;
    public Material m;
    [SerializeField]
    public int numberOfTails;

    // Save the start location for respawning
    public Transform startingTransform;

    //Transform View;
    Vector3 rotation;
    Vector3 newXYZ;

    [SerializeField]
    protected Head target;  // the head's target will always be the last tail, but each tail's target will be the tail in FRONT of it
                            // so the target of the tail immediately behind the head will be the head!
    [SerializeField]
    public Head head;

    void Start()
    {
        // TODO: add this after you make the GameController create the Player Head
        //livesRemaining = GameObject.Find("GameController1").GetComponent<GameController>().numberOfLives;
        livesRemaining = 3;
        startingTransform = transform;
        target = this;
        head = this;

        updateInterval = GameController.updateInterval;
        numberOfTails = 0;

        if (isPlayer)
        {
            //m = Materials.Load("Player", typeof(Material)) as Material;
            GetComponent<Renderer>().material = m;
            //View = transform.Find("Camera");
        }
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
        Head tmp = target;  // save current target (last tail) temporarily to set as new tail's target
        target = Instantiate(tail, target.transform).GetComponent<Tail>();  // make a new tail at current target's location (transform)
        target.updateInterval = updateInterval;
        target.transform.parent = null;  // necessary in case parent head is moving in a different direction?
        target.head = this;  // set this as head
        target.target = tmp;  // set target to temporary last tail we saved before
        numberOfTails += 1;
    }

    public void respawn()
    {
        Debug.Log("Respawning");
        // pick a random respawn point and teleport there
        System.Random ran = new System.Random();
        Transform[] options = GameController.spawnPointList;
        Transform respawnTransform = options[ran.Next(0, options.Length)];
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
    }

    public virtual void destroyTails(Head whereToStop)
    {
        if (numberOfTails == 0) { return; }
        else
        {
            target.destroyTails(whereToStop);
        }
    }

    // OnCollisionEnter wasn't working so I went with OnTriggerEnter and used RigidBody
    public virtual void OnTriggerEnter(Collider collision)
    {
        if (isPlayer)
        {
            //Debug.Log("Player Head TriggerEnter");

            // if Player runs into a Wall
            if (collision.gameObject.GetComponent<Wall>())
            {
                Debug.Log("Player Head ran into Wall");
                GameObject.Find("Player1").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("Player1").GetComponent<Head>().livesRemaining != 0)
                {
                    respawn();
                }
                else
                {
                    LoadingScreenManager.LoadScene(0);
                }
            }
            // if Player runs into a Tail
            else if (collision.gameObject.GetComponent<Tail>())
            {
                // if Player runs into an enemy tail
                if (collision.gameObject.GetComponent<Tail>().head != this)
                {
                    Debug.Log("Player Head ran into enemy Tail");
                    GameObject.Find("Player1").GetComponent<Head>().livesRemaining -= 1;
                    if (GameObject.Find("Player1").GetComponent<Head>().livesRemaining != 0)
                    {
                        respawn();
                    }
                    else
                    {
                        LoadingScreenManager.LoadScene(0);
                    }
                }
                // if Player runs into your own tail
                else
                {
                    // the first time you create a tail, it places it where the head is, so there's a collision. ignore the first time!
                    if (numberOfTails > 1)
                    {
                        Debug.Log("Player Head ran into own Tail");
                        GameObject.Find("Player1").GetComponent<Head>().livesRemaining -= 1;
                        if (GameObject.Find("Player1").GetComponent<Head>().livesRemaining != 0)
                        {
                            respawn();
                        }
                        else
                        {
                            LoadingScreenManager.LoadScene(0);
                        }
                    }
                }
            }
            // if Player runs into a Head - NOTE: leave this as last else-if so it runs Tail code first (because Tails inherit from Head)
            else if (collision.gameObject.GetComponent<Head>())
            {
                Debug.Log("Player Head ran into enemy Head");
                GameObject.Find("Player1").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("Player1").GetComponent<Head>().livesRemaining != 0)
                {
                    respawn();
                }
                else
                {
                    LoadingScreenManager.LoadScene(0);
                }
            }
        }
        else
        {
            //Debug.Log("AI Head TriggerEnter");

            // if AI runs into a Wall
            if (collision.gameObject.GetComponent<Wall>())
            {
                Debug.Log("AI Head ran into Wall");
                GameObject.Find("AI1").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("AI1").GetComponent<Head>().livesRemaining != 0)
                {
                    respawn();
                }
                else
                {
                    // leaving commented for testing
                    //LoadingScreenManager.LoadScene(0);
                }
            }
            else if (collision.gameObject.GetComponent<Tail>())
            {
                // if AI runs into an enemy tail
                if (collision.gameObject.GetComponent<Tail>().head != this)
                {
                    Debug.Log("AI Head ran into enemy Tail");
                    GameObject.Find("AI1").GetComponent<Head>().livesRemaining -= 1;
                    if (GameObject.Find("AI1").GetComponent<Head>().livesRemaining != 0)
                    {
                        respawn();
                    }
                    else
                    {
                        // leaving commented for testing
                        //LoadingScreenManager.LoadScene(0);
                    }
                }
                // if AI runs into its own tail
                else
                {
                    // the first time AI creates a tail, it places it where the head is, so there's a collision. ignore the first time!
                    if (numberOfTails > 1)
                    {
                        Debug.Log("AI head ran into own Tail");
                        GameObject.Find("AI1").GetComponent<Head>().livesRemaining -= 1;
                        if (GameObject.Find("AI1").GetComponent<Head>().livesRemaining != 0)
                        {
                            respawn();
                        }
                        else
                        {
                            // leaving commented for testing
                            //LoadingScreenManager.LoadScene(0);
                        }
                    }
                }
            }
            // if AI runs into a Head - NOTE: leave this as last else-if so it runs Tail code first (because Tails inherit from Head)
            else if (collision.gameObject.GetComponent<Head>())
            {
                Debug.Log("AI Head ran into enemy Head");
                GameObject.Find("AI1").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("AI1").GetComponent<Head>().livesRemaining != 0)
                {
                    respawn();
                }
                else
                {
                    // leaving commented for testing
                    //LoadingScreenManager.LoadScene(0);
                }
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
            // 3D movement
            //else if (Input.GetButtonDown("Up")) rotation = new Vector3(-90, 0, 0);
            //else if (Input.GetButtonDown("Down")) rotation = new Vector3(90, 0, 0);

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
            // TODO: remove, this is for testing only - press B to extend
            if (Input.GetKeyDown(KeyCode.B))
            {
                extend();
            }

            // compare food locations to head location - order them by distance
            Vector3 foodLoc = FindObjectOfType<Food>().transform.position;
            //Debug.Log(foodLoc);

            // loop
                // pop next closest food
                // create path array (adjust X first, then Z)
                // check for obstruction along the way
            // adjust position based on shortest path
        }
    }
}

