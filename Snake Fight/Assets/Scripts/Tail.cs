using UnityEngine;

public class Tail : Head
{
    void Start()
    {
        // copy material from head
        GetComponent<Renderer>().material = head.m;
    }
    void Update()
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
    public override void OnTriggerEnter(Collider collision)
    {
        // need to override because Tails will inherit and run Head's OnTriggerEnter()
    }
}
