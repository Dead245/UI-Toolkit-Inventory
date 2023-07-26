using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the Inventory slot logic of the items in the inventory.
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount;
    
    private bool isOpen;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab,this.transform);
        }
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, slotCount * 10);
    }
}
