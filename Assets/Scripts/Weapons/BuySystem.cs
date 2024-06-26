using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuySystem : MonoBehaviour
{
    public static BuySystem Instance { get; private set; }
    public GameObject BuyScreenGO;
    [HideInInspector] public GameObject WeaponIns;
    [HideInInspector] public GameObject WeaponType;

    private void Awake()
    {
        Instance = this;
    }

    public void BuyWeapon(GameObject _WeaponType)
    {
        GameObject wpnType = WeaponSwitcher.Instance.GetItemThroughID(_WeaponType.GetComponent<WeaponSystem>().WeaponID);
        if(wpnType.GetComponent<WeaponSystem>().WeaponID == 0)
        {
            wpnType = Inventory.Instance.primaryWeapon;
        }
        else if(wpnType.GetComponent<WeaponSystem>().WeaponID == 1)
        {
            wpnType = Inventory.Instance.secondaryWeapon;
        }
        else if(wpnType.GetComponent <WeaponSystem>().WeaponID == 2)
        {
            wpnType = Inventory.Instance.meleeWeapon;
        }
        _WeaponType = wpnType;
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
    public GameObject GetWeaponFromInventory(WeaponSystem.WeaponType weaponType)
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

