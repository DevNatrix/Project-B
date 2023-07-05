using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherClient : MonoBehaviour
{
	public int ID;
	public string username;
	[SerializeField] Transform lookDirectionIndicatorTransform;

	Vector3 pastPosition;
	Vector3 targetPosition;
	Vector3 pastRotation;
	Vector3 targetrotation;
	float lerpTime;
	float pastUpdateTime;


	public void setInfo(int _ID, string _username)
	{
		ID = _ID;
		username = _username;
	}

	public void setTransform(Vector3 position, Vector3 rotation)
	{
		pastPosition = targetPosition;
		targetPosition = position;

		pastRotation = targetrotation;
		targetrotation = rotation;

		lerpTime = Time.time - pastUpdateTime;

		pastUpdateTime = Time.time;
	}

	private void Update()
	{
		float percentOfLerp = Mathf.Clamp((Time.time - pastUpdateTime) / lerpTime, 0, 1);
		transform.position = Vector3.Lerp(pastPosition, targetPosition, percentOfLerp);
	}
}
