using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    [SerializeField]
    public Vector3 mapSize;
    [SerializeField]
    public int playerLives;
    public Vector3 food1Location;


    // Use this for initialization
    void Start()
    {
        mapSize.Set(15, 0, 15);
        playerLives = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerLives == 0)
        {
            Debug.Log("Game Over");
        }
        food1Location = GameObject.Find("Food1").transform.position;
    }
}