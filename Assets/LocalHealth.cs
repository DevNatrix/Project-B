using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalHealth : MonoBehaviour
{
    [HideInInspector] public static LocalHealth Instance;
    public int health;
    public TextMeshProUGUI healthText;

    private void Update()
    {
        healthText.text = health.ToString();
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;


        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("You killed something");
        }
    }
}
