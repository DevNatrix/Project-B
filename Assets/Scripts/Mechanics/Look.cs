using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] Transform playerCam;
    [SerializeField] Camera cam;
    [SerializeField] Rigidbody rb;

    [Header("Basic Settings:")]
    [SerializeField] float sensitivityX = 8f;
    [SerializeField] float sensitivityY = 0.5f;
    [SerializeField] float generalSense = .8f;
    [SerializeField] float xClamp = 85f;

    [Header("Smoothing Settings:")]
    [SerializeField] float xRotOffsetChangeSpeed;
    [SerializeField] float camHeightChangeSpeed;
    //[SerializeField] float FOVChangeSpeed = 1;
    [SerializeField] float camRollSpeed = 1;

    [HideInInspector] public float xRotOffset = 0;
	float yRotOffset = 0;
	float zRotOffset = 0;

    [HideInInspector] public float xRotation = 0;
	[HideInInspector] public float yRotation = 0;
    float zRotation = 0;

    float originalCamHeight;
    float mouseX, mouseY;

    //outside changes
    [HideInInspector] public float targetXRotOffset;
    [HideInInspector] public float camHeightOffset;
    [HideInInspector] public float targetFOV = 60f;
    [HideInInspector] public float targetCamRoll = 0;

    private void Update()
    {
		if (!MenuController.menu)
		{
			xRotOffset = Mathf.Lerp(xRotOffset, targetXRotOffset, xRotOffsetChangeSpeed * Time.deltaTime);
			yRotOffset = 0;
			zRotOffset = 0;

			//cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, FOVChangeSpeed * Time.deltaTime);
			cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, Mathf.Lerp(cam.transform.localPosition.y, originalCamHeight + camHeightOffset, camHeightChangeSpeed * Time.deltaTime), cam.transform.localPosition.z);
			//playerCam.eulerAngles = new Vector3(playerCam.eulerAngles.x, playerCam.eulerAngles.y, Mathf.Lerp(playerCam.eulerAngles.z, targetCamRoll, camRollSpeed * Time.deltaTime));


			rb.MoveRotation(Quaternion.Euler(Vector3.up * (yRotation + yRotOffset)));
			//transform.Rotate(Vector3.up, mouseX);

			xRotation -= mouseY;
			yRotation += mouseX;
			xRotation = Mathf.Clamp(xRotation - mouseY, -xClamp, xClamp);
			zRotation = Mathf.Lerp(zRotation, targetCamRoll, camRollSpeed * Time.deltaTime);
			Vector3 targetRotation = transform.eulerAngles;
			targetRotation.x = xRotation + xRotOffset;
			targetRotation.z = zRotation + zRotOffset;
			playerCam.eulerAngles = targetRotation;
		}
    }

    private void Awake()
    {
        originalCamHeight = cam.transform.localPosition.y;
    }

    public void ReceiveInput(Vector2 mouseInput)
    {
        mouseX = mouseInput.x * sensitivityX * generalSense;
        mouseY = mouseInput.y * sensitivityY * generalSense;
    }

    public IEnumerator changeFOVForSeconds(float FOVchange, float seconds)
    {
        targetFOV += FOVchange;
        yield return new WaitForSeconds(seconds);
        targetFOV -= FOVchange;

        yield return null;
    }
}
