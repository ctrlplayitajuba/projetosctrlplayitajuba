using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Joystick joystick;
	private Rigidbody player;
	private Transform mainCamera;
	private Vector3 cameraOffset;
	public float deadZoneLimit = 5f;
	public float speed = 10f;

	// Use this for initialization
	void Start () {
		cameraOffset = new Vector3 (0f, 35f, -25f);
		joystick = FindObjectOfType<Joystick> ();
		player = GetComponent<Rigidbody>();
		mainCamera = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		player.velocity = new Vector3 (joystick.Horizontal * speed, player.velocity.y, joystick.Vertical * speed);
		MoveCamera ();
	}

	/// <summary>
	/// Controle do movimento da câmera
	/// </summary>
	void MoveCamera() {
		Vector3 deadZone = this.transform.position - mainCamera.position + cameraOffset;
		if (deadZone.magnitude > deadZoneLimit) {
			mainCamera.Translate (deadZone.normalized * speed * Time.deltaTime, Space.World);
		}
	}
}
