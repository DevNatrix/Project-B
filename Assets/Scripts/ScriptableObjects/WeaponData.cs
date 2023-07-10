using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "WeaponDataCreator/CreateWeaponSO")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;

    public float Damage;
    public float MaxDistance;

    public int CurrentAmmo;
    public int MagSize;
    public float FireRate;
    public float ReloadTime;
    [HideInInspector] public bool Reloading;
}
