using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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

	public Transform leftHandTarget;
	public Transform rightHandTarget;

	public LayerMask hitMask;


	[TextArea]
	[Tooltip("Doesn't do anything. Just important info")]
	public string ImportantInfo = "When adding another weapon, dont forget to make sure the weapon ID is right, you add it to the weapon switcher list, add it to the other client visual model (along with positioning the ik points), and assign it to the other client weapon list.";

	[HideInInspector] public static WeaponSystem Instance;
    PlayerControls playerControls;
	float headshotHeight = .6f;

	[Header("References")]
    private GameObject cam;
	PlayerManager playerManager;
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

	Transform weaponContainer;

    public bool isShotgun = false;

    [HideInInspector] public int crntAmmoReset;
    [HideInInspector] public int ammoReserve;

    private void Start()
    {
        crntAmmoReset = currentAmmo;
        ammoReserve = AmmoInReserve;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        Instance = this;

        cam = GameReferences.Instance.MainCam;
        AmmoAndMagText = GameReferences.Instance.AmmoAndMagText;
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
		bulletManager = GameObject.Find("game manager").GetComponent<BulletManager>();
		hitMarker = GameObject.Find("game manager").GetComponent<HitMarker>();
		playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
		cam = GameObject.Find("Main Camera");

		Physics.IgnoreLayerCollision(17, 20);
		Physics.IgnoreLayerCollision(17, 7);

		weaponContainer = transform.parent.parent;
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

	private void OnDrawGizmos()
	{
		RaycastHit hit;

		if (Physics.Raycast(weaponContainer.position, weaponContainer.forward, out hit, maxDistance, hitMask))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(hit.point, .1f);
		}
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, hitMask)){
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(hit.point, .1f);
		}

	}

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
            Debug.DrawRay(weaponContainer.position, weaponContainer.forward);

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
		//Debug.Log("New settings for gun " + WeaponID + ": " + ServerEvents.combineStringArray(settings));

		headshotMultiplier = float.Parse(settings[2]);
		damage = int.Parse(settings[3]);
		maxDistance = int.Parse(settings[4]);
		fireRate = float.Parse(settings[5]);
		bulletSpeed = float.Parse(settings[6]);
		AmmoInReserve = int.Parse(settings[7]);
		maxAmmo = int.Parse(settings[8]);
		currentAmmo = int.Parse(settings[9]);
	}


	private void Melee()
    {
        if(playerControls.Weapon.Fire.WasPerformedThisFrame())
        {
            Ray ray = new Ray(weaponContainer.position, weaponContainer.forward);
            RaycastHit hit;

            if(Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, hitMask))
            {
                if(hit.transform.gameObject.GetComponent<OtherClient>())
                {
					float damageMult = 1 + Upgrades.instance.damageMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Damage");
					int moddedDamage = (int)((float)damage * damageMult);

                    int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
                    OtherClient hitClientScript = serverEvents.getOtherClientScriptByID(clientID);
                    serverEvents.sendDirectEvent("damage", new string[] { moddedDamage.ToString(), Client.ID + "" }, clientID);
                }
            }
            
        }
    }
    private void Shoot()
    {
        if(isShotgun)
        {
            ShotgunShoot();
        }
        else if(!isShotgun)
        {
            NormalShoot();
        }
    }

    void NormalShoot()
    {
        if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0 && currentAmmo > 0 && gameObject.GetComponent<Animator>().GetBool("Reloading") == false && !MenuController.weaponUseLocked)
        {
            float fireRateMult = 1 + Upgrades.instance.firerateMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Firerate");
            nextFire = 1 / (fireRate * fireRateMult);

            RaycastHit hit;

                //Shoot raycast
                Ray ray = new Ray(weaponContainer.position, weaponContainer.forward);
                if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, hitMask))
                {
                    if (hit.transform.gameObject.GetComponent<OtherClient>())
                    {
                        bool isHeadshot = hit.point.y > hit.transform.position.y + headshotHeight;
                        hitMarker.hitPlayer(isHeadshot);
                        int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
                        OtherClient hitClientScript = serverEvents.getOtherClientScriptByID(clientID);

                        float damageMult = 1 + Upgrades.instance.damageMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Damage");
                        int moddedDamage;
                        if (isHeadshot)
                        {
                            float moddedHeadshot = headshotMultiplier + Upgrades.instance.headshotMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Headshot Mult");
                            moddedDamage = (int)((float)damage * moddedHeadshot * damageMult);
                        }
                        else
                        {
                            moddedDamage = (int)((float)damage * damageMult);
                        }

                        serverEvents.sendDirectEvent("damage", new string[] { moddedDamage.ToString(), Client.ID + "" }, clientID);
                    }
                    else
                    {
                        GameObject shotbulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                        Destroy(shotbulletHole, 6);
                    }
                }
            


            //create visual bullet
            bulletManager.createBullet(shootPoint.position, weaponContainer.forward * bulletSpeed);

            currentAmmo--;
            Recoil.Instance.FireRecoil();
            AudioPlayer.Instance.createAudio(1, gameObject.transform.position, .2f, 1, Client.ID);
        }
    }

    void ShotgunShoot()
    {
        int shotgunPellets = 8;
        float spreadAngle = 5f;

        if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0 && currentAmmo > 0 && gameObject.GetComponent<Animator>().GetBool("Reloading") == false && !MenuController.weaponUseLocked)
        {
            float fireRateMult = 1 + Upgrades.instance.firerateMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Firerate");
            nextFire = 1 / (fireRate * fireRateMult);

            RaycastHit hit;
            for (int i = 0; i < shotgunPellets; i++)
            {
                float spreadX = Random.Range(-spreadAngle, spreadAngle);
                float spreadY = Random.Range(-spreadAngle, spreadAngle);

                // Calculate the rotation for the pellet based on the spread angles
                Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);

                // Calculate the direction for the raycast using the spread rotation
                Vector3 pelletDirection = spreadRotation * weaponContainer.forward;

                //Shoot raycast
                Ray ray = new Ray(weaponContainer.position, pelletDirection);
                if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, hitMask))
                {
                    if (hit.transform.gameObject.GetComponent<OtherClient>())
                    {
                        bool isHeadshot = hit.point.y > hit.transform.position.y + headshotHeight;
                        hitMarker.hitPlayer(isHeadshot);
                        int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
                        OtherClient hitClientScript = serverEvents.getOtherClientScriptByID(clientID);

                        float damageMult = 1 + Upgrades.instance.damageMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Damage");
                        int moddedDamage;
                        if (isHeadshot)
                        {
                            float moddedHeadshot = headshotMultiplier + Upgrades.instance.headshotMultPerLevel * (float)Upgrades.instance.getUpgradeLevel("Headshot Mult");
                            moddedDamage = (int)((float)damage * moddedHeadshot * damageMult);
                        }
                        else
                        {
                            moddedDamage = (int)((float)damage * damageMult);
                        }

                        serverEvents.sendDirectEvent("damage", new string[] { moddedDamage.ToString(), Client.ID + "" }, clientID);
                    }
                    else
                    {
                        GameObject shotbulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                        Destroy(shotbulletHole, 6);
                    }
                }
            }


            //create visual bullet
            bulletManager.createBullet(shootPoint.position, weaponContainer.forward * bulletSpeed);

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
        int ammoToAdd = (int)(maxAmmo) - currentAmmo;
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
