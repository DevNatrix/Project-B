using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    public int health;

    public void TakeDamage(int _damage)
    {
        health -= _damage;
    }
	public void SetHealth(int _health)
    {
        health = _health;
    }
}
