using UnityEngine;

public class Head : MonoBehaviour
{
    [SerializeField]
    private float updateInterval;
    private float elapsed;

    [SerializeField]
    private GameObject tail;

    Transform View;
    Vector3 rotation;

    protected Head target;
    //protected Head head;

    void Start()
    {
        View = transform.Find("Camera");
        target = this;
        //head = this;
    }

    public virtual void move()
    {
        transform.Rotate(rotation);
        transform.position += transform.forward;
        rotation = Vector3.zero;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= updateInterval)
        {
            target.move();
            elapsed = 0;
        }

        if (Input.GetButtonDown("Right")) rotation = new Vector3(0, 90, 0);
        else if (Input.GetButtonDown("Left")) rotation = new Vector3(0, -90, 0);
        else if (Input.GetButtonDown("Up")) rotation = new Vector3(-90, 0, 0);
        else if (Input.GetButtonDown("Down")) rotation = new Vector3(90, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Head tmp = target;
            target = Instantiate(tail, target.transform).GetComponent<Tail>();
            target.updateInterval = updateInterval;
            target.transform.parent = null;
            //target.head = this;
            target.target = tmp;
        }
    }
}

