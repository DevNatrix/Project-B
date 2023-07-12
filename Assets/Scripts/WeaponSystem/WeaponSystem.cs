using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{

    [Header("References")]
    [HideInInspector] public static WeaponSystem Instance;
    [SerializeField] private WeaponInfo weaponInfo;
    [HideInInspector] public Transform cam;
    [HideInInspector] public GameObject eyesDirection;
    [SerializeField] public GameObject WeaponModel;
	[SerializeField] public ServerEvents serverEvents;
	[SerializeField] public UDPServer uDPServer;
    [HideInInspector] public GameObject WeaponModelClone;

    float timeSinceLastShot;

    PlayerControls playerControls;

    private void Start()
    {
        Instance = this;
    }

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
        CheckForInput();

        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, eyesDirection.transform.forward * weaponInfo.maxDistance);
    }

    void CheckForInput()
    {
        if(playerControls.Weapon.Fire.IsPressed())
        {
            Shoot();
        }

        if(playerControls.Weapon.Drop.WasPerformedThisFrame())
        {
            DropCurrentWeapon();
        }
    }

    private void DropCurrentWeapon()
    {
        BuySystem.Instance.WeaponIns.SetActive(false);
        WeaponModelClone = Instantiate(WeaponModel, cam.transform.position, default);
	}
}