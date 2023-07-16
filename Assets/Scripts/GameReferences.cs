using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance { get; private set; }

    [Header("References")]
    [SerializeField] public GameObject MainCam;
    [SerializeField] public GameObject weaponHolder;
    [SerializeField] public TextMeshProUGUI AmmoAndMagText;
    [SerializeField] public GameObject AmmoDisplayGO;

    void Awake()
    {
        Instance = this;
    }
}
