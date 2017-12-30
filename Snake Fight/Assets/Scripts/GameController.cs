using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    [SerializeField]
    public Vector3 mapSize;

    [SerializeField]
    public int livesForPlayer1;
    [SerializeField]
    public int livesForPlayer2;
    [SerializeField]
    public int livesForPlayer3;
    [SerializeField]
    public int livesForPlayer4;

    [SerializeField]
    public int livesForAI1;
    [SerializeField]
    public int livesForAI2;
    [SerializeField]
    public int livesForAI3;
    [SerializeField]
    public int livesForAI4;

    public Vector3 food1Location;
    public Vector3 food2Location;


    // Use this for initialization
    void Start()
    {
        mapSize.Set(15, 0, 15);
        livesForPlayer1 = 3;
        livesForPlayer2 = 3;
        livesForPlayer3 = 3;
        livesForPlayer4 = 3;
        livesForAI1 = 3;
        livesForAI2 = 3;
        livesForAI3 = 3;
        livesForAI4 = 3;
    }

    // Update is called once per frame
    void Update()
    {
        food1Location = GameObject.Find("Food1").transform.position;

        if (livesForPlayer1 == 0)
        {
            Debug.Log("Player1 is dead");
        }
        if (livesForPlayer2 == 0)
        {
            Debug.Log("Player2 is dead");
        }
        if (livesForPlayer3 == 0)
        {
            Debug.Log("Player3 is dead");
        }
        if (livesForPlayer4 == 0)
        {
            Debug.Log("Player4 is dead");
        }
        if (livesForAI1 == 0)
        {
            Debug.Log("AI1 is dead");
        }
        if (livesForAI2 == 0)
        {
            Debug.Log("AI2 is dead");
        }
        if (livesForAI3 == 0)
        {
            Debug.Log("AI3 is dead");
        }
        if (livesForAI4 == 0)
        {
            Debug.Log("AI4 is dead");
        }
    }
}