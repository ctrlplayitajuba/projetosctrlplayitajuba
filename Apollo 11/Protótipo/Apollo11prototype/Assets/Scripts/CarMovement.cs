using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estrutura que guarda:
///   - Tempo que o carro fica em movimento (time);
///   - Direção que o carro apontava no início do movimento (direction);
///   - Se existe caminho para frente, esquerda ou para a direita no início do movimento (FREE_FORWARD, FREE_LEFT, FREE_RIGHT respectivamente).
/// </summary>
public struct Movement
{
	public float time;
	public int direction;
	public bool FREE_FORWARD;
	public bool FREE_LEFT;
	public bool FREE_RIGHT;
}

public class CarMovement : MonoBehaviour {

	/// <summary>
	/// Atributos da classe
	/// </summary>
	#region
	//Variáveis FLOAT
	[SerializeField] private float speed 			= 50.0f; 					//velocidade do carro
	[SerializeField] private float turnTime 		= 0.2f;  					//tempo que o carro demorará para rotacionar
	private float moveTime 							= 0f;  				    	//duração de um movimento
	private float CORRIDOR_WIDTH					= 10.0f;					//largura da pista
	private float timer								= 0.0f;						//timer para uma nova detecção de paredes
	private const float movementSafetyCoeficient	= 1.05f;					//margem de segurança de tempo para os movimentos 

	//Variáveis BOOL
	private bool isRotating 						= false;					//indica se o carro está rotacionando
	private bool isCreatingMovement                 = false;					//indica se o carro já está empilhando um movimento
	private bool canMove							= false;					//indica se o carro pode andar
	private bool isReversing 						= false;					//indica se o carro está invertendo algum movimento da pilha
	private bool isRewinding 						= false;					//indica se o carro entrou na fase de avaliação para desempilhar movimento
	private bool reachedEnd 						= false;					//indica se o carro encontrou o final da pista

	//Variáveis STACK
	private Stack<Movement> movementStack 			= new Stack<Movement>();	//pilha que guarda a sequência de movimentos do carro

	//Variáveis LIST
	private List<Movement> pathToEnd				= new List<Movement>();
	private List<Movement> pathToStart				= new List<Movement>();

	//Variáveis INT
	private int initialTurn							= FORWARD;					//direção que o carro virou no início do movimento
	private int[] TurnAngle;													//vetor para as direções
	private const int FORWARD						= 0;						//constante para indicar a frente local do carro
	private const int RIGHT 						= 1;						//constante para indicar a direita local do carro
	private const int BACKWARDS 					= 2;						//constante para indicar as costas local do carro
	private const int LEFT 							= 3;						//constante para indicar a esquerda local do carro

	//Variáveis CHARACTER CONTROLLER
	private CharacterController controller;										//character controller do carro

	//Variáveis COLLIDER
	public Collider finishCollider;
	#endregion

	/// <summary>
	/// Indica que a rotação do carro terminou
	/// </summary>
	private void FinishedRotating(){
		isRotating = false;
		moveTime = 0f;
		isCreatingMovement = false;
	}

	/// <summary>
	/// Método que faz o carro se mover para frente por "time" segundos
	/// </summary>
	/// <param name="time">tempo que o carro continuará em movimento</param>
	public void Move (float moveTime){
		iTween.MoveBy(gameObject, iTween.Hash(
			"x", moveTime*speed, 
			"time", moveTime, 
			"easetype", iTween.EaseType.linear,
			"oncomplete", "FinishedMoving"));
	}

	/// <summary>
	/// Método que faz o carro girar no própio eixo por "angle" graus
	/// </summary>
	/// <param name="angle"> ângulo em graus que o carro vai virar </param>
	/// <param name="turnTime"> tempo que demora para o carro girar </param>
	public void Rotate (int angle, float turnTime){
		isRotating = true;
		iTween.RotateBy (gameObject, iTween.Hash (
			"y", TurnAngle[angle]/360.0f,
			"time", turnTime,
			"easetype", iTween.EaseType.linear,
			"oncomplete", "FinishedRotating"));
	}

	/// <summary>
	/// Retira o último movimento realizado da pilha de movimentos
	/// e executa seu inverso
	/// </summary>
	private IEnumerator ReverseMovement () {
		canMove = false;
		if (movementStack.Count != 0) {
			Movement movement = movementStack.Pop ();
			Rotate (BACKWARDS, turnTime);
			yield return new WaitForSeconds (turnTime);
			Move (movement.time);
			yield return new WaitForSeconds (movement.time * movementSafetyCoeficient);
			if(movement.direction == FORWARD)
				Rotate (BACKWARDS, turnTime);
			else
				Rotate (movement.direction, turnTime);
			yield return new WaitForSeconds (turnTime);
		}
		isReversing = false;
	}

