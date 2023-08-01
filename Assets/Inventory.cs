using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public static Inventory Instance;

    public GameObject InventoryVisual;
    [HideInInspector] public GameObject selectedItem;
    [SerializeField] private GameObject defaultKnife;

    void Start()
    {
        InventoryVisual.SetActive(false);
        selectedItem = defaultKnife;
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInventory()
    {
        InventoryVisual.SetActive(true);
    }

    public void ExitInventory()
    {
        InventoryVisual.SetActive(false);
    }

    public void SelectItem(GameObject _ItemPrefab)
    {
        selectedItem = _ItemPrefab;
    }
}
