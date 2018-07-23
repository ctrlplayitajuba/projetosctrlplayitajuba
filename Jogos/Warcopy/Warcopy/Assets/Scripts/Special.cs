using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Special : NetworkBehaviour {



	// Use this for initialization
	void Start () {

		Collider[] explosionColliders = Physics.OverlapSphere (this.transform.position, 10f);

		foreach (Collider co in explosionColliders) {
			if(co.gameObject.GetComponent<PlayerController>() != null)
			{
				Debug.Log ("SpecialHit target push a partir do special hit");
				co.gameObject.GetComponent<PlayerController> ().TargetPush (co.gameObject.GetComponent<PlayerController> ().connectionToClient, 1000f, this.transform.position, 10f);
			}
		}
	}
}
