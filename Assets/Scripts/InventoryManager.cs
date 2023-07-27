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

        SelectionManager.onItemDrop += ItemDropped;

        AddItem("Bomb");  //Testing the function
    }

    void AddItem(string itemName) {
        //Adds item into inventory by looking for first empty slot and setting the appropriate info
        //Also checks to see if item can stack before putting it in empty slot

        ItemManager.Item foundItem = onGetItemInfo?.Invoke(itemName); //Calls the ItemManager to get item info and return it

        //Find the sprite with name
        Sprite itemSprite = Resources.Load<Sprite>("UI Sprites/" + foundItem.itemSprite);


        //Sets the item into slot 0 for now
        GameObject slotToModify = Slots[0];

        //Item Sprite
        slotToModify.transform.GetChild(0).gameObject.SetActive(true);
        slotToModify.transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
        //Item Label
        slotToModify.transform.GetChild(1).gameObject.SetActive(true);
        slotToModify.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = foundItem.name;
        //Item Quantity
        slotToModify.transform.GetChild(2).gameObject.SetActive(true);
        slotToModify.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = 1.ToString(); //Temp set quantity to 1

        Slots[0] = slotToModify;
    }

    void RemoveItem() {
        //Removes a certain item from a slot in Slots list
    }

    void ItemDropped(PointerEventData eventData) {
        Debug.Log("Dear god");
    }
}
