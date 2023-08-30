using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
	//I have so many settings here because I plan on making this customizable
	float accuracy;
	public float targetAccuracy;
	public float smoothness;
	public float distanceMult;
	public float startDistance;
	public float width;
	public float length;

	[SerializeField] Transform topPart;
	[SerializeField] Transform rightPart;
	[SerializeField] Transform bottomPart;
	[SerializeField] Transform leftPart;

	private void Start()
	{
		applySettings();
	}

	// Update is called once per frame
	void Update()
    {
        accuracy = Mathf.Lerp(accuracy, targetAccuracy, smoothness * Time.deltaTime);

		topPart.localPosition = new Vector3(0, startDistance + accuracy * distanceMult, 0);
		rightPart.localPosition = new Vector3(startDistance + accuracy * distanceMult, 0, 0);
		bottomPart.localPosition = new Vector3(0, -startDistance - accuracy * distanceMult, 0);
		leftPart.localPosition = new Vector3(-startDistance - accuracy * distanceMult, 0, 0);
	}

	public void applySettings()
	{
		Vector2 dimentions = new Vector2(length, width);
		topPart.GetComponent<RectTransform>().sizeDelta = dimentions;
		rightPart.GetComponent<RectTransform>().sizeDelta = dimentions;
		bottomPart.GetComponent<RectTransform>().sizeDelta = dimentions;
		leftPart.GetComponent<RectTransform>().sizeDelta = dimentions;
	}
}
