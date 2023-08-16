using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
Manages the Inventory slot logic of the items in the inventory.
Inventory Json is read into theInventoryList.
The inventory and Json is saved/updated based off theInventoryList.
*/
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    string inventoryJsonPath = Application.dataPath + "/InventoryData.txt";
    
    private List<GameObject> Slots = new List<GameObject>();

    public delegate ItemManager.Item GetItemInfo(int itemId);
    public static event GetItemInfo onGetItemInfo;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount;
    
    

    [SerializeField] private GameObject mouseItemDisplay;
    private bool itemSelected;
    private int selectedItemId;

    private Canvas canvas;

    [Serializable]
    public class itemSlotInfo {
        public int itemId;
        public int amount;
    }

    [Serializable]
    public class inventoryList {
        public List<itemSlotInfo> inventory;
    }

    inventoryList theInventoryList = new inventoryList();
    private void Awake()
    {
        selectedItemId = -1;

        if (instance == null) {
            instance = this;
        }

        SelectionManager.onSlotSelected += SlotSelect;
        canvas = transform.root.GetComponent<Canvas>();

        //Load inventory from Json
        if (File.Exists(inventoryJsonPath)) {
            using StreamReader reader = new StreamReader(inventoryJsonPath);
            string json = reader.ReadToEnd();
            reader.Close();

            theInventoryList = JsonUtility.FromJson<inventoryList>(json);
        }
        else {
            Debug.Log("Error loading inventory.");
        }
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
        
        //Loads inventory from Json
        int invCount = theInventoryList.inventory.Count;
        for (int i = 0; i < invCount; i++)
        {
            ChangeItemInSlot(Slots[i], theInventoryList.inventory[i].itemId, theInventoryList.inventory[i].amount);
        }
    }

    private void Update()
    {
        //Keeps mouseItemDisplay on the cursor
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out screenPos);
        mouseItemDisplay.transform.position = canvas.transform.TransformPoint(screenPos);
    }

    void ChangeItemInSlot(GameObject slotToChange, int itemId, int amount) {

        int foundIndex;

        //Will have to set the slot as null if itemID is -1
        if (itemId == -1) {
            //Item Sprite
            slotToChange.transform.GetChild(0).gameObject.SetActive(false);
            slotToChange.transform.GetChild(0).GetComponent<Image>().sprite = null;
            //Item Label
            slotToChange.transform.GetChild(1).gameObject.SetActive(false);
            slotToChange.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = null;
            //Item Quantity
            slotToChange.transform.GetChild(2).gameObject.SetActive(false);
            slotToChange.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = null;
            
            //Find the object in the Slots[] list and return the index
            foundIndex = Slots.IndexOf(slotToChange);
            
            //Update theInventoryList
            theInventoryList.inventory[foundIndex].itemId = itemId;
            theInventoryList.inventory[foundIndex].amount = amount;

            UpdateInventory();
            return;
        } 

        //Calls the ItemManager to get item info and return it
        ItemManager.Item foundItem = onGetItemInfo?.Invoke(itemId); 
        
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

        //Find the object in the Slots[] list and return the index
        foundIndex = Slots.IndexOf(slotToChange);

        //Update theInventoryList
        theInventoryList.inventory[foundIndex].itemId = itemId;
        theInventoryList.inventory[foundIndex].amount = amount;

        UpdateInventory();
    }

    private void SlotSelect(GameObject selectedObject)
    {
        int index;

        if (itemSelected) {
            //if the slot clicked on doesnt have an item displayed, aka, no item in the slot
            if (!selectedObject.transform.GetChild(0).gameObject.activeInHierarchy) {
                return;
            }

            index = Slots.IndexOf(selectedObject);
            selectedItemId = theInventoryList.inventory[index].itemId;
            mouseItemDisplay.GetComponent<ItemHolder>().itemID = selectedItemId;

            mouseItemDisplay.SetActive(true);
            //Set Item Image
            mouseItemDisplay.GetComponent<Image>().sprite =
                selectedObject.transform.GetChild(0).GetComponent<Image>().sprite;
            //Set Item Quantity
            mouseItemDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                selectedObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;

            //Empty the slot that is clicked on
            ChangeItemInSlot(selectedObject,-1,0);
        }
        else {
            Sprite spriteHolder = null;
            string amountHolder = null;
            int itemIdHolder = -1;
            bool itemSwap = false;
            //If this is true, that means there is an item in the slot
            if (selectedObject.transform.GetChild(0).gameObject.activeInHierarchy) {
                itemSwap = true;
                //"Pick up" the object in the slot to make room
                // Item Sprite
                spriteHolder = selectedObject.transform.GetChild(0).GetComponent<Image>().sprite;
                //Item Quantity
                amountHolder = selectedObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
                index = Slots.IndexOf(selectedObject);
                itemIdHolder = theInventoryList.inventory[index].itemId;
            }

            //Set the item down into the slot
            ChangeItemInSlot(selectedObject, selectedItemId,
                Int32.Parse(mouseItemDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text));

            if (itemSwap)
            {
                selectedItemId = itemIdHolder;
                mouseItemDisplay.GetComponent<Image>().sprite = spriteHolder;
                mouseItemDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = amountHolder;
                itemSelected = !itemSelected;
            }
            else
            {
                mouseItemDisplay.SetActive(false);
                mouseItemDisplay.GetComponent<Image>().sprite = null;
                mouseItemDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
            }

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
    }

    private void UpdateInventory() {
        
        //If inventory is LARGER than what is saved, expand the list
        if (Slots.Count > theInventoryList.inventory.Count) {
            int difference = Slots.Count - theInventoryList.inventory.Count;
            for (int i = 0; i < difference; i++)
            {
                theInventoryList.inventory.Add(new itemSlotInfo {itemId = -1,amount = 0});
            }
        }

        string json = JsonUtility.ToJson(theInventoryList);
       

        if (!File.Exists(inventoryJsonPath)) { 
            File.WriteAllText(inventoryJsonPath, json);
        } else {
            using (var writer = new StreamWriter(inventoryJsonPath, false)) { 
                writer.WriteLine(json);
                writer.Close();
            }
            
        }

        json = null;
    }
}
