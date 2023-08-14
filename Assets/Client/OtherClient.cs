using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

	float yOffset = 0; //for making the client not visible

	public int team = 0;

	public Material friendlyMaterial;
	public Material opponentMaterial;

	public Renderer clientRenderer;

	public GameObject friendlyUI;

	public int health;
	int maxHealth = 100;
	public Slider slider;

	public float slideAngle;
	bool isSliding = false;
	public float rotSpeed;

	public void SetHealth(int _health)
	{
		health = _health;
		slider.value = (float)health / (float)maxHealth;
	}

	private void Update()
	{
		lerpPercent = (Time.time - pastUpdateTime) / (1 / (float)Client.transformTPS);

		//position
		transform.position = Vector3.Lerp(pastPosition, targetPosition, lerpPercent) + new Vector3(0f, yOffset, 0f);

		//rotation
		Quaternion currentRotation = Quaternion.Slerp(pastRotation, targetRotation, lerpPercent);
		transform.rotation = Quaternion.Euler(new Vector3(0f, currentRotation.eulerAngles.y, 0f));
		lookIndicator.localRotation = Quaternion.Euler(new Vector3(currentRotation.eulerAngles.x, 0f, currentRotation.eulerAngles.z));

		//make info canvas face towards player cam
		infoCanvas.LookAt(playerCam);

		float targetX;
		float currentX = transform.eulerAngles.x;
		if (isSliding)
		{
			Debug.Log("IS SLIDING");
			targetX = slideAngle;
		}
		else
		{
			Debug.Log("IS NOT SLIDING");
			targetX = 0;
		}

		transform.eulerAngles = new Vector3(Mathf.Lerp(currentX, targetX, rotSpeed * Time.deltaTime), transform.eulerAngles.y, transform.eulerAngles.z);
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

	public void setTransform(Vector3 position, Quaternion rotation, bool _isSliding)
	{
		pastPosition = targetPosition;
		targetPosition = position;

		pastRotation = targetRotation;
		targetRotation = rotation;

		pastUpdateTime = Time.time;
		isSliding = _isSliding;
	}

	public void setVisibility(bool visible)
	{
		if(visible)
		{
			yOffset = 0f;
		}
		else
		{
			yOffset = 1000f;
		}
	}

	public void setTeam(int _team)
	{
		team = _team;

		updateTeamThings(PlayerManager.team == team);
	}

	public void updateTeamThings(bool friendly)
	{
		if (friendly)
		{
			clientRenderer.material = friendlyMaterial;
			friendlyUI.SetActive(true);
		}
		else
		{
			clientRenderer.material = opponentMaterial;
			friendlyUI.SetActive(false);
		}
	}

	public void setEquippedWeapon(int newWeapon)
	{
		//weapons[equippedWeapon].SetActive(false);
		//weapons[newWeapon].SetActive(true);

		//equippedWeapon = newWeapon;
	}
}
