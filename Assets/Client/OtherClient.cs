using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class OtherClient : MonoBehaviour
{
	public int ID;
	public string username;
	[SerializeField] Transform lookIndicator;
	ServerEvents serverEvents;

	Vector3 pastPosition;
	Vector3 targetPosition;
	Vector3 pastRotation;
	Vector3 targetRotation;

	private void Start()
	{
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
	}

	public void setInfo(int _ID, string _username)
	{
		ID = _ID;
		username = _username;
	}

	public void setTransform(Vector3 position, Vector3 rotation)
	{
		pastPosition = targetPosition;
		targetPosition = position;

		pastRotation = targetRotation;
		targetRotation = rotation;
	}

	private void Update()
	{
		transform.position = Vector3.Lerp(pastPosition, targetPosition, serverEvents.lerpPercent);

		Vector3 currentRotation = Vector3.Slerp(pastRotation, targetRotation, serverEvents.lerpPercent);
		//body
		transform.rotation = Quaternion.Euler(new Vector3(0f, currentRotation.y, 0f));
		//look indicator
		lookIndicator.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, 0f, currentRotation.z));
	}
}
