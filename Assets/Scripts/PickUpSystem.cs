using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    [HideInInspector] public static PickUpSystem Instance;
    PlayerControls playerControls;
    public float distanceCanPickup = 100f;
    private GameObject cam;

    [Header("StoreInfo")]
    [HideInInspector] public static string WeaponID;
    [HideInInspector] public static int AmmoInReserve;
    [HideInInspector] public static int currentAmmo;
    [HideInInspector] public static int maxAmmo;
    public GameObject WeaponType;

    private GameObject AmmoDisplayGOS;

    void Awake()
    {
        playerControls = new PlayerControls();

        cam = GameReferences.Instance.MainCam;
        Instance = this;
        AmmoDisplayGOS = GameReferences.Instance.AmmoDisplayGO;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        if(playerControls.Weapon.Interact.WasPressedThisFrame())
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        //Shoot raycast
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, distanceCanPickup))
        {
            if(hit.transform.gameObject == gameObject)
            {
                GameObject PickUpGun = Instantiate(WeaponType, GameReferences.Instance.weaponHolder.transform);
                WeaponSystem PickUpGunInfo = PickUpGun.GetComponent<WeaponSystem>();

                //Apply those stored info
                PickUpGunInfo.currentAmmo = currentAmmo;
                PickUpGunInfo.maxAmmo = maxAmmo;
                PickUpGunInfo.AmmoInReserve = AmmoInReserve;
                PickUpGunInfo.WeaponID = WeaponID;


                AmmoDisplayGOS.SetActive(true);
                WeaponSwitcher.Instance.currentSelectedWeapon.SetActive(false);


                Destroy(gameObject);
            }
        }
    }
}
