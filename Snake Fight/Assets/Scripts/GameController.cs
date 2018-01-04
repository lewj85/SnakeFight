using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    public Vector3 mapSize;

    [SerializeField]
    public float updateInterval;

    [SerializeField]
    public int numberOfLives;

    public Vector3 food1Location;
    public Vector3 food2Location;

    private Transform spawnPoint1;
    private Transform spawnPoint2;
    private Transform spawnPoint3;
    private Transform spawnPoint4;
    public Transform[] spawnPointList;


    // Use this for initialization
    void Start()
    {
        mapSize.Set(15, 0, 15);
        updateInterval = 0.25f;

        numberOfLives = 3;

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
            updateInterval = 0;
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
    }
}