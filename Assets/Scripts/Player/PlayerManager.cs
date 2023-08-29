 using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerManager : MonoBehaviour
{
	public static int team;
	public List<Transform> spawnPoints;

	int health = 100;
	int maxHealth = 100;
	public TextMeshProUGUI healthText;

	public ServerEvents serverEvents;
	public KillFeed killFeed;

	public int currentLife;
	public Movement movement;

	public MenuController menuController;

	public PostProcessVolume postVolume;
	Vignette vignette;
	public float bloodFadeSpeed;
	public float hitBloodIntensity;


	public Transform leftHand;
	public Transform rightHand;

	[HideInInspector] public Transform leftHandTarget;
	[HideInInspector] public Transform rightHandTarget;

	public Transform weaponContainer;
	public Transform rightShoulder;
	public Transform weaponHolder;

	public Transform playerCam;

	public float xRotRecoil;
	public float yRotRecoil;
	public LayerMask aimMask;

	public Vector3 aimPointOffset;

	public int coins = 0;
	public float coinsGivenOnDeath = 1;
	public TextMeshProUGUI coinText;

	void Start()
	{
		Invoke("initializeHealth", 1f);
		postVolume.profile.TryGetSettings(out vignette);
	}

	public void addCoins(int _coins)
	{
		coins += _coins;
		coinText.text = "$" + coins;
	}

	private void Update()
	{
		weaponContainer.position = rightShoulder.position;

		RaycastHit hit;
		Physics.Raycast(playerCam.position, playerCam.forward, out hit, Mathf.Infinity, aimMask);
		weaponContainer.LookAt(hit.point - aimPointOffset);
		weaponContainer.rotation *= Quaternion.Euler(new Vector3(xRotRecoil, yRotRecoil, 0));

		if (leftHandTarget != null)
		{
			leftHand.position = leftHandTarget.position;
			leftHand.rotation = leftHandTarget.rotation;

			rightHand.position = rightHandTarget.position;
			rightHand.rotation = rightHandTarget.rotation;
		}

		vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, bloodFadeSpeed * Time.deltaTime);
	}

	public void newSettings(string[] settings)
	{
		maxHealth = int.Parse(settings[1]);
		movement.acceleration = float.Parse(settings[2]);
		movement.inAirAcceleration = float.Parse(settings[3]);
		movement.decceleration = float.Parse(settings[4]);
		movement.inAirDecceleration = float.Parse(settings[5]);
		movement.slidingDecceleration = float.Parse(settings[6]);
		movement.speedBoostOnSlide = float.Parse(settings[7]);
		movement.jumpPower = float.Parse(settings[8]);
		movement.maxSpeed = float.Parse(settings[9]);
		movement.minYPos = float.Parse(settings[10]);
		movement.addedGravity = float.Parse(settings[11]);
		movement.dashSpeed = float.Parse(settings[13]);
		movement.dashSeconds = float.Parse(settings[14]);
		movement.dashCooldown = float.Parse(settings[15]);
	}

	void initializeHealth()
	{
		SetHealth(maxHealth);
	}

    public void TakeDamage(int damage, int attackedLife, int attackerID)
	{
		if(currentLife == attackedLife)
		{
			SetHealth(health - damage, attackerID);
			vignette.intensity.value = hitBloodIntensity;
			//set current life
		}
		else
		{
			string[] sendData = { Client.ID + "", health + "", currentLife + "" };
			serverEvents.sendDirectEvent("setHealth", sendData, attackerID);
			Debug.LogWarning("Client " + attackerID + " did damage in the past life " + attackedLife + " (current life is " + currentLife + ")");
		}
	}

	public void SetHealth(int _health, int attackerID = -1)
	{
		health = _health;

		if (health <= 0)
		{
			Death(_health, attackerID);
		}
	}

	public void respawn()
	{
		transform.position = spawnPoints[team].position;
		SetHealth((int)(maxHealth * AbilityManager.Instance.getMultiplier("health")));
	}

	public void setTeam(int _team)
	{
		team = _team;
		Debug.Log("Joined team " + team);

		serverEvents.sendEventToOtherClients("setClientTeam", new string[] { Client.ID + "", team + "", "false" });
		
		respawn();
	}

	public void Death(int health, int attackerID)
	{
		if (attackerID != -1)
		{
			OtherClient otherClient = serverEvents.getOtherClientScriptByID(attackerID);
			killFeed.createNewFeed(otherClient.username, Client.username);

			serverEvents.sendDirectEvent("changeCoins", new string[] { coinsGivenOnDeath + "" }, attackerID);
		}

		currentLife++;
		menuController.triggerDeathMenu();

		foreach (GameObject weapon in WeaponSwitcher.Instance.weaponInventory)
		{
			if (weapon.GetComponent<WeaponSystem>() != null)
			{
				weapon.GetComponent<WeaponSystem>().currentAmmo = WeaponSystem.Instance.crntAmmoReset;
			}
		}


		healthText.text = health.ToString();

		string[] sendData = { Client.ID + "", health + "", currentLife + "" };
		serverEvents.sendEventToOtherClients("setHealth", sendData);
	}
}
