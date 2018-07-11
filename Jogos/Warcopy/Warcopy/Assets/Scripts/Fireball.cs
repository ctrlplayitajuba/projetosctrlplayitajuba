using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour {
	[SerializeField] private float force = 500f;
	void OnCollisionEnter(Collision collision){
		if (!isServer) {
			return;
		}
		PlayerController pc = collision.gameObject.GetComponent<PlayerController> ();
		if (pc != null) {
			pc.TargetPush (pc.connectionToClient, force, this.transform.position);
			Destroy (gameObject);
		}
	}
}
