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
	[SerializeField] private float moveTime 		= 1.0f;  					//tempo que o carro ficará em movimento
	[SerializeField] private float turnTime 		= 0.5f;  					//tempo que o carro demorará para rotacionar
	private bool isMoving 							= false;					//indica se o carro está se movendo
	private bool isRotating 						= false;					//indica se o carro está rotacionando
	private Stack<Movement> movementStack 			= new Stack<Movement>();	//pilha que guarda a sequência de movimentos do carro
	private float CORRIDOR_WIDTH					= 100.0f;					//largura da pista
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
	}

	/// <summary>
	/// Método que faz o carro se mover para frente por "time" segundos
	/// </summary>
	/// <param name="time">tempo que o carro continuará em movimento</param>
	public void Move (float time){
		isMoving = true;
		iTween.MoveBy(gameObject, iTween.Hash(
			"x", time*speed, 
			"time", time, 
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
	private IEnumerator Rewind(){
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
		}
	}

	/// <summary>
	/// Determina se o carro pode executar uma ação de movimento ou rotação
	/// </summary>
	/// <returns><c>true</c> Se o carro não está se movendo nem rotacionando; senão, <c>false</c>.</returns>
	private bool CanMove(){
		return !isMoving && !isRotating;
	}

	private Vector3 GetForwardDirection(){
		return transform.right;
	}

	// Use this for initialization
	void Start () {
	}

	/// <summary>
	/// Cria uma série de movimentos e os empurra para a pilha de movimentos
	/// </summary>
	/// <returns>The movements.</returns>
	public IEnumerator PresetMovements(){
		Movement movement = new Movement ();
		movement.time = 2.0f;
		movement.direction = TurnAngle.FORWARDS;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		RaycastHit hit;
		Ray ray = new Ray();
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);

		movement.time = 3.0f;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);

		movement.time = 2.0f;
		movement.direction = TurnAngle.RIGHT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);

		movement.time = 2.0f;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);

		movement.time = 4.0f;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);

		movement.time = 3.0f;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
		ray.origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		ray.direction = GetForwardDirection();
		Physics.Raycast (ray, out hit,	CORRIDOR_WIDTH/2);
		Debug.DrawLine(ray.origin, hit.point);
		Debug.Log (hit.collider);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.UpArrow) && CanMove()) {
			Move (moveTime);
		}
		if (Input.GetKeyUp (KeyCode.RightArrow) && CanMove()) {
			Rotate (TurnAngle.RIGHT, turnTime);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow) && CanMove()) {
			Rotate (TurnAngle.LEFT, turnTime);
		}
		if (Input.GetKeyUp (KeyCode.R) && CanMove()) {
			StartCoroutine(Rewind ());
		}
		if (Input.GetKeyUp (KeyCode.S) && CanMove()) {
			StartCoroutine(PresetMovements());
		}
	}
}
