using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour {
	[SerializeField] private float force = 500f;
	void OnCollisionEnter(Collision collision){
		CmdPush (collision.gameObject);
	}

	[Command]
	void CmdPush (GameObject ob){
		Destroy (gameObject);
		ob.GetComponent<Rigidbody>().AddExplosionForce (force, this.transform.position, 1f);
	}
}
