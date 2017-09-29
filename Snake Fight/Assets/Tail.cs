using UnityEngine;

public class Tail : Head
{
    void Start()
    {
        
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
