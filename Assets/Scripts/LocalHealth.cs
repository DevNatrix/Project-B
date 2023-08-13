using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHealth : MonoBehaviour
{
    public int health;
    public TextMeshProUGUI healthText;
	public ServerEvents serverEvents;

	public void TakeDamage(int damage)
    {
		Debug.Log("You got damaged: " + damage);
		SetHealth(health - damage);
    }

	public void SetHealth(int _health)
	{
		health = _health;

		if (health <= 0)
		{
			Debug.Log("You Died");

			health = 100;

			transform.position = new Vector3(0f, 10f, 0f);
		}

		healthText.text = health.ToString();

		string[] sendData = { Client.ID + "", health + "" };
		serverEvents.sendEventToOtherClients("setHealth", sendData);
	}
}
