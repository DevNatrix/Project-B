using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] public Transform cam;

    float timeSinceLastShot;

    PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();

        cam = BuySystem.Instance.WeaponCam.transform;
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
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, weaponInfo.maxDistance))
                {
                    Debug.Log(hitInfo);
                }

                timeSinceLastShot = 0;
            }
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, cam.forward * weaponInfo.maxDistance);
    }
}