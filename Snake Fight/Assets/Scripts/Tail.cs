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
}
