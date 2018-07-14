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
	private Rigidbody playerRigidBody;							//referência do rigidbody do jogador
	private PlayerHealth playerHealth;							//vida do jogador
	private Transform playerHealthBar;							//barra de vida do jogador
	[SerializeField] private float speed = 10f;					//velocidade de movimento do jogador
	private Transform spellSpawnPoint;							//ponto em que as bolas de fogo são spawnadas
	public GameObject fireball;									//bola de fogo que o jogador pode lançar
	[SerializeField] private float fireballSpeed    = 15f;  	//velocidade de movimento da bola de fogo
	[SerializeField] private float fireballCooldown = 1f;		//velocidade de movimento da bola de fogo
	#endregion

	// Use this for initialization
	void Start () {
		cameraOffset = new Vector3 (0f, 35f, -25f);
		spellSpawnPoint = this.transform.GetChild (1);
		joystick = FindObjectOfType<Joystick> ();
		playerHealth = GetComponent<PlayerHealth> ();
		buttonFireball = GameObject.FindWithTag("Fireball").GetComponent<Joybutton>();
		buttonFireball.setCooldown (fireballCooldown);
		playerRigidBody = GetComponent<Rigidbody>();
		mainCamera = Camera.main.transform;
		playerHealthBar = this.transform.Find ("Healthbar");
		playerHealthBar.rotation = mainCamera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;	
		}
		playerHealthBar.rotation = mainCamera.rotation;
		MoveCamera ();
		MovePlayer ();
		CmdCheckLava ();
		CheckSpells ();
	}

	/// <summary>
	/// Move o player e faz com que ele sempre olhe para frente
	/// </summary>
	void MovePlayer(){
		//TargetMovePlayer (connectionToClient);
		Vector3 targetPosition = this.transform.position + new Vector3 (joystick.Horizontal * speed * Time.deltaTime, 0, joystick.Vertical * speed * Time.deltaTime);
		this.transform.position = Vector3.Lerp(this.transform.position, targetPosition , 1.0f);
		this.transform.LookAt (this.transform.position + (new Vector3(joystick.Horizontal, 0, joystick.Vertical)) * 2);
	}

	/// <summary>
	/// Move a câmera conforme a posição do jogador, deixando uma zona morta no centro da tela em que a câmera não se move
	/// </summary>
	void MoveCamera() {
		Vector3 positionInDeadZone = this.transform.position - mainCamera.position + cameraOffset;
		if (positionInDeadZone.magnitude > deadZoneLimit) {
			float smoothModifier = 0.85f;
			mainCamera.Translate (positionInDeadZone.normalized * speed * Time.deltaTime * smoothModifier, Space.World);
		}
	}

	/// <summary>
	/// Verifica se o jogador está pisando em lava
	/// </summary>
	[Command]
	void CmdCheckLava(){
		RaycastHit hit;
		Physics.Raycast (this.transform.position, Vector3.down, out hit, 3.0f);
		if(hit.collider.tag.Equals("Lava")){
			playerHealth.TakeDamage(10 * Time.deltaTime);
		}
	}

	/// <summary>
	/// Checa se algum botão de feitiço foi pressionado
	/// </summary>
	void CheckSpells(){
		if (buttonFireball.isPressed ()) {
			CmdCastFireball ();
		}
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

	[TargetRpc]
	public void TargetPush(NetworkConnection connection, float force, Vector3 explosionPoint){
		playerRigidBody.AddExplosionForce (force, explosionPoint, 2.0f);
	}
}
