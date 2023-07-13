using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using UnityEngine;

public class OtherClient : MonoBehaviour
{
	public int ID;
	public string username;
	[SerializeField] Transform lookIndicator;
	[SerializeField] Transform infoCanvas;
	[SerializeField] TextMeshProUGUI usernameText;
	Transform playerCam;
	ServerEvents serverEvents;

	Vector3 pastPosition;
	Vector3 targetPosition;
	Quaternion pastRotation;
	Quaternion targetRotation;

	public int health = 200;

	private void Start()
	{
		playerCam = GameObject.Find("Main Camera").transform;
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
	}

	public void setInfo(int _ID, string _username)
	{
		ID = _ID;
		username = _username;
		usernameText.text = username;
	}

	public void setTransform(Vector3 position, Quaternion rotation)
	{
		pastPosition = targetPosition;
		targetPosition = position;

		pastRotation = targetRotation;
		targetRotation = rotation;
	}

	public void TakeDamage(int _damage)
	{
		health -= _damage;

		if (health <= 0)
		{
			Destroy(gameObject);
			Debug.Log("You killed something");
		}
	}

	private void Update()
	{
		//position
		transform.position = Vector3.Lerp(pastPosition, targetPosition, serverEvents.lerpPercent);

		//rotation
		Quaternion currentRotation = Quaternion.Slerp(pastRotation, targetRotation, serverEvents.lerpPercent);
		transform.rotation = Quaternion.Euler(new Vector3(0f, currentRotation.eulerAngles.y, 0f));
		lookIndicator.localRotation = Quaternion.Euler(new Vector3(currentRotation.eulerAngles.x, 0f, currentRotation.eulerAngles.z));

		//make info canvas face towards player cam
		infoCanvas.LookAt(playerCam);
	}
}
