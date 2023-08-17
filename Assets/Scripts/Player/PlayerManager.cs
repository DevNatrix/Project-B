using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public static int team;
	public List<Transform> spawnPoints;

	public int health;
	public TextMeshProUGUI healthText;

	public ServerEvents serverEvents;
	public KillFeed killFeed;

	public int currentLife;

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
		}
		else
		{
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
