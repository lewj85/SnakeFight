﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    static public Vector3 mapSize;

	[SerializeField]
	static public float updateInterval;

    [SerializeField]
	static public int numberOfLives;

	static public Vector3 food1Location;
	static public Vector3 food2Location;
    static public Vector3 item1Location;
    static public int timeUntilItem1Spawn;

	static private Transform spawnPoint1;
	static private Transform spawnPoint2;
	static private Transform spawnPoint3;
	static private Transform spawnPoint4;
	static public Transform[] spawnPointList;

    public bool locationIsTaken(Vector3 locationToCheck)
    {
        float radius = 0.4f;
        return Physics.CheckSphere(locationToCheck, radius);
    }


    // Use this for initialization
    void Start()
    {
        mapSize.Set(15, 0, 15);
        updateInterval = 0.25f;
        numberOfLives = 999;
        timeUntilItem1Spawn = 300;

        spawnPoint1 = new GameObject().transform;
        spawnPoint1.position = new Vector3(-10, 0, -10);
        spawnPoint1.rotation = new Quaternion(0, 0, 0, 1);
        spawnPoint2 = new GameObject().transform;
        spawnPoint2.position = new Vector3(-10, 0, 10);
        spawnPoint2.rotation = new Quaternion(0, 1, 0, 0);
        spawnPoint3 = new GameObject().transform;
        spawnPoint3.position = new Vector3(10, 0, -10);
        spawnPoint3.rotation = new Quaternion(0, 0, 0, 1);
        spawnPoint4 = new GameObject().transform;
        spawnPoint4.position = new Vector3(10, 0, 10);
        spawnPoint4.rotation = new Quaternion(0, 1, 0, 0);

        spawnPointList = new Transform[] { spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4 };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadingScreenManager.LoadScene(0);
        }

        food1Location = GameObject.Find("Food1").transform.position;
        food2Location = GameObject.Find("Food2").transform.position;

        if (GameObject.Find("Player1"))
        {
            if (GameObject.Find("Player1").GetComponent<Head>().livesRemaining == 0) { Debug.Log("Player1 is dead"); }
        }
        if (GameObject.Find("AI1"))
        {
            if (GameObject.Find("AI1").GetComponent<Head>().livesRemaining == 0) { Debug.Log("AI1 is dead"); }
        }

        timeUntilItem1Spawn -= 1;
        if (timeUntilItem1Spawn < 0)
        {
            do
            {
                item1Location = new Vector3(Random.Range(-(int)mapSize.x, (int)mapSize.x), 0.0f, Random.Range(-(int)mapSize.z, (int)mapSize.z));
            } while (locationIsTaken(item1Location));
            GameObject.Find("Item1").transform.position = item1Location;
            timeUntilItem1Spawn = 300;  // reset spawn timer to ~5 seconds - let it bounce around to be 'elusive'
        }
    }
}