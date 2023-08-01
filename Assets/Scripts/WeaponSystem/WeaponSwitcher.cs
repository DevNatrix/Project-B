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
    public GameObject currentSelectedWeapon;

    [SerializeField] private GameObject[] weaponInventory;

    [Header("UI Weapon")]
    private GameObject AmmoDisplayGOS;

    [Header("Info")]
    public float pickupDistance = 10f;
    public float dropForwardForce, dropUpwardForce;

    public Camera cam;
    public Transform playerT;

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
        AmmoDisplayGOS.SetActive(false);
        LoadKnife();
    }


    void Update()
    {
        EquipWeapon();
        DropCurrentWeapon();
        UpdateUI();
    }

    public GameObject AddItem(GameObject weaponPrefab, WeaponSystem.WeaponType weaponType)
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

        return weaponGO;
    }

    public void RemoveItem(int index)
    {
        weaponInventory[index] = null;
    }

    public void EquipWeapon()
    {
        if (playerControls.Weapon.PrimaryWeapon.WasPerformedThisFrame())
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
        }
    }

    public void DropItem(GameObject itemDrop, GameObject droppedItemPrefab)
    {
        GameObject visualDroppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        Rigidbody visualDroppedItemRB = visualDroppedItem.GetComponent<Rigidbody>();

        //Add Force
        visualDroppedItemRB.velocity = playerT.GetComponent<Rigidbody>().velocity;
        visualDroppedItemRB.AddForce(cam.transform.forward * dropForwardForce, ForceMode.Impulse);

        //Random rotation
        float random = Random.Range(-1f, 1f);
        visualDroppedItemRB.AddTorque(new Vector3(random, random, random) * 10);

        //Store data on groundPrefab
        WeaponSystem itemDropWP = itemDrop.GetComponent<WeaponSystem>();
        PickUpSystem droppedItemPrefabPS = visualDroppedItem.GetComponent<PickUpSystem>();

        droppedItemPrefabPS.AmmoInReserve = itemDropWP.AmmoInReserve;
        droppedItemPrefabPS.currentAmmo = itemDropWP.currentAmmo;
        droppedItemPrefabPS.maxAmmo = itemDropWP.maxAmmo;
        droppedItemPrefabPS.WeaponID = itemDropWP.WeaponID;


        Destroy(itemDrop);
    }

    public void DropCurrentWeapon()
    {
        if (playerControls.Weapon.Drop.WasPerformedThisFrame())
        {
            if (currentSelectedWeapon != null && currentSelectedWeapon.tag != "Knifes")
            {
                DropItem(currentSelectedWeapon, currentSelectedWeapon.GetComponent<WeaponSystem>().groundPrefab);
            }
        }
    }

    public void UpdateUI()
    {
        if(currentSelectedWeapon != null && currentSelectedWeapon.tag != "Knifes")
        {
            AmmoDisplayGOS.SetActive(true);
        }
        else
        {
            AmmoDisplayGOS.SetActive(false);
        }
    }

    public void LoadKnife()
    {
        AddItem(Inventory.Instance.selectedItem, WeaponSystem.WeaponType.Melee);
        SwitchWeapon((int)WeaponSystem.WeaponType.Melee);
    }
}