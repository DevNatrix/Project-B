using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
	[Header("References:")]
	[SerializeField] Transform camTransform;

	[Header("Settings:")]
	[SerializeField] float ySensitivity;
	[SerializeField] float xSensitivity;
	[SerializeField] float generalSensitivity;

	float xRotation;
	[SerializeField] float xRotClamp;

	private void Start()
	{
		//lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * ySensitivity * generalSensitivity, 0f));

		xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * ySensitivity * generalSensitivity, -xRotClamp, xRotClamp);
		camTransform.localEulerAngles = new Vector3(xRotation, camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);
	}
}
