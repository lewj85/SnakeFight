using UnityEngine;

public class Tail : Head
{
    // copy material from head
    //private Material mt = head.GetComponentInChildren<Material>();
    
    void Start()
    {
        GetComponentInChildren<Renderer>().material = head.m;
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