	private IEnumerator Rewind () {
		isRewinding = true;
		while (movementStack.Count > 0) {
			if (movementStack.Peek ().FREE_RIGHT) {
				Rotate (RIGHT, turnTime);
				initialTurn = RIGHT;
				yield return new WaitForSeconds (turnTime);
				canMove = true;
				Movement movement = movementStack.Pop ();
				movement.FREE_RIGHT = false;
				movementStack.Push (movement);
				break;
			} else if (movementStack.Peek ().FREE_LEFT) {
				Rotate (LEFT, turnTime);
				initialTurn = LEFT;
				yield return new WaitForSeconds (turnTime);
				canMove = true;
				Movement movement = movementStack.Pop ();
				movement.FREE_LEFT = false;
				movementStack.Push (movement);
				break;
			} else {
				if (movementStack.Count < 1)
					break;
				else {
					int direction = movementStack.Peek ().direction;
					isReversing = true;
					StartCoroutine (ReverseMovement ());
					yield return new WaitWhile (() => isReversing);
					if (movementStack.Count > 0) {
						Movement movement = new Movement ();
						movement = movementStack.Pop ();
						movement.FREE_FORWARD = false;
						if (direction == RIGHT)
							movement.FREE_RIGHT = false;
						else if (direction == LEFT)
							movement.FREE_LEFT = false;
						movementStack.Push (movement);
					}
				}
			}
		}
		isRewinding = false;
	}

	/// <summary>
	/// Mostra a direção para frente local do carro
	/// </summary>
	/// <returns>The forward direction.</returns>
	private Vector3 GetForwardDirection(){
		return transform.right;
	}

	/// <summary>
	/// Mostra a direção para a direita local do carro
	/// </summary>
	/// <returns>The right direction.</returns>
	private Vector3 GetRightDirection(){
		return - transform.forward;
	}

	/// <summary>
	/// Mostra a direção para a esquerda local do carro
	/// </summary>
	/// <returns>The left direction.</returns>
	private Vector3 GetLeftDirection(){
		return transform.forward;
	}

	/// <summary>
	/// Funciona como sensores de distância. 
	/// Detecta se existe parede para a frente e para os lados utilizando Raycast
	/// </summary>
	private void Detection(){
		bool canTurnLeft, canTurnRight, canGoForward;
		canTurnLeft = canTurnRight = false;
		canGoForward = true;
		RaycastHit hit;
		Ray ray = new Ray();
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		//detecta esquerda
		ray.direction = GetLeftDirection();
		if (!Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 1.5f)) {
			canTurnLeft = true;
			canMove = false;
		}
		Debug.DrawLine(ray.origin, hit.point);

