using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    [HideInInspector] public static WeaponSystem Instance;
    PlayerControls playerControls;

    [Header("References")]
    private GameObject cam;
    public Animator anim;
    public GameObject groundPrefab;
    public GameObject bulletHole;
    [HideInInspector] public ServerEvents serverEvents;

    [Header("Info")]
    public string WeaponID;

    public int damage;

    public int maxDistance;

    public float fireRate;

    private float nextFire;

    public WeaponType weaponType;

    [Header("Ammo")]
    public int AmmoInReserve;
    public int currentAmmo;
    public int maxAmmo;

    [Header("UI")]
    private TextMeshProUGUI AmmoAndMagText;

    [Header("Other")]
    public float dropForce = 0.5f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        Instance = this;

        cam = GameReferences.Instance.MainCam;
        AmmoAndMagText = GameReferences.Instance.AmmoAndMagText;
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
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

    private void Shoot()
    {
       if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0 && currentAmmo > 0 && gameObject.GetComponent<Animator>().GetBool("Reloading") == false)
        {
            nextFire = 1 / fireRate;

            //Shoot raycast
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance))
            {
                if (hit.transform.gameObject.GetComponent<Health>())
                {
                    int clientID = hit.transform.gameObject.GetComponent<OtherClient>().ID;
                    serverEvents.sendDirectEvent("damage", new string[] { damage.ToString() }, clientID);
                }

                GameObject shotbulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                StartCoroutine(RemoveBulletHole(shotbulletHole));
            }

            currentAmmo--;
            AudioPlayer.Instance.sendAudioByID(1, gameObject.transform.position, .2f, 1);
        }
    }
    
    IEnumerator RemoveBulletHole(GameObject _bulletHole)
    {
        yield return new WaitForSeconds(6);
        Destroy(_bulletHole);
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
