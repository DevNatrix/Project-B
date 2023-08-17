using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class OtherClient : MonoBehaviour
{
	public int ID;
	public string username;
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
	public float xRot = 0f;

	public GameObject currentWeapon;

	[HideInInspector] public Vector3 direction;

	public List<GameObject> weapons;

	[SerializeField] Transform leftTarget;
	[SerializeField] Transform rightTarget;

	[SerializeField] Transform weaponLeftTarget;
	[SerializeField] Transform weaponRightTarget;

	public void SetHealth(int _health)
	{
		health = _health;
		slider.value = (float)health / (float)maxHealth;
	}

	private void Update()
	{
		//set targets to hand positions in guns
		leftTarget.position = weaponLeftTarget.position;
		rightTarget.position = weaponRightTarget.position;
		
		leftTarget.rotation = weaponLeftTarget.rotation;
		rightTarget.rotation = weaponRightTarget.rotation;



		lerpPercent = (Time.time - pastUpdateTime) / (1 / (float)Client.transformTPS);

		//position
		transform.position = Vector3.Lerp(pastPosition, targetPosition, lerpPercent) + new Vector3(0f, yOffset, 0f);

		//rotation
		Quaternion currentRotation = Quaternion.Slerp(pastRotation, targetRotation, lerpPercent);
		transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, currentRotation.eulerAngles.y, 0f));

		//make info canvas face towards player cam
		infoCanvas.LookAt(playerCam);

		float targetX;
		if (isSliding)
		{
			targetX = slideAngle;
		}
		else
		{
			targetX = 0;
		}

		xRot = Mathf.Lerp(xRot, targetX, rotSpeed * Time.deltaTime);

		transform.rotation = Quaternion.Euler(new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z));
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

		direction = pastPosition - targetPosition;
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
		Debug.Log("Client " + ID + " equipped weapon " + newWeapon);

		//set only new weapon active
		currentWeapon.SetActive(false);
		currentWeapon = weapons[newWeapon];
		currentWeapon.SetActive(true);

		OtherClientWeapon currentWeaponScript = currentWeapon.GetComponent<OtherClientWeapon>();
		weaponLeftTarget = currentWeaponScript.leftHandPos;
		weaponRightTarget = currentWeaponScript.rightHandPos;
	}
}
