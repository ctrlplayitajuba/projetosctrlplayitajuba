using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour {
	[SerializeField] private float force = 500f;
	[SerializeField] private float damage = 7.0f;
	void OnCollisionEnter(Collision collision){
		if (!isServer) {
			return;
		}
		HitPlayer (collision.gameObject);
		Destroy (gameObject);

	}
	void HitPlayer(GameObject ob){
		PlayerHealth ph = ob.gameObject.GetComponent<PlayerHealth> ();
		PlayerController pc = ob.gameObject.GetComponent<PlayerController> ();
		if (ph != null) {
			ph.TakeDamage (damage);
		}
		if (pc != null) {
			pc.TargetPush (pc.connectionToClient, force, this.transform.position);
		}
	}
}
