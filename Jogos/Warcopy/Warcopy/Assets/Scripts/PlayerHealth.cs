using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {
	[SerializeField] private float maxHealth = 100.0f;
	[SyncVar(hook = "OnChangeHealth")]
	private float currentHealth;
	public RectTransform healthBar;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeDamage(float damage){
		currentHealth -= damage;
	}

	void OnChangeHealth(float health){
		healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
	}
}
