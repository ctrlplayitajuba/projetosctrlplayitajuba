using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
	private float force = 20f;
	void OnCollisionEnter(Collision collision){
		Vector3 direction = collision.transform.position - this.transform.position;
		direction = direction.normalized;
		collision.rigidbody.AddForce (direction * force);
	}
}
