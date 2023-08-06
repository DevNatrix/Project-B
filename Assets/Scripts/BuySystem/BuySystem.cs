using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuySystem : MonoBehaviour
{
    public static BuySystem Instance { get; private set; }
    PlayerControls playerControls;
    public GameObject BuyScreenGO;
    bool BuyScreenVisual = false;
    [HideInInspector] public GameObject WeaponIns;
    [HideInInspector] public GameObject WeaponType;

    private void Awake()
    {
        playerControls = new PlayerControls();

        Instance = this;
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
        BuyScreen();
    }

    void BuyScreen()
    {
        if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && !BuyScreenVisual)
        {
            BuyScreenGO.SetActive(true);
            BuyScreenVisual = true;
			Cursor.lockState = CursorLockMode.None;
		}
        else if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && BuyScreenVisual)
        {
			Cursor.lockState = CursorLockMode.Locked;
            BuyScreenGO.SetActive(false);
            BuyScreenVisual = false;
        }
    }

    public void BuyWeapon(GameObject _WeaponType)
    {
        WeaponSystem.WeaponType currentWeaponType = _WeaponType.GetComponent<WeaponSystem>().weaponType;
        Debug.Log($"Bought {_WeaponType}");

        // Check if the item already exists in the inventory
        GameObject existingWeapon = GetWeaponFromInventory(currentWeaponType);

        if (existingWeapon != null)
        {
            // If the weapon already exists in the inventory, drop the newly purchased weapon
            GameObject weaponGO = Instantiate(_WeaponType, transform);
            DropWeapon(weaponGO);
        }
        else
        {
            // If the weapon does not exist in the inventory, add it to the inventory
            GameObject purchasedWeapon = WeaponSwitcher.Instance.AddItem(_WeaponType, currentWeaponType);
            WeaponSwitcher.Instance.SwitchWeapon((int)purchasedWeapon.GetComponent<WeaponSystem>().weaponType);
        }
    }

    // Function to check if a weapon with the given weaponType exists in the inventory
    private GameObject GetWeaponFromInventory(WeaponSystem.WeaponType weaponType)
    {
        // Iterate through the weaponInventory to find a match
        foreach (GameObject item in WeaponSwitcher.Instance.weaponInventory)
        {
            // Check if the item is not null and has the WeaponSystem component
            if (item != null && item.GetComponent<WeaponSystem>() != null && item.GetComponent<WeaponSystem>().weaponType == weaponType)
            {
                return item; // Return the reference to the existing weapon in the inventory
            }
        }
        return null; // Weapon not found in the inventory
    }

    // Function to drop the newly purchased weapon
    private void DropWeapon(GameObject _WeaponType)
    {
        Debug.Log("Dropping the newly purchased weapon because you already have this weapon type in the inventory: " + _WeaponType.name);

        WeaponSwitcher.Instance.DropItem(_WeaponType, _WeaponType.GetComponent<WeaponSystem>().groundPrefab);
    }

}

