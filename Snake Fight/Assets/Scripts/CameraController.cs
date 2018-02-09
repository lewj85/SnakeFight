using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;
    Vector3 targetPositionOld, targetPositionNew, beginPosition;
    Quaternion targetRotationOld, targetRotationNew, beginRotation;

	bool targetHasMoved { get { return targetPositionNew != targetPositionOld; } }
	bool targetHasTurned { get { return targetRotationNew != targetRotationOld; } }

	float positionElapsedTime, rotationElapsedTime;
	float movePeriod, rotationPeriod;

	// Use this for initialization
	void Start ()
    {
		movePeriod = GameController.updateInterval;
		rotationPeriod = GameController.updateInterval * 2.5f;
	}

    // lerp formulas
	float linearTime(float t) { return t; }
	float exponentialEaseOutTime(float t) { return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t); }
	float circleEaseOutTime(float t) { t -= 1; return Mathf.Sqrt(1 - t * t); }

	// Update is called once per frame
	void Update ()
    {
		targetPositionOld = targetPositionNew;  // position in last Update()
		targetPositionNew = target.transform.position;  // get new position
		targetRotationOld = targetRotationNew;  // rotation in last Update()
		targetRotationNew = target.transform.rotation;  // get new rotation

		positionElapsedTime += Time.deltaTime; //time since last update frame
		rotationElapsedTime += Time.deltaTime; //time since last update frame

        // if you DON'T want the camera POSITION to lerp (except during rotations), comment out the code below
        if (targetHasMoved)
        {
            beginPosition = transform.position;
            positionElapsedTime = 0;
        }

        // start turning camera immediately if the target has rotated
        if (targetHasTurned)
		{
            // if you DON'T want the camera POSITION to lerp (except during rotations), use the code below
            //beginPosition = transform.position;
            beginRotation = transform.rotation;  // use camera's current rotation as start
			rotationElapsedTime = 0;
		}

        // if you DON'T want the camera POSITION to lerp (except during rotations), use the code below
        //if (rotationElapsedTime < rotationPeriod) transform.position = Vector3.Lerp(beginPosition, targetPositionNew, circleEaseOutTime(Mathf.Min(rotationElapsedTime / rotationPeriod, 1)));
        //else transform.position = target.transform.position;
        transform.position = Vector3.Lerp(beginPosition, targetPositionNew, linearTime(Mathf.Min(positionElapsedTime / movePeriod, 1)));

        // use circleEaseOutTime for rotation, not linearTime
        transform.rotation = Quaternion.Lerp(beginRotation, targetRotationNew, circleEaseOutTime(Mathf.Min(rotationElapsedTime / rotationPeriod, 1)));
	}
}
