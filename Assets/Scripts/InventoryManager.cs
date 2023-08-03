using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Manages the Inventory slot logic of the items in the inventory.
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

    private Canvas canvas;

    [Serializable]
    public class itemSlotInfo {
        public int itemId;
        public int amount;
    }

    [Serializable]
    public class inventoryList {
        public List<itemSlotInfo> inventory = new List<itemSlotInfo>();
    }

    inventoryList theInventoryList = new inventoryList();
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
            theInventoryList.inventory.Add(new itemSlotInfo {itemId = -1, amount = 0}); // -1 for null item
        }

        //Adjusts the transform of the Grid Layout inside of the Scroll View based on the amount of slots
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, slotCount * 10);

        
        AddItem(0, 1);  //[TEMP] Testing the function
    }

    private void Update()
    {
        //Keeps mouseItemDisplay on the cursor
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out screenPos);
        mouseItemDisplay.transform.position = canvas.transform.TransformPoint(screenPos);
    }

    void ChangeItemInSlot(GameObject slotToChange, int itemId, int amount) {
        
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
    }

    //Adds item into inventory by looking for first empty slot and setting the appropriate info
    //Also checks to see if item can stack before putting it in empty slot
    void AddItem(int newItemId, int newAmount) {

        //[TEMP]
        ChangeItemInSlot(Slots[0], newItemId, newAmount);
        theInventoryList.inventory[0] = new itemSlotInfo { itemId = newItemId, amount = newAmount };
        UpdateInventory();
    }

    void RemoveItem() {
        //Removes a certain item from a slot in Slots list

        UpdateInventory();
    }
    private void SlotSelect(GameObject selectedObject)
    {
        if (itemSelected) {
            mouseItemDisplay.SetActive(true);
        }
        else {
            mouseItemDisplay.SetActive(false);
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
        string json = JsonUtility.ToJson(theInventoryList);

        if (!File.Exists(inventoryJsonPath)) { 
            File.WriteAllText(inventoryJsonPath, json);
        } else {
            using (var writer = new StreamWriter(inventoryJsonPath, false)) { 
                writer.WriteLine(json);
                writer.Close();
            }
            
        }
    }
}
