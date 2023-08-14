using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance;

    [Header("References")]
    [SerializeField] public GameObject MainCam;
    [SerializeField] public GameObject weaponHolder;
    [SerializeField] public TextMeshProUGUI AmmoAndMagText;
    [SerializeField] public GameObject AmmoDisplayGO;

    public void Awake()
    {
        Instance = this;
    }
}
