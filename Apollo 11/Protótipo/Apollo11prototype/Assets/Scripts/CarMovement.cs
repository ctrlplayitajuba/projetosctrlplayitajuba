using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerado que indica o ângulo de virar para a direita (RIGHT) e esquerda (LEFT)
/// </summary>
public enum TurnAngle{
	RIGHT = 90,
	LEFT = -90,
	BACKWARDS = 180,
	FORWARDS = 0
}

/// <summary>
/// Estrutura que guarda:
///   - Tempo que o carro fica em movimento (time);
///   - Direção que o carro apontava no início do movimento (direction);
///   - Se existe caminho para frente, esquerda ou para a direita no início do movimento (FREE_FORWARD, FREE_LEFT, FREE_RIGHT respectivamente).
/// </summary>
public struct Movement
{
	public float time;
	public TurnAngle direction;
	public bool FREE_FORWARD;
	public bool FREE_LEFT;
	public bool FREE_RIGHT;
}

public class CarMovement : MonoBehaviour {

	/// <summary>
	/// Atributos da classe
	/// </summary>
	#region
	[SerializeField] private float speed 			= 10.0f; 					//velocidade do carro
	[SerializeField] private float turnTime 		= 0.5f;  					//tempo que o carro demorará para rotacionar
	private float moveTime 							= 0f;  				    	//tempo que o carro ficará em movimento
	private float movementStartTime					= 0f;						//momento que o movimento começou
	private float movementEndTime					= 0f;                       //momento que o movimento terminou
	private float CORRIDOR_WIDTH					= 10.0f;					//largura da pista
	private float timer								= 0.0f;						//tempo para detecção de paredes
	private bool isMoving 							= false;					//indica se o carro está se movendo
	private bool isRotating 						= false;					//indica se o carro está rotacionando
	private bool isCreatingMovement                 = false;					//indica se o carro já está empilhando um movimento
	private bool canMove							= false;					//indica se o carro pode andar
	private bool finished 							= false;
	private bool isReversing 						= false;
	private bool isRewinding 						= false;
	private Stack<Movement> movementStack 			= new Stack<Movement>();	//pilha que guarda a sequência de movimentos do carro
	private TurnAngle initialTurn					= TurnAngle.FORWARDS;		//direção que o carro virou no início do movimento
	private CharacterController controller;										//character controller do carro
	#endregion

	/// <summary>
	/// Indica que o movimento do carro terminou
	/// </summary>
	private void FinishedMoving(){
		isMoving = false;
	}

	/// <summary>
	/// Indica que a rotação do carro terminou
	/// </summary>
	private void FinishedRotating(){
		isRotating = false;
		moveTime = 0f;
		StartCoroutine(Wait(CORRIDOR_WIDTH / speed));
	}

	private IEnumerator Wait (float time) { 
		yield return new WaitForSeconds(time);
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
	public void Rotate (TurnAngle angle, float turnTime){
		isRotating = true;
		iTween.RotateBy (gameObject, iTween.Hash (
			"y", (int)angle/360.0f,
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
			Rotate (TurnAngle.BACKWARDS, turnTime);
			yield return new WaitForSeconds (turnTime);
			Move (movement.time);
			yield return new WaitForSeconds (movement.time);
			if(movement.direction == TurnAngle.FORWARDS)
				Rotate (TurnAngle.BACKWARDS, turnTime);
			else
				Rotate (movement.direction, turnTime);
			yield return new WaitForSeconds (turnTime);
		}
		isReversing = false;
	}

	private IEnumerator Rewind () {
		int i = 0;
		isRewinding = true;
		while (movementStack.Count > 0) {
			if (movementStack.Peek ().FREE_RIGHT) {
				Rotate (TurnAngle.RIGHT, turnTime);
				Debug.Log ("ENTROU RIGHT");
				break;
			} else if (movementStack.Peek ().FREE_LEFT) {
				Rotate (TurnAngle.LEFT, turnTime);
				Debug.Log ("ENTROU LEFT");
				break;
			} else {
				Debug.Log ("ENTROU: " + i);
				i++;
				StartCoroutine (ReverseMovement ());
				isReversing = true;
				yield return new WaitWhile (() => isReversing);
				Movement movement = new Movement ();
				movement = movementStack.Pop ();
				movement.FREE_FORWARD = false;
				if (movement.direction == TurnAngle.RIGHT)
					movement.FREE_RIGHT = false;
				else if (movement.direction == TurnAngle.LEFT)
					movement.FREE_LEFT = false;
				movementStack.Push (movement);
			}
		}
		isRewinding = false;
		finished = false;
	}

	/// <summary>
	/// Determina se o carro pode executar uma ação de movimento ou rotação
	/// </summary>
	/// <returns><c>true</c> Se o carro não está se movendo nem rotacionando; senão, <c>false</c>.</returns>
	private bool CanMove(){
		return !isMoving && !isRotating;
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
		if (!Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 2)) {
			canTurnLeft = true;
			canMove = false;
		}
		Debug.DrawLine(ray.origin, hit.point);

		//detecta direita
		ray.direction = GetRightDirection();
		if (!Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 2)) {
			canTurnRight = true;
			canMove = false;
		}
		Debug.DrawLine(ray.origin, hit.point);

		//detecta frente
		ray.direction = GetForwardDirection();
		if (Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH - 2)) {
			canGoForward = false;
			canMove = false;
		}
		Debug.DrawLine(ray.origin, hit.point);

