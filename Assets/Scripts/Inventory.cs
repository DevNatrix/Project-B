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

    void Start()
    {
        InventoryVisual.SetActive(false);
        primaryWeapon = defaultPrimary;
        secondaryWeapon = defaultSecondary;
        meleeWeapon = defaultMelee;
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
    }

    public void SelectSecondary(GameObject _SecondaryPrefab)
    {
        secondaryWeapon = _SecondaryPrefab;
    }

    public void SelectMelee(GameObject _MeleePrefab)
    {
        meleeWeapon = _MeleePrefab;
    }
}