		//detecta direita
		ray.direction = GetRightDirection();
		if (!Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 1.5f)) {
			canTurnRight = true;
			canMove = false;
		}
		Debug.DrawLine(ray.origin, hit.point);

		//detecta frente
		ray.direction = GetForwardDirection();
		if (Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 1.5f)) {
			canGoForward = false;
			canMove = false;
		}
		if (hit.collider != null) {
			if (hit.collider.ToString ().Equals (finishCollider.ToString ())) {
				reachedEnd = true;
			}
		}
		Debug.DrawLine(ray.origin, hit.point);

		DetectionTreatment (canTurnLeft, canTurnRight, canGoForward);
	}

	/// <summary>
	/// Utiliza os dados gerados pelo método Detection() para avaliar se é necessário
	/// empilhar um novo movimento. Isso ocorre caso ele encontre uma parede a frente
	/// ou se há caminho livre tanto para frente quanto para um ou dois lados.
	/// </summary>
	/// <param name="canTurnLeft">If set to <c>true</c> can turn left.</param>
	/// <param name="canTurnRight">If set to <c>true</c> can turn right.</param>
	/// <param name="canGoForward">If set to <c>true</c> can go forward.</param>
	private void DetectionTreatment(bool canTurnLeft, bool canTurnRight, bool canGoForward){
		if (canGoForward && !canTurnLeft && !canTurnRight) {
			canMove = true;
		}
		else {
			if (!isCreatingMovement) {
				isCreatingMovement = true;
				PileMovement(initialTurn, canTurnLeft, canTurnRight, canGoForward);
				if (reachedEnd)
					MakePaths ();
				if (!canGoForward && canTurnRight) {
					Rotate(RIGHT, turnTime);
					initialTurn = RIGHT;
				}
				else if (!canGoForward && canTurnLeft) {
					Rotate(LEFT, turnTime);
					initialTurn = LEFT;
				}
				else if (canGoForward && (canTurnLeft || canTurnRight)) {
					Rotate (FORWARD, turnTime);
					initialTurn = FORWARD;
				}
				else if (!isRewinding && !reachedEnd)
						StartCoroutine (Rewind ());
			}
		}
	}

	/// <summary>
	/// Empilha um novo movimento
	/// </summary>
	/// <param name="turn">Direção da rotação inicial do carro para o movimento</param>
	private void PileMovement(int turn, bool canTurnLeft, bool canTurnRight, bool canGoForward) {
		Movement movement = new Movement();
		movement.direction = turn;
		movement.time = moveTime;
		movement.FREE_LEFT = canTurnLeft;
		movement.FREE_RIGHT = canTurnRight;
		movement.FREE_FORWARD = canGoForward;
		movementStack.Push(movement);
		canMove = true;
	}

	/// <summary>
	/// Utiliza a lista de movimentos do destino final até o começo (pathToStart) para fazer o carro voltar à posição inicial
	/// </summary>
	/// <returns>delay</returns>
	private IEnumerator ReturnToStart() {
		Rotate (BACKWARDS, turnTime);
		yield return new WaitForSeconds (turnTime);
		for (int i = 0; i < pathToStart.Count; i++) {
			Move (pathToStart[i].time);
			yield return new WaitForSeconds (pathToStart[i].time * movementSafetyCoeficient);
			if (pathToStart [i].direction != FORWARD) {
				Rotate (pathToStart [i].direction, turnTime);
				yield return new WaitForSeconds (turnTime);
			}
		}
		Rotate (BACKWARDS, turnTime);
		yield return new WaitForSeconds (turnTime);
	}

	/// <summary>
	/// Utiliza a lista de movimentos do destino começo até o final (pathToEnd) para fazer o carro ir até posição final
	/// </summary>
	/// <returns>delay</returns>
	private IEnumerator GoToEnd() {
		for (int i = 0; i < pathToEnd.Count; i++) {
			if (pathToEnd [i].direction != FORWARD) {
				Rotate (pathToEnd [i].direction, turnTime);
				yield return new WaitForSeconds (turnTime);
			}
			Move (pathToEnd[i].time);
			yield return new WaitForSeconds (pathToEnd[i].time * movementSafetyCoeficient);
		}
	}

	/// <summary>
	/// Cria listas com os movimentos necessários para ir do destino final até o começo e vice-versa
	/// </summary>
	private void MakePaths(){
		reachedEnd = true;
		Stack<Movement> movesToStart = new Stack<Movement>(new Stack<Movement>(new Stack<Movement>(movementStack)));
		Stack<Movement> movesToEnd = new Stack<Movement>();
		Movement movement = new Movement ();

		for (int i = movesToStart.Count; i > 0; i--) {
			movement = movesToStart.Pop ();
			movesToEnd.Push (movement);
			if (movement.direction == RIGHT)
				movement.direction = LEFT;
			else if (movement.direction == LEFT)
				movement.direction = RIGHT;
			pathToStart.Insert (0, movement);
		}
		for (int i = movesToEnd.Count; i > 0; i--) {
			pathToEnd.Insert (0, movesToEnd.Pop ());
		}
	}

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();

		TurnAngle = new int[4];
		TurnAngle [FORWARD] 	= 0;
		TurnAngle [RIGHT] 		= 90;
		TurnAngle [BACKWARDS] 	= 180;
		TurnAngle [LEFT]		= -90;
	}

	// Update is called once per frame
	void Update () {
		if (isCreatingMovement || isRewinding || reachedEnd)
			timer = 0;
		else
			timer += Time.deltaTime;
		if (timer > CORRIDOR_WIDTH / speed) {
			if(!isRotating)
				Detection ();
			timer = 0;
		}

		if (canMove && !isRotating && !reachedEnd) {
			controller.Move(GetForwardDirection() * speed * Time.deltaTime);
			moveTime += Time.deltaTime;
		}

		if (Input.GetKeyUp (KeyCode.S) && reachedEnd) {
			StartCoroutine (ReturnToStart());
		}
		if (Input.GetKeyUp (KeyCode.E) && reachedEnd) {
			StartCoroutine (GoToEnd());
		}
		if (Input.GetKeyUp (KeyCode.R) && reachedEnd) {
			Rotate(BACKWARDS, turnTime);
		}
	}
}
