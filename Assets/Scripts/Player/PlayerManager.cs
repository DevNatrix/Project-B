using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public static int team;
	public List<Transform> spawnPoints;

	int health = 100;
	int maxHealth;
	public TextMeshProUGUI healthText;

	public ServerEvents serverEvents;
	public KillFeed killFeed;

	public int currentLife;
	public Movement movement;

	public void newSettings(string[] settings)
	{
		Debug.Log(ServerEvents.combineStringArray(settings, ", "));
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
		//movement.canWalljump = bool.Parse(settings[12]);
		movement.dashSpeed = float.Parse(settings[13]);
		movement.dashSeconds = float.Parse(settings[14]);
		movement.dashCooldown = float.Parse(settings[15]);
	}

	private void Start()
    {
		Invoke("initializeHealth", 1f);
    }

	void initializeHealth()
	{
		SetHealth(100);
	}

    public void TakeDamage(int damage, int attackedLife, int attackerID)
	{
		if(currentLife == attackedLife)
		{
			SetHealth(health - damage, attackerID);

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
			Debug.Log(attackerID);
			OtherClient otherClient = serverEvents.getOtherClientScriptByID(attackerID);
			killFeed.createNewFeed(otherClient.username, Client.username);

			health = 100;
			currentLife++;

			respawn();
		}

		healthText.text = health.ToString();

		string[] sendData = { Client.ID + "", health + "", currentLife + "" };
		serverEvents.sendEventToOtherClients("setHealth", sendData);
	}

	public void respawn()
	{
		transform.position = spawnPoints[team].position;
	}

	public void setTeam(int _team)
	{
		team = _team;
		Debug.Log("Joined team " + team);

		serverEvents.sendEventToOtherClients("setClientTeam", new string[] { Client.ID + "", team + "", "false" });
		
		respawn();
	}
}
