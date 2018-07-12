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
		CmdPushPlayers (collision.gameObject);
		Destroy (gameObject);

	}
	[Command]
	void CmdPushPlayers(GameObject ob){
		PlayerController pc = ob.gameObject.GetComponent<PlayerController> ();
		if (pc != null) {
			pc.TargetPush (pc.connectionToClient, force, this.transform.position);
		}
	}
}
