using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the Inventory slot logic of the items in the inventory.
public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount;
    
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab,this.transform);
        }
    }
}
