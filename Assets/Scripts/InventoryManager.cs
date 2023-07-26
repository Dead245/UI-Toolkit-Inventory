using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the Inventory slot logic of the items in the inventory.
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount;

    [SerializeField] List<GameObject> Slots = new List<GameObject>();


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
            Slots.Add(Instantiate(slotPrefab,this.transform));
            
        }
        //Adjusts the transform if the Grid Layout inside of the Scroll View based on the amount of slots
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, slotCount * 10);
    }

    void AddItem() { 
        //Adds item into inventory by looking for first empty slot and setting the appropriate info
        //Also checks to see if item can stack before putting it in empty slot
    }

    void RemoveItem() {
        //Removes a certain item from a slot in Slots list
    }
}
