using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

	[SerializeField] private float speed = 1.0f;
	[SerializeField] private float time = 1.0f;
	[SerializeField] private float turnTime = 0.5f;
	[SerializeField] private int angle = 90;
	private bool isMoving = false;
	private bool isRotating = false;

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
	/// <param name="angle">ângulo em graus que o carro vai virar</param>
	/// <param name="turnTime">tempo que demora para o carro girar</param>
	public void Rotate (int angle, float turnTime){
		isRotating = true;
		iTween.RotateBy (gameObject, iTween.Hash (
			"y", angle/360.0f,
			"time", turnTime,
			"easetype", iTween.EaseType.linear,
			"oncomplete", "FinishedRotating"));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.UpArrow) && !isRotating && !isMoving) {
			Move (time);
		}
		if (Input.GetKeyUp (KeyCode.RightArrow) && !isRotating && !isMoving) {
			Rotate (90, turnTime);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow) && !isRotating && !isMoving) {
			Rotate (-90, turnTime);
		}
	}
}
