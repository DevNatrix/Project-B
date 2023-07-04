using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherClient : MonoBehaviour
{
	public int ID;
	public string username;
	[SerializeField] Transform lookDirectionIndicatorTransform;
	public void setInfo(int _ID, string _username)
	{
		ID = _ID;
		username = _username;
	}

	public void setTransform(Vector3 position, Vector3 rotation)
	{
		lookDirectionIndicatorTransform.eulerAngles = rotation;
		transform.position = position;
	}
}