		NewMovement (canTurnLeft, canTurnRight, canGoForward);
	}

	/// <summary>
	/// Utiliza os dados gerados pelo método Detection() para avaliar se é necessário
	/// empilhar um novo movimento. Isso ocorre caso ele encontre uma parede a frente
	/// ou se há caminho livre tanto para frente quanto para um ou dois lados.
	/// </summary>
	/// <param name="canTurnLeft">If set to <c>true</c> can turn left.</param>
	/// <param name="canTurnRight">If set to <c>true</c> can turn right.</param>
	/// <param name="canGoForward">If set to <c>true</c> can go forward.</param>
	private void NewMovement(bool canTurnLeft, bool canTurnRight, bool canGoForward){
		if (canGoForward && !canTurnLeft && !canTurnRight) {
			canMove = true;
		}
		else {
			if (!isCreatingMovement) {
				isCreatingMovement = true;
				movementEndTime = Time.time;
				PileMovement(initialTurn, canTurnLeft, canTurnRight, canGoForward);
				if (!canGoForward && canTurnRight) {
					Rotate(TurnAngle.RIGHT, turnTime);
					initialTurn = TurnAngle.RIGHT;
				}
				else if (!canGoForward && canTurnLeft) {
					Rotate(TurnAngle.LEFT, turnTime);
					initialTurn = TurnAngle.LEFT;
				}
				else if (canGoForward && (canTurnLeft || canTurnRight)) {
					Rotate(TurnAngle.FORWARDS, turnTime);
					initialTurn = TurnAngle.FORWARDS;
				}
				else {
					Debug.Log("REWIND");
					finished = true;
					if (!isRewinding)
						StartCoroutine (Rewind ());
				}
			}
		}
	}

	/// <summary>
	/// Empilha um novo movimento
	/// </summary>
	/// <param name="turn">Direção da rotação inicial do carro para o movimento</param>
	private void PileMovement(TurnAngle turn, bool canTurnLeft, bool canTurnRight, bool canGoForward) {
		Movement movement = new Movement();
		movement.direction = turn;
		movement.time = /*movementEndTime - movementStartTime*/ moveTime;
		movement.FREE_LEFT = canTurnLeft;
		movement.FREE_RIGHT = canTurnRight;
		movement.FREE_FORWARD = canGoForward;
		movementStack.Push(movement);
		movementStartTime = Time.time;
		canMove = true;
	}

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update () {
		if (isCreatingMovement || finished)
			timer = 0;
		else
			timer += Time.deltaTime;
		if (timer > CORRIDOR_WIDTH / speed) {
			if(!isRotating)
				Detection ();
			timer = 0;
		}

		if (canMove && !isRotating && !finished) {
			controller.Move(GetForwardDirection() * speed * Time.deltaTime);
			moveTime += Time.deltaTime;
		}
	}
}
