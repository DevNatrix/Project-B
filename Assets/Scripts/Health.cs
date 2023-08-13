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

	public void SetHealth(int _health)
    {
        health = _health;
		slider.value = (float)health / (float)maxHealth;
	}
}
