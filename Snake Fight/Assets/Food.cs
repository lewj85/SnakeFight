using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public Vector3 mapSize;
    public Vector3 foodLocation;

	// Use this for initialization
	void Start ()
    {
        // generate a random location in range mapSize(i,j,k)

        // while any object exists at that location, pick a new one
        
        // set the food object to that location

    }
	
	// Update is called once per frame
	void Update ()
    {
		// collision destroys the Food object, creates a new Food item

	}
}
