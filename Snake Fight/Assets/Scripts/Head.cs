using UnityEngine;
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
    [SerializeField]
    public int numberOfTails;

    //Transform View;
    Vector3 rotation;
    Vector3 newXYZ;

    protected Head target;
    public Head head;

    void Start()
    {
        updateInterval = GameObject.Find("GameController1").GetComponent<GameController>().updateInterval;
        numberOfTails = 0;

        if (isPlayer)
        {
            //m = Materials.Load("Player", typeof(Material)) as Material;
            GetComponent<Renderer>().material = m;
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
        numberOfTails += 1;
    }


    // OnCollisionEnter wasn't working so I went with OnTriggerEnter and used RigidBody
    public virtual void OnTriggerEnter(Collider collision)
    {
        if (isPlayer)
        {
            //Debug.Log("Player Head TriggerEnter");
            if (collision.gameObject.GetComponent<Tail>())
            {
                // if you ran into an enemy tail
                if (collision.gameObject.GetComponent<Tail>().head != this)
                {
                    // do stuff
                    Debug.Log("Player Head ran into enemy Tail");
                }
                // if you ran into your own tail
                else
                {
                    // the first time you create a tail, it places it where the head is, so there's a collision. ignore the first time!
                    if (numberOfTails > 1)
                    {
                        Debug.Log("Player head ran into own Tail");
                    }
                }
            }
        }
        else
        {
            //Debug.Log("AI Head TriggerEnter");
            if (collision.gameObject.GetComponent<Tail>())
            {
                // if AI ran into an enemy tail
                if (collision.gameObject.GetComponent<Tail>().head != this)
                {
                    // do stuff
                    Debug.Log("AI Head ran into enemy Tail");
                }
                // if AI ran into its own tail
                else
                {
                    // the first time AI creates a tail, it places it where the head is, so there's a collision. ignore the first time!
                    if (numberOfTails > 1)
                    {
                        Debug.Log("AI head ran into own Tail");
                    }
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

