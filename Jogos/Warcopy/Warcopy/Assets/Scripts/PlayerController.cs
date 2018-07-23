using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	/// <summary>
	/// Game related variables
	/// </summary>
	#region
	private Joystick joystick; 								//referência do joystick virtual do jogo
	private Joybutton buttonFireball;						//referência do botão virtual do jogo para a fireball
	private Joybutton buttonDash;							//referência do botão virtual do jogo para o dash
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
	[SerializeField] private float fireballCooldown = 1f;
	[SerializeField] private float dashCooldown = 3f;			//velocidade de movimento da bola de fogo
	[SerializeField] private float dashForceModifier = 1000f;	//modificador de força do dash
	[SerializeField] private Vector3 DashVector3;   			//Vector3 do dash original para usar de referência ao remover a força

	private bool canFire;
	private bool canDash;
	#endregion

	// Use this for initialization
	void Start () {
		if (!(Application.platform == RuntimePlatform.WindowsPlayer || 
			Application.platform == RuntimePlatform.LinuxPlayer   ||
			Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.LinuxEditor )) {
			joystick = FindObjectOfType<Joystick> ();
			buttonFireball = GameObject.FindWithTag("Fireball").GetComponent<Joybutton>();
			buttonDash = GameObject.FindWithTag("Dash").GetComponent<Joybutton>();
			buttonFireball.setCooldown (fireballCooldown);
			buttonDash.setCooldown (dashCooldown);
		}
		canFire = true;
		canDash = true;
		cameraOffset = new Vector3 (0f, 35f, -25f);
		spellSpawnPoint = this.transform.GetChild (1);

		playerHealth = GetComponent<PlayerHealth> ();

		playerRigidBody = GetComponent<Rigidbody>();
		mainCamera = Camera.main.transform;
		playerHealthBar = this.transform.Find ("Healthbar");
		playerHealthBar.rotation = mainCamera.transform.rotation;
	}

	void LateUpdate() {
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
		CheckSpellsFireball ();
		CheckSpellsDash ();
	}

	/// <summary>
	/// Move o player e faz com que ele sempre olhe para frente
	/// </summary>
	void MovePlayer(){
		if (Application.platform == RuntimePlatform.WindowsPlayer || 
			Application.platform == RuntimePlatform.LinuxPlayer   ||
			Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.LinuxEditor ) {
			Vector3 targetPosition = this.transform.position + new Vector3 (Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
			this.transform.position = Vector3.Lerp(this.transform.position, targetPosition , 1.0f);

			Ray cameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
			float rayLength;

			if(groundPlane.Raycast(cameraRay, out rayLength)){
				Vector3 pointToLook = cameraRay.GetPoint(rayLength);

				this.transform.LookAt(pointToLook);
			}
		} else {
			Vector3 targetPosition = this.transform.position + new Vector3 (joystick.Horizontal * speed * Time.deltaTime, 0, joystick.Vertical * speed * Time.deltaTime);
			this.transform.position = Vector3.Lerp(this.transform.position, targetPosition , 1.0f);
			this.transform.LookAt (this.transform.position + (new Vector3(joystick.Horizontal, 0, joystick.Vertical)) * 2);
		}
	}
	/// <summary>
	/// Habilidade de Dash do Player
	/// </summary>
	void Dash(){

		DashVector3 = this.transform.forward;
		playerRigidBody.AddForce (DashVector3 * dashForceModifier);

	}

	void Special(){



	}

	/// <summary>
	/// Move a câmera conforme a posição do jogador, deixando uma zona morta no centro da tela em que a câmera não se move
	/// </summary>
	void MoveCamera() {
		Vector3 positionInDeadZone = this.transform.position - mainCamera.position + cameraOffset;
		if (positionInDeadZone.magnitude > deadZoneLimit) {
			float smoothModifier = 0.95f;
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
	void CheckSpellsFireball(){
		if (Application.platform == RuntimePlatform.WindowsPlayer || 
			Application.platform == RuntimePlatform.LinuxPlayer   ||
			Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.LinuxEditor ) {
			if(Input.GetMouseButtonUp(0) && canFire){
				CmdCastFireball ();
				StartCoroutine(PcCooldownFireball (fireballCooldown));
			}
		} else {
			if (buttonFireball.isPressed ()) {
				CmdCastFireball ();
			}
		}
	}

	void CheckSpellsDash(){
		if (Application.platform == RuntimePlatform.WindowsPlayer || 
			Application.platform == RuntimePlatform.LinuxPlayer   ||
			Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.LinuxEditor ) {
			if(Input.GetKeyDown(KeyCode.LeftShift) && canDash){
				Debug.Log ("isDashing");
				Dash ();
				StartCoroutine(PcCooldownDash (dashCooldown));
			}
		} else {
			if (buttonDash.isPressed ()) {
				Dash ();
			}
		}
	}

	/// <summary>
	/// Implementação de cooldown para PC
	/// </summary>
	/// <returns>The cooldown.</returns>
	IEnumerator PcCooldownFireball(float seconds) {
		canFire = false;
		yield return new WaitForSeconds (seconds);
		canFire = true;
	}

	IEnumerator PcCooldownDash(float seconds) {
		canDash = false;
		yield return new WaitForSeconds (seconds);
		canDash = true;
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
