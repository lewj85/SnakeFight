﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class Head : MonoBehaviour
{
    public int livesRemaining;
    private float _updateInterval;
    private float _elapsed;
    //private Vector3 mapSize;

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
    [SerializeField]
    public Vector3[] positions;
    public Vector3[] rotations;

    public KeyCode kcRight;
    public KeyCode kcLeft;

    private int _effectTimer;
    private bool noneFound;
    
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
        //isPlayer = false; // for all AIs

        // TODO: add this after you make the GameController create the Player Head
        //livesRemaining = GameObject.Find("GameController1").GetComponent<GameController>().numberOfLives;
        livesRemaining = GameController.numberOfLives;
        _updateInterval = GameController.updateInterval;
        _effectTimer = 0;
        //mapSize = GameController.mapSize;
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
            // if this isn't Player1, set isPlayer = false
            if (!(this.gameObject == GameObject.Find("Player1"))) { isPlayer = false; }
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
        transform.position += transform.forward;
        // round values
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
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
        _effectTimer = 0; // reset speed boosts
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
        // if anyone runs into an Item - speed boost
        if (collision.gameObject.GetComponent<Item>())
        {
            //collision.gameObject.GetComponent<Item>().transform.position = new Vector3(UnityEngine.Random.Range(-(int)mapSize.x, (int)mapSize.x), 0.0f, UnityEngine.Random.Range(-(int)mapSize.z, (int)mapSize.z));
            GameObject.Find("GameController1").GetComponent<GameController>().timeUntilItemSpawn = 0;
            this._updateInterval = GameController.updateInterval / 2;
            _effectTimer = 150;  // lasts ~3 seconds
        }

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
        // reset _updateInterval
        if (_effectTimer < 0) { _updateInterval = GameController.updateInterval; }

        // player controls go here
        if (isPlayer)
        {
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

                // manhattan distance of objectives
                int food1Dist = Math.Abs((int)(food1Loc.x - transform.position.x)) + Math.Abs((int)(food1Loc.z - transform.position.z));
                int food2Dist = Math.Abs((int)(food2Loc.x - transform.position.x)) + Math.Abs((int)(food2Loc.z - transform.position.z));
                int item1Dist = Math.Abs((int)(item1Loc.x - transform.position.x)) + Math.Abs((int)(item1Loc.z - transform.position.z));

                if ((food1Dist <= food2Dist) && (food1Dist <= item1Dist))
                {
                    objectiveLocation = food1Loc;
                    objectiveDistance = food1Dist;
                }
                else if ((food2Dist <= food1Dist) && (food2Dist <= item1Dist))
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
                forwardLoc = new Vector3(Mathf.Round(forwardLoc.x), Mathf.Round(forwardLoc.y), Mathf.Round(forwardLoc.z));
                transform.Rotate(rotationLeft);
                Vector3 leftLoc = transform.position + transform.forward;
                leftLoc = new Vector3(Mathf.Round(leftLoc.x), Mathf.Round(leftLoc.y), Mathf.Round(leftLoc.z));
                transform.Rotate(rotationRight);
                transform.Rotate(rotationRight);
                Vector3 rightLoc = transform.position + transform.forward;
                rightLoc = new Vector3(Mathf.Round(rightLoc.x), Mathf.Round(rightLoc.y), Mathf.Round(rightLoc.z));
                transform.Rotate(rotationLeft);
                rotations = new Vector3[] { rotationForward, rotationLeft, rotationRight };
                positions = new Vector3[] { forwardLoc, leftLoc, rightLoc };

                List<Vector3> allowableRotations = new List<Vector3>();

                noneFound = false;

                // check 3 locations for distances and collisions
                for (int i = 0; i < positions.Length; i++)
                {
                    if ( !(Physics.CheckSphere(positions[i], 0.45f)) )
                    {
                        allowableRotations.Add(rotations[i]);
                    }
                    // get distance from the new location
                    int newDist = Math.Abs((int)(objectiveLocation.x - positions[i].x)) + Math.Abs((int)(objectiveLocation.z - positions[i].z));
                    // if the new location is closer than the current location
                    if (newDist <= objectiveDistance)
                    {
                        // if there is no collision or we have reached the destination, end the loop early
                        if (newDist == 0 || !(Physics.CheckSphere(positions[i], 0.45f)) )
                        {
                            transform.Rotate(rotations[i]);
                            break;
                        }
                    }

                    // if you made it to this line on the last loop, no locations were found
                    if (i == positions.Length)
                    {
                        noneFound = true;
                    }
                }

                // if no closer locations were found, the closest item must be directly behind, so turn randomly
                if (noneFound)
                {
                    Debug.Log("No closer locations found!");
                    // if there's at least 1 allowable rotation
                    if (allowableRotations.Count > 0)
                    {
                        // rotate to the first one
                        transform.Rotate(allowableRotations[0]);
                    }
                }
            }
            // TODO: remove, this is for testing only - press B to extend
            if (Input.GetKeyDown(KeyCode.B))
            {
                extend();
            }
        }

        // move every updateInterval
        if (_elapsed >= _updateInterval)
        {
            target.move();
            _elapsed = 0;
        }
    }
}

