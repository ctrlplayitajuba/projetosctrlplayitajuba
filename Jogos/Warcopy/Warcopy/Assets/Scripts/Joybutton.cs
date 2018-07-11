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

	/// <summary>
	/// Ajusta o tempo de recarga para pressionar esse botão
	/// </summary>
	/// <param name="cooldown">Tempo de recarga</param>
	public void setCooldown(float cooldown){
		this.cooldown = cooldown;
	}

	/// <summary>
	/// Indica que o botão foi ou está pressionado
	/// </summary>
	/// <param name="eventData">Evento de pressionar o botão</param>
	public void OnPointerDown(PointerEventData eventData){
		if (onCooldown) {
			pressed = false;
		} else {
			pressed = true;
			onCooldown = true;
		}
	}

	/// <summary>
	/// Indica se o jogador deixou de pressionar o botão
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerUp(PointerEventData eventData){
		pressed = false;
	}

	/// <summary>
	/// Indica se o botão foi pressionado
	/// </summary>
	/// <returns><c>true</c> se o botão foi pressionado <c>false</c> se não foi</returns>
	public bool isPressed(){
		bool pressedValue = pressed;
		pressed = false;
		return pressedValue;
	}
}
