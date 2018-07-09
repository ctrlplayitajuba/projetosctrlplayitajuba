using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	/// <summary>
	/// Game related variables
	/// </summary>
	#region
	private Joystick joystick; 								//referência do joystick virtual do jogo
	private Joybutton buttonFireball;						//referência do botão virtual do jogo para a bola de fogo 
	private Transform mainCamera;							//câmera principal do jogo que seguirá o jogador
	private Vector3 cameraOffset;							//distância entre jogador e a câmera
	[SerializeField] private float deadZoneLimit = 5f;		//tamanho da área morta da câmera
	#endregion

	/// <summary>
	/// Player related variables
	/// </summary>
	#region
	private Rigidbody player;								//referência do rigidbody do jogador
	[SerializeField] private float speed = 10f;				//velocidade de movimento do jogador
	private Transform spellSpawnPoint;						//ponto em que as bolas de fogo são spawnadas
	public GameObject fireball;								//bola de fogo que o jogador pode lançar
	[SerializeField] private float fireballSpeed    = 15f;  //velocidade de movimento da bola de fogo
	[SerializeField] private float fireballCooldown = 1f;	//velocidade de movimento da bola de fogo
	#endregion

	// Use this for initialization
	void Start () {
		cameraOffset = new Vector3 (0f, 35f, -25f);
		spellSpawnPoint = this.transform.GetChild (1);
		joystick = FindObjectOfType<Joystick> ();
		buttonFireball = GameObject.FindWithTag("Fireball").GetComponent<Joybutton>();
		buttonFireball.setCooldown (fireballCooldown);
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
		LookForward ();

		if (buttonFireball.isPressed ()) {
			CmdCastFireball ();
		}
	}

	/// <summary>
	/// Move a câmera conforme a posição do jogador, deixando uma zona morta no centro da tela em que a câmera não se move
	/// </summary>
	void MoveCamera() {
		Vector3 positionInDeadZone = this.transform.position - mainCamera.position + cameraOffset;
		if (positionInDeadZone.magnitude > deadZoneLimit) {
			mainCamera.Translate (positionInDeadZone.normalized * speed * Time.deltaTime * 0.95f, Space.World);
		}
	}

	/// <summary>
	/// Faz o jogador sempre olhar para a direção que está andando
	/// </summary>
	void LookForward(){
		this.transform.LookAt (this.transform.position + (new Vector3(joystick.Horizontal, 0, joystick.Vertical)) * 2);
	}

	/// <summary>
	/// Lança uma bola de fogo
	/// </summary>
	[Command]
	void CmdCastFireball(){
		var ball = (GameObject)Instantiate (fireball, spellSpawnPoint.position, this.transform.rotation);
		Vector3 direction = spellSpawnPoint.position - this.transform.position;
		direction.y = 0;
		direction = direction.normalized;
		ball.GetComponent<Rigidbody> ().velocity = direction * fireballSpeed;
		NetworkServer.Spawn (ball);
		Destroy (ball, 5f);
	}
}
