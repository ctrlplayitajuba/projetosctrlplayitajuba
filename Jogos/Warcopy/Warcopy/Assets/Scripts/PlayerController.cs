using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	/// <summary>
	/// Game related variables
	/// </summary>
	#region
	private Joystick joystick; 								//referência do joystick virtual do jogo
	private Transform mainCamera;							//câmera principal do jogo que seguirá o jogador
	private Vector3 cameraOffset;							//distância entre jogador e a câmera
	[SerializeField] private float deadZoneLimit = 5f;		//tamanho da área morta da câmera
	#endregion

	/// <summary>
	/// Player related variables
	/// </summary>
	#region
	private Rigidbody player;								//referência do rigidbody3d do jogador
	[SerializeField] private float speed = 10f;				//velocidade de movimento do jogador]
	public GameObject fireball;								//bola de fogo que o jogador pode lançar
	#endregion




	// Use this for initialization
	void Start () {
		cameraOffset = new Vector3 (0f, 35f, -25f);
		joystick = FindObjectOfType<Joystick> ();
		player = GetComponent<Rigidbody>();
		mainCamera = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
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
