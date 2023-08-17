using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
		Invoke("initializeHealth", 1f);
    }

	void initializeHealth()
	{
		SetHealth(100);
	}

    public void TakeDamage(int damage, int attackerID)
	{
		SetHealth(health - damage, attackerID);
	}

	public void SetHealth(int _health, int attackerID = -1)
	{
		health = _health;

		if (health <= 0)
		{
			Debug.Log("You Died");
			Debug.Log(attackerID);
			OtherClient otherClient = serverEvents.getOtherClientScriptByID(attackerID);
			killFeed.createNewFeed(otherClient.username, Client.username);

			health = 100;

			respawn();
		}

		healthText.text = health.ToString();

		string[] sendData = { Client.ID + "", health + "" };
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
