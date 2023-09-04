using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class WeaponSwitcher : MonoBehaviour
{
	public static WeaponSwitcher Instance;

    private PlayerControls playerControls;
    public bool switchonScroll = true;
    private Animator anim;
    public GameObject currentSelectedWeapon;

    public GameObject[] weaponInventory;
    public List<GameObject> Weapons;
    public List<GameObject> DropWeapons;
	List<PickUpSystem> droppedWeaponList = new List<PickUpSystem>();

    [Header("UI Weapon")]
    private GameObject AmmoDisplayGOS;

    [Header("Info")]
    public float pickupDistance = 10f;
    public float dropForwardForce, dropUpwardForce;
    public float distanceToDrop = 5f;

    public Camera cam;
    public Transform playerT;
	public ServerEvents serverEvents;
	public PlayerManager playerManager;

	int currentDropID = 0;

	private void Start()
    {
        Instance = this;

        AmmoDisplayGOS = GameReferences.Instance.AmmoDisplayGO;

        weaponInventory = new GameObject[3];
        AmmoDisplayGOS.SetActive(false);

		if(Lobby.bestUDPPort != -1)
		{
			LoadWeapons();
		}

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

    public GameObject GetItemThroughID(int _ID)
    {
        foreach (GameObject weaponPrefab in Weapons)
        {
            if (weaponPrefab != null && weaponPrefab.GetComponent<WeaponSystem>().WeaponID == _ID)
            {
                return weaponPrefab;
            }
        }
        return null;
    }

    public void RemoveItem(int index)
    {
        weaponInventory[index] = null;
    }

    public void EquipWeapon()
    {
		if(!MenuController.weaponUseLocked)
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

			WeaponSystem currentWeaponSystem = currentSelectedWeapon.GetComponent<WeaponSystem>();
			Debug.Log(currentWeaponSystem);
			playerManager.leftHandTarget = currentWeaponSystem.leftHandTarget;
			playerManager.rightHandTarget = currentWeaponSystem.rightHandTarget;

			int newID = currentSelectedWeapon.GetComponent<WeaponSystem>().WeaponID;
			serverEvents.sendEventToOtherClients("switchGun", new string[] { Client.ID + "", newID + "" });
		}
    }

    //Drop function for inventory
    public void DropItem(GameObject itemDrop, GameObject droppedItemPrefab)
	{
		/*GameObject visualDroppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
		Rigidbody visualDroppedItemRB = visualDroppedItem.GetComponent<Rigidbody>();

        //Add Force
        visualDroppedItemRB.velocity = playerT.GetComponent<Rigidbody>().velocity;
        visualDroppedItemRB.AddForce(cam.transform.forward * dropForwardForce, ForceMode.Impulse);

        //Random rotation
        visualDroppedItemRB.AddTorque(randomVector3() * 10);

        //Store data on groundPrefab
        PickUpSystem droppedItemPrefabPS = visualDroppedItem.GetComponent<PickUpSystem>();

        droppedItemPrefabPS.AmmoInReserve = itemDropWP.AmmoInReserve;
        droppedItemPrefabPS.currentAmmo = itemDropWP.currentAmmo;
        droppedItemPrefabPS.maxAmmo = itemDropWP.maxAmmo;
        droppedItemPrefabPS.WeaponID = itemDropWP.WeaponID;*/

        WeaponSystem itemDropWP = itemDrop.GetComponent<WeaponSystem>();
		string[] dataToSend = new string[] { itemDropWP.WeaponID + "", currentDropID + "", transform.position + "", playerT.GetComponent<Rigidbody>().velocity + "", cam.transform.forward * dropForwardForce + "", randomVector3() * 10 + "", itemDropWP.AmmoInReserve + "", itemDropWP.currentAmmo + "", itemDropWP.maxAmmo + "" };

		serverEvents.sendGlobalEvent("newDroppedWeapon", dataToSend);

        Destroy(itemDrop);
    }

	public void otherPlayerPickedUpWeapon(int dropID)
	{
		foreach(PickUpSystem droppedWeapon in droppedWeaponList)
		{
			if(droppedWeapon.dropID == dropID)
			{
				Destroy(droppedWeapon.gameObject);
				return;
			}
		}

		Debug.LogError("Couldnt find weapon other player picked up: " + dropID);
		return;
	}

	public void createDropItem(int weaponID, int dropID, Vector3 position, Vector3 velocity, Vector3 forceAdded, Vector3 torqueAdded, int ammoInReserve, int currentAmmo, int maxAmmo)
	{
		if(dropID >= currentDropID)
		{
			currentDropID = dropID + 1;
		}

		GameObject droppedWeapon = Instantiate(DropWeapons[weaponID], position, Quaternion.identity);
		Rigidbody droppedWeaponRB = droppedWeapon.GetComponent<Rigidbody>();

		droppedWeaponRB.velocity = velocity;
		droppedWeaponRB.AddForce(forceAdded, ForceMode.Impulse);
		droppedWeaponRB.AddTorque(torqueAdded);

		PickUpSystem droppedWeaponPS = droppedWeapon.GetComponent<PickUpSystem>();
		droppedWeaponPS.AmmoInReserve = ammoInReserve;
		droppedWeaponPS.currentAmmo = currentAmmo;
		droppedWeaponPS.maxAmmo = maxAmmo;
		droppedWeaponPS.WeaponID = weaponID;
		droppedWeaponPS.dropID = dropID;

		droppedWeaponList.Add(droppedWeaponPS);
	}

	public static Vector3 randomVector3()
	{
		return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
	}

    public void DropCurrentWeapon()
    {
        if (playerControls.Weapon.Drop.WasPerformedThisFrame() && !MenuController.weaponUseLocked)
        {
            if (currentSelectedWeapon != null && currentSelectedWeapon.GetComponent<WeaponSystem>().weaponType != WeaponSystem.WeaponType.Melee)
            {
                DropItem(currentSelectedWeapon, currentSelectedWeapon.GetComponent<WeaponSystem>().groundPrefab);
                Debug.Log("Dropped Current Weapon");

				SwitchWeapon(2);
            }
        }
    }

    public void UpdateUI()
    {
        if(currentSelectedWeapon != null && currentSelectedWeapon.GetComponent<WeaponSystem>().weaponType != WeaponSystem.WeaponType.Melee)
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