﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class Head : MonoBehaviour
{
    public int livesRemaining;
    private float _updateInterval;
    private float _elapsed;
    private Vector3 mapSize;

    [SerializeField]
    public bool isPlayer;
    [SerializeField]
    private GameObject tail;
    public Material m;
    [SerializeField]
    public int numberOfTails;
    [SerializeField]
    public Vector3 objectiveLocation;
    [SerializeField]
    public int objectiveDistance;

    public KeyCode kcRight;
    public KeyCode kcLeft;

    private int _effectTimer;

    private int nR;
    private int numRightTurns
    {
        get { return nR; }
        set
        {
            if (value > 2) nR = 2; // max value is 2
            else if (value < 0) nR = 0; // min value is 0
            else nR = value;
        }
    }

    private int nL;
    private int numLeftTurns
    {
        get { return nL; }
        set
        {
            if (value > 2) nL = 2; // max value is 2
            else if (value < 0) nL = 0; // min value is 0
            else nL = value;
        }
    }

    public Transform startingTransform;  // save the start location for respawning

    //Transform View;
    Vector3 rotationLeft;
    Vector3 rotationRight;
    Vector3 rotationForward;
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
        livesRemaining = GameController.numberOfLives;
        _updateInterval = GameController.updateInterval;
        mapSize = GameController.mapSize;
        startingTransform = transform;
        target = this;
        head = this;
        numberOfTails = 0;
        extend();  // make a tail
        numRightTurns = 1;
        numLeftTurns = 1;

        rotationLeft = new Vector3(0, -90, 0);
        rotationRight = new Vector3(0, 90, 0);
        rotationForward = new Vector3(0, 0, 0); //.zero;

        if (PlayerPrefs.GetInt("numPlayers") == 1)
        {
            if (this.gameObject == GameObject.Find("Player2"))
            {
                isPlayer = false;
            }
        }

        if (isPlayer)
        {
            //m = Materials.Load("Player", typeof(Material)) as Material;
            GetComponent<Renderer>().material = m;
            //View = transform.Find("Camera");
        }
    }

    public virtual void move()
    {
        //transform.Rotate(rotation); 
        // round values
        //Vector3 tmp = transform.forward;
        //transform.position += new Vector3(Mathf.Round(tmp.x), Mathf.Round(tmp.y), Mathf.Round(tmp.z));
        transform.position += transform.forward;
        //rotation = Vector3.zero;  // reset immediately after moving
        numRightTurns = 1;
        numLeftTurns = 1;
    }

    public void extend()
    {
        Head tmp = target;  // save current target (last tail) temporarily to set as new tail's target
        target = Instantiate(tail, target.transform).GetComponent<Tail>();  // make a new tail at current target's location (transform)
        target._updateInterval = _updateInterval;
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
        Vector3 respawnLocation;
        Transform respawnTransform;
        do
        {
            respawnTransform = options[ran.Next(0, options.Length)];
            respawnLocation = respawnTransform.position;
        
        // TODO: fix this so if all 4 spawnpoints are occupied, it won't crash the game - consider randomizing the spawn points 
        // another solution would be to prevent players from entering spawn zones - give them a 'safe' starting area but force them out of it
        } while (Physics.CheckSphere(respawnLocation, 0.4f));  // loops while there's a collision at the chosen respawn location
        
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
        extend(); // make a tail
    }

    public void destroyTails(Head whereToStop)  // not virtual anymore
    {
        if (numberOfTails == 0) { return; }
        else
        {
            Head tmp;
            while (target != whereToStop)
            {
                Debug.Log("Destroying a tail");
                tmp = target.target;
                // Destroy(target.GetComponent<MeshRenderer>());  // hides it
                Destroy(target.gameObject);  // destroys it
                target = tmp;
                numberOfTails -= 1;
            }   
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
                    // destroy own tails
                    destroyTails(this);
                    respawn();
                }
                else
                {
                    LoadingScreenManager.LoadScene(0);
                }
            }

            // if Player runes into an Item - speed boost
            if (collision.gameObject.GetComponent<Item>())
            {
                collision.gameObject.GetComponent<Item>().transform.position = new Vector3(UnityEngine.Random.Range(-(int)mapSize.x, (int)mapSize.x), 0.0f, UnityEngine.Random.Range(-(int)mapSize.z, (int)mapSize.z));
                this._updateInterval = GameController.updateInterval / 2;
                _effectTimer = 150;  // lasts ~3 seconds
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
                        // destroy own tails
                        destroyTails(this);
                        // destroy the enemy tails from collision tail to the end
                        collision.gameObject.GetComponent<Tail>().head.destroyTails(collision.gameObject.GetComponent<Tail>().target);
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
                            // destroy own tails
                            destroyTails(this);
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
                    // destroy own tails
                    destroyTails(this);
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
                GameObject.Find("Player2").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("Player2").GetComponent<Head>().livesRemaining != 0)
                {
                    // destroy own tails
                    destroyTails(this);
                    respawn();
                }
                else
                {
                    LoadingScreenManager.LoadScene(0);
                }
            }
            else if (collision.gameObject.GetComponent<Tail>())
            {
                // if AI runs into an enemy tail
                if (collision.gameObject.GetComponent<Tail>().head != this)
                {
                    Debug.Log("AI Head ran into enemy Tail");
                    GameObject.Find("Player2").GetComponent<Head>().livesRemaining -= 1;
                    if (GameObject.Find("Player2").GetComponent<Head>().livesRemaining != 0)
                    {
                        // destroy own tails
                        destroyTails(this);
                        // destroy the enemy tails from collision tail to the end
                        collision.gameObject.GetComponent<Tail>().head.destroyTails(collision.gameObject.GetComponent<Tail>().target);
                        respawn();
                    }
                    else
                    {
                        LoadingScreenManager.LoadScene(0);
                    }
                }
                // if AI runs into its own tail
                else
                {
                    // the first time AI creates a tail, it places it where the head is, so there's a collision. ignore the first time!
                    if (numberOfTails > 1)
                    {
                        Debug.Log("AI head ran into own Tail");
                        GameObject.Find("Player2").GetComponent<Head>().livesRemaining -= 1;
                        if (GameObject.Find("Player2").GetComponent<Head>().livesRemaining != 0)
                        {
                            // destroy own tails
                            destroyTails(this);
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
                GameObject.Find("Player2").GetComponent<Head>().livesRemaining -= 1;
                if (GameObject.Find("Player2").GetComponent<Head>().livesRemaining != 0)
                {
                    // destroy own tails
                    destroyTails(this);
                    respawn();
                }
                else
                {
                    // leaving commented out for testing
                    //LoadingScreenManager.LoadScene(0);
                }
            }
        }
    }

    void Update()
    {
        // update time
        _elapsed += Time.deltaTime;
        _effectTimer -= 1;
        if (_effectTimer < 0) { _updateInterval = GameController.updateInterval; }

        // player controls go here
        if (isPlayer)
        {
            // move every updateInterval
            if (_elapsed >= _updateInterval)
            {
                target.move();
                _elapsed = 0;
            }

            // ROTATION CONTROLS
            // can rotate once every update, but no more than once in either direction
            if (Input.GetKeyDown(kcRight))
            {
                if (numRightTurns > 0)
                {
                    //rotation = new Vector3(0, 90, 0);
                    transform.Rotate(rotationRight);
                    //rotation = Vector3.zero;  // reset immediately after rotating
                    numRightTurns -= 1;
                    numLeftTurns += 1;
                }
            }
            else if (Input.GetKeyDown(kcLeft))
            {
                if (numLeftTurns > 0)
                {
                    //rotation = new Vector3(0, -90, 0);
                    transform.Rotate(rotationLeft);
                    //rotation = Vector3.zero;  // reset immediately after rotating
                    numRightTurns += 1;
                    numLeftTurns -= 1;
                }
            }

            // TODO: remove, this is for testing only
            if (Input.GetKeyDown(KeyCode.Space))
            {
                extend();
            }
        }
        // AI pathing goes here
        else
        {
            if (_elapsed >= _updateInterval)
            {
                // compare food locations to head location - choose closest - check for collisions - rotate as needed
                Vector3 food1Loc = GameObject.Find("GameController1").GetComponent<GameController>().food1Location;
                Vector3 food2Loc = GameObject.Find("GameController1").GetComponent<GameController>().food2Location;
                Vector3 item1Loc = GameObject.Find("GameController1").GetComponent<GameController>().item1Location;

                // manhattan distance of food1
                int food1Dist = Math.Abs((int)(food1Loc.x - transform.position.x)) + Math.Abs((int)(food1Loc.z - transform.position.z));
                int food2Dist = Math.Abs((int)(food2Loc.x - transform.position.x)) + Math.Abs((int)(food2Loc.z - transform.position.z));
                int item1Dist = Math.Abs((int)(item1Loc.x - transform.position.x)) + Math.Abs((int)(item1Loc.z - transform.position.z));

                if ((food1Dist >= food2Dist) && (food1Dist >= item1Dist))
                {
                    objectiveLocation = food1Loc;
                    objectiveDistance = food1Dist;
                }
                else if ((food2Dist >= food1Dist) && (food2Dist >= item1Dist))
                {
                    objectiveLocation = food2Loc;
                    objectiveDistance = food2Dist;
                }
                else // if ((item1Distance >= food1Distance) && (item1Distance >= food2Distance))
                {
                    objectiveLocation = item1Loc;
                    objectiveDistance = item1Dist;
                }
                // TODO: add enemy heads as possible objectives here

                //create list of 3 possible locations
                Vector3 forwardLoc = transform.position + transform.forward;
                transform.Rotate(rotationLeft);
                Vector3 leftLoc = transform.position + transform.forward;
                transform.Rotate(rotationRight);
                transform.Rotate(rotationRight);
                Vector3 rightLoc = transform.position + transform.forward;
                transform.Rotate(rotationLeft);
                Vector3[] rotations = { rotationForward, rotationLeft, rotationRight };
                Vector3[] positions = { forwardLoc, leftLoc, rightLoc };

                for (int i = 0; i < positions.Length; ++i)
                {
                    int newDist = Math.Abs((int)(objectiveLocation.x - positions[i].x)) + Math.Abs((int)(objectiveLocation.z - positions[i].z));
                    if (newDist <= objectiveDistance)
                    {
                        if (newDist == 0 || !(Physics.CheckSphere(positions[i], 0.45f)) )
                        {
                            transform.Rotate(rotations[i]);
                            break;
                        }
                        else
                        {
                            Debug.Log("Hit something. Trying new position.");
                        }
                    }
                }

                // move
                target.move();
                _elapsed = 0;
            }
            // TODO: remove, this is for testing only - press B to extend
            if (Input.GetKeyDown(KeyCode.B))
            {
                extend();
            }
        }
    }
}

