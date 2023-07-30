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
		ChangeHealth(-_damage);


		if (health <= 0)
        {
			Debug.Log("You Died");

			ChangeHealth(100, true);

			string[] sendData = { UDPServer.ID + "", health + ""};
			serverEvents.sendEvent("SetHealth", sendData);

			transform.position = new Vector3(0f, 10f, 0f);
        }
    }

	public void ChangeHealth(int _health, bool set = false)
	{
		if (set)
		{
			health = _health;
		}
		else
		{
			health += _health;
		}
		healthText.text = health.ToString();
	}
}
