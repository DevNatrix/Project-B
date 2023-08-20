using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
	//controlled by server:
	public float headshotMultiplier = 1.5f;
    public int damage;
	public int maxDistance;
	public float fireRate;
    public float bulletSpeed;
	public int AmmoInReserve;
	public int maxAmmo;
	public int currentAmmo;

	public LayerMask hitMask;


	[TextArea]
	[Tooltip("Doesn't do anything. Just important info")]
	public string ImportantInfo = "When adding another weapon, dont forget to make sure the weapon ID is right, you add it to the weapon switcher list, add it to the other client visual model (along with positioning the ik points), and assign it to the other client weapon list.";

	[HideInInspector] public static WeaponSystem Instance;
    PlayerControls playerControls;
	float headshotHeight = .6f;

	[Header("References")]
    private GameObject cam;
    public Animator anim;
    public GameObject groundPrefab;
    public GameObject bulletHole;
    [HideInInspector] public ServerEvents serverEvents;

    [Header("Info")]
    public int WeaponID;


    private float nextFire;


	public WeaponType weaponType;

    [Header("UI")]
    private TextMeshProUGUI AmmoAndMagText;

    [Header("Other")]
    public float dropForce = 0.5f;

	BulletManager bulletManager;
	public Transform  shootPoint;
	HitMarker hitMarker;

    private void Awake()
    {
        playerControls = new PlayerControls();
        Instance = this;

        cam = GameReferences.Instance.MainCam;
        AmmoAndMagText = GameReferences.Instance.AmmoAndMagText;
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
		bulletManager = GameObject.Find("game manager").GetComponent<BulletManager>();
		hitMarker = GameObject.Find("game manager").GetComponent<HitMarker>();
	}

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public enum WeaponType { Primary, Secondary, Melee}

    void Update()
    {
        if(weaponType == WeaponType.Melee)
        {
            Melee();
        }
        else
        {
            Shoot();
            Reload();
            Debug.DrawRay(cam.transform.position, cam.transform.forward);

            if(nextFire > 0)
            {
                nextFire -= Time.deltaTime;
            }


            AmmoAndMagText.text = currentAmmo.ToString() + "/" + AmmoInReserve.ToString();

            if(playerControls.Weapon.Drop.WasPerformedThisFrame())
            {
                //DropItem();
            }
        }
    }

	public void newSettings(string[] settings)
	{
		Debug.Log("New settings for gun " + WeaponID + ": " + ServerEvents.combineStringArray(settings));

		headshotMultiplier = float.Parse(settings[1]);
		damage = int.Parse(settings[2]);
		maxDistance = int.Parse(settings[3]);
		fireRate = float.Parse(settings[4]);
		bulletSpeed = float.Parse(settings[5]);
		AmmoInReserve = int.Parse(settings[6]);
		maxAmmo = int.Parse(settings[7]);
		currentAmmo = int.Parse(settings[8]);
	}


	private void Melee()
    {
        if(playerControls.Weapon.Fire.WasPerformedThisFrame())
        {
            float knifeDistance = 2f;
            float knifeDamage = 80f;


            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if(Physics.Raycast(ray.origin, ray.direction, out hit, knifeDistance, hitMask))
            {
                if(hit.transform.gameObject.GetComponent<OtherClient>())
                {
                    int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
                    OtherClient hitClientScript = serverEvents.getOtherClientScriptByID(clientID);
                    serverEvents.sendDirectEvent("damage", new string[] { knifeDamage.ToString(), hitClientScript.currentLife + "", Client.ID + "" }, clientID);
                }
            }
            
        }
    }
    private void Shoot()
    {
       if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0 && currentAmmo > 0 && gameObject.GetComponent<Animator>().GetBool("Reloading") == false && !MenuController.menu)
        {
            nextFire = 1 / fireRate;

            //Shoot raycast
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

			RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, hitMask))
            {
                if (hit.transform.gameObject.GetComponent<OtherClient>())
                {
					bool isHeadshot = hit.point.y > hit.transform.position.y + headshotHeight;
					hitMarker.hitPlayer(isHeadshot);
					int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
					OtherClient hitClientScript = serverEvents.getOtherClientScriptByID(clientID);

					int moddedDamage;
					if (isHeadshot)
					{
						moddedDamage = (int) ((float) damage * headshotMultiplier);
					}
					else
					{
						moddedDamage = damage;
					}

					serverEvents.sendDirectEvent("damage", new string[] { moddedDamage.ToString(), hitClientScript.currentLife + "",  Client.ID + ""}, clientID);
                }
				else
				{
					GameObject shotbulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
					Destroy(shotbulletHole, 6);
				}
            }

			//create visual bullet
			bulletManager.createBullet(shootPoint.position, cam.transform.forward * bulletSpeed);

            currentAmmo--;
            Recoil.Instance.FireRecoil();
            AudioPlayer.Instance.createAudio(1, gameObject.transform.position, .2f, 1, Client.ID);
        }
    }

    void Reload()
    {
        if(playerControls.Weapon.Reload.WasPressedThisFrame())
        {
            if (currentAmmo < maxAmmo && AmmoInReserve > 0)
            {
                //Play Reload Animation
                anim.SetBool("Reloading", true);
            }
        }
    }

    public void ReloadAmmo()
    {
        int ammoToAdd = maxAmmo - currentAmmo;
        int clamped = Mathf.Min(ammoToAdd, AmmoInReserve);
        AmmoInReserve -= clamped;
        currentAmmo += clamped;
    }

    //Stops reloading
    public void SetReloadFalse()
    {
        anim.SetBool("Reloading", false);
    }
}
