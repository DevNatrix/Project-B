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
	public Crosshair crosshair;

	public Vector3 aimPointOffset;

	[SerializeField] GameManager gameManager;

	public float timeBeforeRegen;
	float regenTimer;

	void Start()
	{
		Invoke("initializeHealth", 1f);
		InvokeRepeating("Regeneration", 1f, .1f);
		postVolume.profile.TryGetSettings(out vignette);
	}

	void Regeneration()
	{
		if(regenTimer < 0f && !GameManager.dead)
		{
			int moddedMaxHealth = maxHealth + Upgrades.instance.healthPerLevel * Upgrades.instance.getUpgradeLevel("Health");
			int newHealth = Mathf.Clamp(health + Upgrades.instance.regenPerUpgrade * Upgrades.instance.getUpgradeLevel("Regen"), 0, moddedMaxHealth);
			SetHealth(newHealth);
		}
	}

	private void Update()
	{
		regenTimer -= Time.deltaTime;
		weaponContainer.position = rightShoulder.position;

		RaycastHit hit;
		Physics.Raycast(playerCam.position, playerCam.forward, out hit, Mathf.Infinity, aimMask);
		Vector3 camPoint = hit.point;

		weaponContainer.LookAt(camPoint);
		weaponContainer.rotation *= Quaternion.Euler(new Vector3(xRotRecoil, yRotRecoil, 0));

		//get accuracy
		//Physics.Raycast(weaponContainer.position, weaponContainer.forward, out hit, Mathf.Infinity, aimMask);
		float accuracy = Mathf.Abs(xRotRecoil) + Mathf.Abs(yRotRecoil);//Vector3.Distance(camPoint, hit.point);
		crosshair.targetAccuracy = accuracy;

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

    public void TakeDamage(int damage, int attackerID)
	{
		if (!GameManager.dead)
		{
			SetHealth(health - damage, attackerID);
			vignette.intensity.value = hitBloodIntensity;
			regenTimer = timeBeforeRegen;
		}
	}

	public void SetHealth(int _health, int attackerID = -1)
	{
		health = _health;

		if (health <= 0)
		{
			Death(attackerID);
		}

		healthText.text = health + "";
		serverEvents.sendEventToOtherClients("setHealth", new string[] {Client.ID + "", health + "" });
	}

	public void respawn()
	{
		transform.position = spawnPoints[team].position;
		SetHealth(maxHealth + Upgrades.instance.healthPerLevel * Upgrades.instance.getUpgradeLevel("Health"));
		menuController.spawn();
		regenTimer = 0;

		reloadAllGuns();
	}

	public void setTeam(int _team)
	{
		team = _team;

		serverEvents.sendEventToOtherClients("setClientTeam", new string[] { Client.ID + "", team + "", "false" });
	}

	public void Death(int attackerID = -1)
	{
		if (attackerID != -1)
		{
			OtherClient otherClient = serverEvents.getOtherClientScriptByID(attackerID);
			killFeed.createNewFeed(otherClient.username, Client.username);
		}

		healthText.text = health.ToString();
		menuController.death();

		serverEvents.sendEventToOtherClients("otherPlayerDied", new string[] { Client.ID + "" });
		gameManager.checkMatchStatus();
	}

	public void reloadAllGuns()
	{
		foreach (GameObject weapon in WeaponSwitcher.Instance.weaponInventory)
		{
			if (weapon != null && weapon.GetComponent<WeaponSystem>() != null)
			{
				weapon.GetComponent<WeaponSystem>().currentAmmo = weapon.GetComponent<WeaponSystem>().crntAmmoReset;
				weapon.GetComponent<WeaponSystem>().AmmoInReserve = weapon.GetComponent<WeaponSystem>().ammoReserve;
			}
		}
	}
}
