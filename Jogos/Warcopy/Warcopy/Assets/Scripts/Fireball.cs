using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
	[SerializeField] private float force = 500f;
	void OnCollisionEnter(Collision collision){
		Vector3 direction = collision.transform.position - this.transform.position;
		Destroy (gameObject);
		direction = direction.normalized;
		collision.rigidbody.AddExplosionForce (force, this.transform.position, 1f);
	}
}
