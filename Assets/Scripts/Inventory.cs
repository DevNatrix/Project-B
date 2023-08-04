using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public static Inventory Instance;

    public GameObject InventoryVisual;
    [HideInInspector] public GameObject primaryWeapon;
    [HideInInspector] public GameObject secondaryWeapon;
    [HideInInspector] public GameObject meleeWeapon;
    [SerializeField] private GameObject defaultPrimary;
    [SerializeField] private GameObject defaultSecondary;
    [SerializeField] private GameObject defaultMelee;
    [SerializeField] private GameObject weaponSlotVisual; // Prefab for visual representation of selected weapon

    private GameObject weaponSlotInstance;

    void Start()
    {
        InventoryVisual.SetActive(false);
        primaryWeapon = defaultPrimary;
        secondaryWeapon = defaultSecondary;
        meleeWeapon = defaultMelee;
        CreateWeaponSlotVisual();
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInventory()
    {
        InventoryVisual.SetActive(true);
    }

    public void ExitInventory()
    {
        InventoryVisual.SetActive(false);
    }

    public void SelectPrimary(GameObject _PrimaryPrefab)
    {
        primaryWeapon = _PrimaryPrefab;
        UpdateWeaponSlotVisual();
    }

    public void SelectSecondary(GameObject _SecondaryPrefab)
    {
        secondaryWeapon = _SecondaryPrefab;
        UpdateWeaponSlotVisual();
    }

    public void SelectMelee(GameObject _MeleePrefab)
    {
        meleeWeapon = _MeleePrefab;
        UpdateWeaponSlotVisual();
    }

    //Function for inspecting weapons
    private void CreateWeaponSlotVisual()
    {
        weaponSlotInstance = Instantiate(weaponSlotVisual, Vector3.zero, Quaternion.identity);
        weaponSlotInstance.transform.SetParent(transform); // Set the weapon slot visual as a child of the inventory
        weaponSlotInstance.SetActive(false); // Hide the weapon slot visual by default
    }

    private void UpdateWeaponSlotVisual()
    {
        if (weaponSlotInstance != null)
        {
            GameObject selectedWeaponPrefab = primaryWeapon; // Update this based on the currently selected weapon (primary, secondary, or melee)
            // You can set the position and rotation of the weaponSlotInstance here based on your UI layout
            // For example, if you have a 2D UI, you might set the position relative to the screen, or if it's a 3D UI, you might set it to a specific location on the player's hand.
            weaponSlotInstance.SetActive(true); // Show the weapon slot visual
        }
    }
}
