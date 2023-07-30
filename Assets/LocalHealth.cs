using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHealth : MonoBehaviour
{
    public int health;
    public TextMeshProUGUI healthText;
	public ServerEvents serverEvents;

    public void TakeDamage(int _damage)
    {
		Debug.Log("You got damaged");
        health -= _damage;
		healthText.text = health.ToString();


		if (health <= 0)
        {
			Debug.Log("You Died");

			health = 100;

			string[] sendData = { UDPServer.ID + "", health + "" };
			serverEvents.sendEvent("SetHealth", sendData);

			transform.position = new Vector3(0f, 10f, 0f);
        }
    }
}
