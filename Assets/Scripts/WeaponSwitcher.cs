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

    [SerializeField] private GameObject[] weaponInventory;

    [Header("UI Weapon")]
    private GameObject AmmoDisplayGOS;

    [Header("Info")]
    public float pickupDistance = 10f;

    public Camera cam;

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
        weaponInventory = new GameObject[3];
    }


    void Update()
    {
        EquipWeapon();
    }

    public void AddItem(GameObject weaponPrefab, WeaponSystem.WeaponType weaponType)
    {
        int newItemIndex = (int)weaponType;

        if (weaponInventory[newItemIndex] != null)
        {
            RemoveItem(newItemIndex);
        }

        weaponInventory[newItemIndex] = weaponPrefab;

        // Instantiate the gun's GameObject (visuals) and parent it to the weapon switcher game object
        GameObject weaponGO = Instantiate(weaponPrefab, transform);
        weaponInventory[newItemIndex] = weaponGO;
    }

    public void RemoveItem(int index)
    {
        weaponInventory[index] = null;
    }

    public void EquipWeapon()
    {
        if(playerControls.Weapon.PrimaryWeapon.WasPerformedThisFrame())
        {
            SwitchWeapon(0);
        }

        if (playerControls.Weapon.SecondaryWeapon.WasPerformedThisFrame())
        {
            SwitchWeapon(1);
        }

        if (playerControls.Weapon.Knife.WasPerformedThisFrame())
        {
            SwitchWeapon(2);
        }
    }

    public void SwitchWeapon(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weaponInventory.Length && weaponInventory[newIndex] != null)
        {
            if (currentSelectedWeapon != null)
            {
                currentSelectedWeapon.SetActive(false); // Unequip the previous weapon
            }

            currentSelectedWeapon = weaponInventory[newIndex];
            currentSelectedWeapon.SetActive(true); // Equip the new weapon

            WeaponSystem currentWeaponSystem = currentSelectedWeapon.GetComponent<WeaponSystem>();
            if (currentWeaponSystem != null)
            {
                AmmoDisplayGOS.GetComponent<TextMeshProUGUI>().text = currentWeaponSystem.currentAmmo.ToString();
            }
        }
    }
}