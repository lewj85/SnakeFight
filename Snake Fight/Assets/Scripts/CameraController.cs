using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;
	Vector3 targetPositionOld, targetPositionNew, beginPosition;
	Quaternion targetRotationOld, targetRotationNew, beginRotation;

	//bool targetHasMoved { get { return targetPositionNew != targetPositionOld; } }
	bool targetHasTurned { get { return targetRotationNew != targetRotationOld; } }

	float positionElapsedTime, rotationElapsedTime;
	float movePeriod, rotationPeriod;

	// Use this for initialization
	void Start () {
		movePeriod = GameController.updateInterval;
		rotationPeriod = GameController.updateInterval * 2.5f;
	}

	float linearTime(float t) { return t; }

	float exponentialEaseOutTime(float t) { return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t); }

	float circleEaseOutTime(float t) { t -= 1; return Mathf.Sqrt(1 - t * t); }

	// Update is called once per frame
	void Update () {
		targetPositionOld = targetPositionNew;
		targetPositionNew = target.transform.position;
		targetRotationOld = targetRotationNew;
		targetRotationNew = target.transform.rotation;

		//positionElapsedTime += Time.deltaTime; //time since last update frame
		rotationElapsedTime += Time.deltaTime; //time since last update frame

		//if (targetHasMoved)
		//{
		//	beginPosition = transform.position;
		//	positionElapsedTime = 0;
		//}

		if (targetHasTurned)
		{
			beginPosition = transform.position;
			beginRotation = transform.rotation;
			rotationElapsedTime = 0;
		}

		if (rotationElapsedTime < rotationPeriod) transform.position = Vector3.Lerp(beginPosition, targetPositionNew, circleEaseOutTime(Mathf.Min(rotationElapsedTime / rotationPeriod, 1)));
		else transform.position = target.transform.position;

		transform.rotation = Quaternion.Lerp(beginRotation, targetRotationNew, circleEaseOutTime(Mathf.Min(rotationElapsedTime / rotationPeriod, 1)));
	}
}
