using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private WeaponInfo weaponInfo;
    [HideInInspector] public Transform cam;
    [HideInInspector] private GameObject eyesDirection;

    float timeSinceLastShot;

    PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();

        cam = BuySystem.Instance.WeaponCam.transform;
        eyesDirection = BuySystem.Instance.camDir;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        weaponInfo.reloading = false;
    }

    public void StartReload()
    {
        if (!weaponInfo.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        weaponInfo.reloading = true;

        yield return new WaitForSeconds(weaponInfo.reloadTime);

        weaponInfo.startAmmo = weaponInfo.magSize;

        weaponInfo.reloading = false;
    }

    private bool CanShoot() => !weaponInfo.reloading && timeSinceLastShot > 1f / (weaponInfo.fireRate / 60f);

    private void Shoot()
    {
        if (weaponInfo.startAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(cam.position, eyesDirection.transform.forward, out RaycastHit hitInfo, weaponInfo.maxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                }

                timeSinceLastShot = 0;
            }
        }
    }

    private void Update()
    {
        if(playerControls.Weapon.Fire.IsPressed())
        {
            Shoot();
        }

        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, eyesDirection.transform.forward * weaponInfo.maxDistance);
    }
}