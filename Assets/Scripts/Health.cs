using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Health : MonoBehaviour
{
    public int health;
    int maxHealth = 100;
	public Slider slider;

    public void TakeDamage(int _damage)
    {
		Debug.Log("Other client took " +  _damage + " damage");
        health -= _damage;
		slider.value = (float)health/(float)maxHealth;
    }
	public void SetHealth(int _health)
    {
        health = _health;
		slider.value = (float)health / (float)maxHealth;
	}
}
