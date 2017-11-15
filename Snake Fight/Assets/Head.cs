using UnityEngine;

public class Head : MonoBehaviour
{
    [SerializeField]
    private float updateInterval;
    private float elapsed;
    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private GameObject tail;

    Transform View;
    Vector3 rotation;
    Vector3 newXYZ;

    protected Head target;
    public Head head;

    void Start()
    {
        View = transform.Find("Camera");
        target = this;
        head = this;
    }

    public virtual void move()
    {
        transform.Rotate(rotation);
        transform.position += transform.forward;
        rotation = Vector3.zero;  // reset immediately after moving
    }

    public void extend()
    {
        Head tmp = target;
        target = Instantiate(tail, target.transform).GetComponent<Tail>();
        target.updateInterval = updateInterval;
        target.transform.parent = null;
        //target.head = this;
        target.target = tmp;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Tail>())
        {
            if (collision.gameObject.GetComponent<Tail>().head != this)
            {
                // do stuff
                Debug.Log("Ran into enemy Tail");
            }
            else
            {
                Debug.Log("Ran into own Tail");
            }
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // player controls go here
        if (isPlayer)
        {
            if (elapsed >= updateInterval)
            {
                target.move();
                elapsed = 0;
            }
        
            if (Input.GetButtonDown("Right")) rotation = new Vector3(0, 90, 0);
            else if (Input.GetButtonDown("Left")) rotation = new Vector3(0, -90, 0);
            // else if (Input.GetButtonDown("Up")) rotation = new Vector3(-90, 0, 0);
            // else if (Input.GetButtonDown("Down")) rotation = new Vector3(90, 0, 0);

            // TODO: remove, this is for testing only
            if (Input.GetKeyDown(KeyCode.Space))
            {
                extend();
            }
        }
        // AI pathing goes here
        else
        {
            // compare food locations to head location - order them by distance
            // loop
                // pop next closest food
                // create path array (adjust X first, then Z)
                // check for obstruction along the way
            // adjust position based on shortest path
        }
    }
}

