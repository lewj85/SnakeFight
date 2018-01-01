using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    private Vector3 mapSize;

    private int cyclesSinceTrigger;

    // the reason we don't just set relocating = cyclesSinceTrigger < 2 is because we want to update it
    // every time that it is checked. the syntax to do that is below. can also use 'set' to do a whole bunch
    // of stuff anytime we try to set the new value of relocating. syntax for that is set { relocating = value; } 
    protected bool relocating
    {
        set
        {
            cyclesSinceTrigger = (value ? 0 : 1);
            // enable/disable rendering
            GetComponent<MeshRenderer>().enabled = (cyclesSinceTrigger > 0);
        }
        get { return cyclesSinceTrigger < 2; }
    }


    protected Vector3 findLocation()
    {
        // get variable from script of another object
        mapSize = GameObject.Find("GameController1").GetComponent<GameController>().mapSize;
        // reset the count
        relocating = true;
        // note: y is set to 0 to keep on the same plane
        return new Vector3(Random.Range(-(int)mapSize.x, (int)mapSize.x), 0.0f, Random.Range(-(int)mapSize.z, (int)mapSize.z));
    }


    // Use this for initialization
    void Start()
    {
        // find a location when we start
        //transform.position = findLocation();
        cyclesSinceTrigger = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // increment
        cyclesSinceTrigger++;
        //if (!relocating) GetComponent<MeshRenderer>().enabled = true;
    }

    // when clicking "Is Trigger" we change Sphere Collider to become a trigger 
    // so we can use OnTriggerEnter instead of OnCollisionEnter
    void OnTriggerEnter(Collider other)
    {
        Head head = other.GetComponent<Head>();  // head will be null if other isn't a Head OR Tail object
        Tail tail = other.GetComponent<Tail>();  // tail will be null if other isn't a Tail
        // if a head is what hit us
        if (head)
        {
            // if it's a head but not a tail
            if (!tail)
            {
                // add a tail piece to that head
                head.extend();
            }

            // move food
            // "transform" is a built-in component that contains position, scale, and rotation. we want to update the position
            transform.position = findLocation();
        }
        // any time an object is colliding with any objects for more than 1 frame 
        // (even if it moves and collides with a 3rd object)
        OnTriggerStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // first check if we're relocating, if so then we need to find a new empty location
        if (relocating)
        {
            transform.position = findLocation();
        }
    }

    // make sure to reset relocating to false after done colliding
    void OnTriggerExit(Collider other)
    {
        relocating = false;
    }
}
