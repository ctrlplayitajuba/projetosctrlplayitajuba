using UnityEngine;
using UnityEngine.EventSystems;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {
	private bool pressed;
	private bool onCooldown;
	private float cooldown;
	private float time;

	void Start(){
		pressed = false;
		onCooldown = false;
		cooldown = 1;
		time = 0;
	}

	void Update(){
		if (onCooldown) {
			time += Time.deltaTime;
			if (time >= cooldown) {
				onCooldown = false;
			}
		} else {
			time = 0;
		}
	}

	public void setCooldown(float cooldown){
		this.cooldown = cooldown;
	}

	public void OnPointerDown(PointerEventData eventData){
		if (onCooldown) {
			pressed = false;
		} else {
			pressed = true;
			onCooldown = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData){
		pressed = false;
	}

	public bool isPressed(){
		bool pressedValue = pressed;
		pressed = false;
		return pressedValue;
	}
}
