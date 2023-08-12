using System.Collections;
using System.Collections.Generic;
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

	float lerpPercent = 0;
	float pastUpdateTime = 0;

	private void Update()
	{
		lerpPercent = (Time.time - pastUpdateTime) / (1 / (float)Client.transformTPS);

		//position
		transform.position = Vector3.Lerp(pastPosition, targetPosition, lerpPercent);

		//rotation
		Quaternion currentRotation = Quaternion.Slerp(pastRotation, targetRotation, lerpPercent);
		transform.rotation = Quaternion.Euler(new Vector3(0f, currentRotation.eulerAngles.y, 0f));
		lookIndicator.localRotation = Quaternion.Euler(new Vector3(currentRotation.eulerAngles.x, 0f, currentRotation.eulerAngles.z));

		//make info canvas face towards player cam
		infoCanvas.LookAt(playerCam);
	}

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

		pastUpdateTime = Time.time;
	}
}
