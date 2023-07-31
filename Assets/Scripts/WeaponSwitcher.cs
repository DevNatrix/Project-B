using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    public static WeaponSwitcher Instance;

    PlayerControls playerControls;
    public bool switchonScroll = true;
    private Animator anim;
    [HideInInspector] public GameObject currentSelectedWeapon;

    [SerializeField] private WeaponSystem[] weaponInventory;

    [Header("UI Weapon")]
    private GameObject AmmoDisplayGOS;

    private void Awake()
    {
        playerControls = new PlayerControls();
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

    private void Start()
    {
        SetVariables();
    }


    void Update()
    {
      
    }

    public void AddItem(WeaponSystem newItem)
    {
        int newItemIndex = (int)newItem.weaponType;

        if (weaponInventory[newItemIndex] != null)
        {
            RemoveItem(newItemIndex);
        }
        weaponInventory[newItemIndex] = newItem;
    }

    public void RemoveItem(int index)
    {
        weaponInventory[index] = null;
    }

    public WeaponSystem GetItem(int index)
    {
        return weaponInventory[index];
    }

    public void SetVariables()
    {
        weaponInventory = new WeaponSystem[3];
    }
}
