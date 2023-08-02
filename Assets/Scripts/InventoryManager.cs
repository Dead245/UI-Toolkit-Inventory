using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Manages the Inventory slot logic of the items in the inventory.
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public delegate ItemManager.Item GetItemInfo(string name);
    public static event GetItemInfo onGetItemInfo;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount;

    [SerializeField] private List<GameObject> Slots = new List<GameObject>();

    
    [SerializeField] private GameObject mouseItemDisplay;
    private bool itemSelected;

    private Canvas canvas;
    private void Awake()
    {

        if (instance == null) {
            instance = this;
        }

        SelectionManager.onSlotSelected += SlotSelect;
        canvas = transform.root.GetComponent<Canvas>();
    }


    // Start is called before the first frame update
    void Start()
    {
        mouseItemDisplay.SetActive(false);
        itemSelected = true;

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, this.transform);
            slotObj.name = "Item Slot " + i;
            Slots.Add(slotObj);
            
        }

        //Adjusts the transform of the Grid Layout inside of the Scroll View based on the amount of slots
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, slotCount * 10);

        
        AddItem("Bomb");  //Testing the function
    }

    private void Update()
    {
        //Keeps mouseItemDisplay on the cursor
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out screenPos);
        mouseItemDisplay.transform.position = canvas.transform.TransformPoint(screenPos);
    }

    void ChangeItemInSlot(GameObject slotToChange, string itemName, int amount) {
        
        //Calls the ItemManager to get item info and return it
        ItemManager.Item foundItem = onGetItemInfo?.Invoke(itemName); 
        
        //Find the sprite with name
        Sprite itemSprite = Resources.Load<Sprite>("UI Sprites/" + foundItem.itemSprite);
        if (itemSprite == null) Debug.LogError("No sprite found with name: UI Sprites/" + foundItem.itemSprite);

        //Item Sprite
        slotToChange.transform.GetChild(0).gameObject.SetActive(true);
        slotToChange.transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
        //Item Label
        slotToChange.transform.GetChild(1).gameObject.SetActive(true);
        slotToChange.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = foundItem.name;
        //Item Quantity
        slotToChange.transform.GetChild(2).gameObject.SetActive(true);
        slotToChange.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }

    //Adds item into inventory by looking for first empty slot and setting the appropriate info
    //Also checks to see if item can stack before putting it in empty slot
    void AddItem(string itemName) {
        //[TEMP] Sets the item into slot 0 for now
        GameObject slotToModify = Slots[0];

        ChangeItemInSlot(slotToModify,itemName, 1); //Temp set quantity to 1

    }

    void RemoveItem() {
        //Removes a certain item from a slot in Slots list
    }
    private void SlotSelect(GameObject selectedObject)
    {
        if (itemSelected) {
            mouseItemDisplay.SetActive(true);
            Debug.Log(selectedObject.name + " selected. " + itemSelected);
        }
        else {
            mouseItemDisplay.SetActive(false);
            Debug.Log(selectedObject.name + " unselected/dropped. " + itemSelected);
        }

        itemSelected = !itemSelected;

    }

    //mouse clicked in empty space with an item
    public void emptyClick()
    {
        if (!itemSelected)
        {
            mouseItemDisplay.SetActive(false);
            itemSelected = !itemSelected;
        }

        Debug.Log("Clicked in an empty area. " + itemSelected);
    }
}
