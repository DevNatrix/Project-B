using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("References")]
    private GameObject cam;
    public WeaponInfo weaponInfo;
    public Animator anim;

    [Header("Info")]
    public string WeaponName;
    public string WeaponID;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;

    [Header("Gun Properties")]
    public int currentAmmo;
    public int magSize;
    public int mags;
    public float fireRate;
    public float reloadTime;

    [HideInInspector] public bool reloading = false;
    private float TimeSinceLastShot;

    private void Awake()
    {
        playerControls = new PlayerControls();

        cam = GameReferences.Instance.MainCam;

    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Shoot();
        Reload();
        Debug.DrawRay(cam.transform.position, cam.transform.forward);
    }

    private void Shoot()
    {
        if(currentAmmo > 0)
        {
            if (CanShoot())
            {
                if (playerControls.Weapon.Fire.IsPressed() && Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, weaponInfo.maxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                    currentAmmo--;
                    TimeSinceLastShot = 0;
                }
            }
        }
    }

    void Reload()
    {
        if(playerControls.Weapon.Reload.WasPressedThisFrame())
        {
            anim.Play("ReloadAnim");
            anim.SetBool("Reloading", true);
        }
    }

    public void SetReloadFalse()
    {
        //Reload Weapon
        anim.SetBool("Reloading", false);
    }

    private bool CanShoot() => !reloading && TimeSinceLastShot > 1f / (fireRate / 60f);
}
