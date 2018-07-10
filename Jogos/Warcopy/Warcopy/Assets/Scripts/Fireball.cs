using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour {
	void OnCollisionEnter(Collision collision){
		PlayerController pc = collision.gameObject.GetComponent<PlayerController> ();
		if (pc != null) {
			this.GetComponent<NetworkIdentity> ().AssignClientAuthority (collision.gameObject.GetComponent<NetworkIdentity> ().connectionToClient);
			pc.CmdPush (1700.0f, collision.transform.position, 1.5f);
		}
		Destroy (gameObject);
	}
}
