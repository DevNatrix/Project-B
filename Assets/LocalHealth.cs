using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHealth : MonoBehaviour
{
    [HideInInspector] public static LocalHealth Instance;
    public int health;
    public TextMeshProUGUI healthText;
	public ServerEvents serverEvents;

    private void Update()
    {
        healthText.text = health.ToString();
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;


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
