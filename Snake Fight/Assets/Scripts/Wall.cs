using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        // if Wall spawns on a Tail
        if (collision.gameObject.GetComponent<Tail>())
        {
            Debug.Log("Wall spawned on a Tail");
            collision.gameObject.GetComponent<Tail>().head.destroyTails(collision.gameObject.GetComponent<Tail>());  // not working?
        }
    }
}
