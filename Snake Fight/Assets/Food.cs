using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    private int cyclesSinceTrigger
    {
        get;
        set; // TODO: disable rendering if < 2 and enable if > 1
    }
    public Vector3 mapSize;  // move this out of food
    // the reason we don't just set relocating = cyclesSinceTrigger < 2 is because we want to update it
    // every time that it is checked. the syntax to do that is below. can also use 'set' to do a whole bunch
    // of stuff anytime we try to set the new value of relocating. syntax for that is set { relocating = value; } 
    protected bool relocating
    {
        get { return cyclesSinceTrigger < 2; }
        set;
    }


    protected Vector3 findLocation()
    {
        // reset the count
        relocating = true;

        return new Vector3(Random.Range(0,mapSize.x), Random.Range(0,mapSize.y), Random.Range(0,mapSize.z));
    }


    // Use this for initialization
    void Start ()
    {
        // find a location when we start
        transform.position = findLocation();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // increment
        cyclesSinceTrigger++;
        if (!relocating) GetComponent<MeshRenderer>().enabled = true;
	}

    // when clicking "Is Trigger" we change Sphere Collider to become a trigger 
    // so we can use OnTriggerEnter instead of OnCollisionEnter
    // OnTriggerStay and OnTriggerExit also exist
    void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // first check if we're relocating, if so then we need to find a new empty location
        if (relocating)
        {
            findLocation();
            return;
        }

        Head head = other.GetComponent<Head>();  // head will be null if other isn't a Head object
        // if a head is what hit us
        if (head)
        {
            // add a tail piece to Head
            head.extend();

            // move food
            // "transform" is a built-in component that contains position, scale, and rotation. we want to update the position
            transform.position = findLocation();
        }
    }
}
