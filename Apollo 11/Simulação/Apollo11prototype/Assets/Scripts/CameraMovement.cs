using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public GameObject car;
	private const float DISTANCE_TO_CAR = 60;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.SetPositionAndRotation (new Vector3 (car.transform.position.x, DISTANCE_TO_CAR, car.transform.position.z), Quaternion.Euler (new Vector3 (90, 0, 0)));
	}
}
