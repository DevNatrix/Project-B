using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    [HideInInspector] public static PickUpSystem Instance;
    PlayerControls playerControls;
    public float distanceCanPickup = 3f;
    private GameObject cam;

    [Header("StoreInfo")]
    [HideInInspector] public int WeaponID;
    [HideInInspector] public int AmmoInReserve;
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int maxAmmo;
    public GameObject WeaponType;

    private GameObject AmmoDisplayGOS;

	[HideInInspector] public int dropID;


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
                    if (WeaponSwitcher.Instance.weaponInventory[(int)gameObject.GetComponent<PickUpSystem>().WeaponType.GetComponent<WeaponSystem>().weaponType] == null)
                    {
                        GameObject newWeaponPickup = WeaponSwitcher.Instance.AddItem(WeaponType, WeaponType.GetComponent<WeaponSystem>().weaponType);
                        newWeaponPickup.SetActive(false);

                        WeaponSystem _WeaponTypeWP = newWeaponPickup.GetComponent<WeaponSystem>();

                        //Apply the stored info
                        _WeaponTypeWP.AmmoInReserve = AmmoInReserve;
                        _WeaponTypeWP.currentAmmo = currentAmmo;
                        _WeaponTypeWP.maxAmmo = maxAmmo;
                        _WeaponTypeWP.WeaponID = WeaponID;

                        if(WeaponSwitcher.Instance.currentSelectedWeapon == null)
                        {
                            WeaponSwitcher.Instance.SwitchWeapon((int)newWeaponPickup.GetComponent<WeaponSystem>().weaponType);
                        }

                        Destroy(gameObject);
                    }
                    else if (WeaponSwitcher.Instance.weaponInventory[(int)gameObject.GetComponent<PickUpSystem>().WeaponType.GetComponent<WeaponSystem>().weaponType] != null)
                    {
                        WeaponSwitcher.Instance.DropItem(WeaponSwitcher.Instance.weaponInventory[(int)gameObject.GetComponent<PickUpSystem>().WeaponType.GetComponent<WeaponSystem>().weaponType], gameObject.GetComponent<PickUpSystem>().WeaponType.GetComponent<WeaponSystem>().groundPrefab);
                        GameObject newWeaponPickup = WeaponSwitcher.Instance.AddItem(WeaponType, WeaponType.GetComponent<WeaponSystem>().weaponType);
                        newWeaponPickup.SetActive(false);

                        WeaponSystem _WeaponTypeWP = newWeaponPickup.GetComponent<WeaponSystem>();

                        //Apply the stored info
                        _WeaponTypeWP.AmmoInReserve = AmmoInReserve;
                        _WeaponTypeWP.currentAmmo = currentAmmo;
                        _WeaponTypeWP.maxAmmo = maxAmmo;
                        _WeaponTypeWP.WeaponID = WeaponID;

                        WeaponSwitcher.Instance.SwitchWeapon((int)newWeaponPickup.GetComponent<WeaponSystem>().weaponType);
                        Destroy(gameObject);
                    }
            }
        }
    }
}
