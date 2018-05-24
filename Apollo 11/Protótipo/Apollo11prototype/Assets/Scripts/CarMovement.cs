using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

	[SerializeField] private float speed = 1.0f;

	public void Move (float time){
		iTween.MoveBy(gameObject, iTween.Hash(
										"x"   , time*speed, 
									  	"time", time,
										"easetype", iTween.EaseType.linear));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
				Move (1);
	}
}
