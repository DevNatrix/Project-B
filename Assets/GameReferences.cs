using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance { get; private set; }

    [Header("References")]
    [SerializeField] public GameObject MainCam;
    [SerializeField] public GameObject weaponHolder;

    void Awake()
    {
        Instance = this;
    }
}
