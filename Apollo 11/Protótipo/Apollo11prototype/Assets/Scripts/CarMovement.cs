using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerado que indica o ângulo de virar para a direita (RIGHT) e esquerda (LEFT)
/// </summary>
public enum TurnAngle{
	RIGHT = 90,
	LEFT = -90,
	BACKWARDS = 180
}

public struct Movement
{
	public float time;
	public TurnAngle direction;
	public bool FREE_FORWARD;
	public bool FREE_LEFT;
	public bool FREE_RIGHT;
}

public class CarMovement : MonoBehaviour {

	[SerializeField] private float speed 			= 5.0f;  //velocidade do carro
	[SerializeField] private float moveTime 		= 1.0f;  //tempo que o carro ficará em movimento
	[SerializeField] private float turnTime 		= 0.5f;  //tempo que o carro demorará para rotacionar
	private bool isMoving = false;
	private bool isRotating = false;
	private Stack<Movement> movementStack = new Stack<Movement>();

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
			Rotate (movement.direction, turnTime);
		}
	}

	// Use this for initialization
	void Start () {
		/*Movement movement = new Movement ();
		movement.time = moveTime;
		movement.direction = TurnAngle.RIGHT;
		movementStack.Push (movement);

		movement.time = moveTime;
		movement.direction = TurnAngle.RIGHT;
		movementStack.Push (movement);

		movement.time = moveTime;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);*/
	}

	/// <summary>
	/// Cria uma série de movimentos e os empurra para a pilha de movimentos
	/// </summary>
	/// <returns>The movements.</returns>
	public IEnumerator PresetMovements(){
		Movement movement = new Movement ();
		movement.time = 1.0f;
		movement.direction = TurnAngle.RIGHT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);

		movement.time = 3.0f;
		movement.direction = TurnAngle.RIGHT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);

		movement.time = 2.0f;
		movement.direction = TurnAngle.LEFT;
		movementStack.Push (movement);
		Rotate (movement.direction, turnTime);
		yield return new WaitForSeconds (turnTime);
		Move (movement.time);
		yield return new WaitForSeconds (movement.time);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.UpArrow) && !isRotating && !isMoving) {
			Move (moveTime);
		}
		if (Input.GetKeyUp (KeyCode.RightArrow) && !isRotating && !isMoving) {
			Rotate (TurnAngle.RIGHT, turnTime);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow) && !isRotating && !isMoving) {
			Rotate (TurnAngle.LEFT, turnTime);
		}
		if (Input.GetKeyUp (KeyCode.R) && !isRotating && !isMoving) {
			StartCoroutine(Rewind ());
		}
		if (Input.GetKeyUp (KeyCode.S) && !isRotating && !isMoving) {
			StartCoroutine(PresetMovements());
		}
	}
}
