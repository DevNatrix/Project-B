using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfoSO", menuName = "ScriptableObjects/CreateWeapon", order = 1)]
public class WeaponInfo : ScriptableObject
{
    [Header("Info")]
    public string WeaponName;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;

    [Header("Gun Properties")]
    public int startAmmo;
    public int magSize;
    public int mags;
    public float fireRate;
    public float reloadTime;

    [HideInInspector] public bool reloading;

}
