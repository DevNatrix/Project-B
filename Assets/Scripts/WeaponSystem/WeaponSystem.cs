using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WeaponData weaponData;

    PlayerControls playerControls;
    float timeSinceLastShot;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        if(playerControls.Weapon.Fire.IsPressed())
        {
            Shoot();
        }

        timeSinceLastShot += Time.deltaTime;
    }

    private bool CanShoot() => !weaponData.Reloading && timeSinceLastShot > 1f / (weaponData.FireRate / 60f);

    public void Shoot()
    {
        if(weaponData.CurrentAmmo > 0)
        {
            if(CanShoot())
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, weaponData.MaxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                }

                weaponData.CurrentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();

            }
        }
    }

    private void OnGunShot()
    {
        
    }
}
