﻿using UnityEngine;

public class Tail : Head
{
    void Start()
    {
        // copy material from head
        GetComponent<Renderer>().material = head.m;
    }

    void Update()  // don't delete this! just leave empty or bug
    {

    }

    public override void move()
    {
        if (target)
        {
            transform.position = target.transform.position;
            target.move();
        }
    }

    //public override void destroyTails(Head whereToStop)
    //{
    //    // base case, stop recursion when you reach the first 
    //    if (target == whereToStop)
    //    {
    //        Destroy(this);
    //    }
    //    else
    //    {
    //        // uses "head" recursion, haha
    //        target.destroyTails(whereToStop);
    //        target = head;
    //        Destroy(this);
    //    }
    //}

    public override void OnTriggerEnter(Collider collision)
    {
        // need to override because Tails will inherit and run Head's OnTriggerEnter()
        //Debug.Log("Tail collided with something");

        // if Wall spawns on a Tail
        if (collision.gameObject.GetComponent<Wall>())
        {
            Debug.Log("Wall spawned on a Tail");
            head.destroyTails(target);
        }
    }
}
