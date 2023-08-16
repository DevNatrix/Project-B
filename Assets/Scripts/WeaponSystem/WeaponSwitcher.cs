using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    public static WeaponSwitcher Instance;

    private PlayerControls playerControls;
    public bool switchonScroll = true;
    private Animator anim;
    public GameObject currentSelectedWeapon;

    public GameObject[] weaponInventory;

    [Header("UI Weapon")]
    private GameObject AmmoDisplayGOS;

    [Header("Info")]
    public float pickupDistance = 10f;
    public float dropForwardForce, dropUpwardForce;
    public float distanceToDrop = 5f;

    public Camera cam;
    public Transform playerT;
	public ServerEvents serverEvents;

	private void Start()
    {
        Instance = this;

        AmmoDisplayGOS = GameReferences.Instance.AmmoDisplayGO;

        weaponInventory = new GameObject[3];
        AmmoDisplayGOS.SetActive(false);
        LoadWeapons();
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
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
		if(!MenuController.typing)
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

			int newID = currentSelectedWeapon.GetComponent<WeaponSystem>().WeaponID;
			serverEvents.sendEventToOtherClients("switchGun", new string[] { Client.ID + "", newID + "" });
		}
    }

    //Drop function for inventory
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

        //Selects closest item
        
    }

    public void DropCurrentWeapon()
    {
        if (playerControls.Weapon.Drop.WasPerformedThisFrame() && !MenuController.typing)
        {
            if (currentSelectedWeapon != null && currentSelectedWeapon.tag != "Knifes")
            {
                DropItem(currentSelectedWeapon, currentSelectedWeapon.GetComponent<WeaponSystem>().groundPrefab);
                Debug.Log("Dropped Current Weapon");
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

    //Loads Selected weapons, skins and such from Inventory.cs
    public void LoadWeapons()
    {
        GameObject _MeleeWeapon = AddItem(Inventory.Instance.meleeWeapon, WeaponSystem.WeaponType.Melee);
        _MeleeWeapon.SetActive(false);

        GameObject _SecondaryWeapon = AddItem(Inventory.Instance.secondaryWeapon, WeaponSystem.WeaponType.Secondary);
        SwitchWeapon((int)WeaponSystem.WeaponType.Secondary);
    }
}