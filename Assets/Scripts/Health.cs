using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [HideInInspector] public static Health Instance;
    public int health;

    public void TakeDamage(int _damage)
    {
        health -= _damage;

        if(health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("You killed something");
        }
    }
}
