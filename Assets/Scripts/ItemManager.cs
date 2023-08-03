using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Manages the reading of item data and setting them to list
//[Future Reference] List name and variables need to be the same as in the json file, case sensitive.
public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] TextAsset itemJSON;


    [System.Serializable]
    public class Item {
        public string name;
        public int maxStackSize;
        public string itemSprite;
    }

    [System.Serializable]
    public class ItemList {
        public List<Item> Item;
    }

    public ItemList theItemList = new ItemList();

    Dictionary<int, Item> itemDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        InventoryManager.onGetItemInfo += GetItemInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        theItemList = JsonUtility.FromJson<ItemList>(itemJSON.text);

        //Convert List to Dictionary with keys
        itemDict = theItemList.Item.Select((val, index) => new { Index = index, Value = val })
               .ToDictionary(i => i.Index, i => i.Value);
    }
    private Item GetItemInfo(int itemId)
    {
        Item foundItem;
        //Look for item with name and return info
        itemDict.TryGetValue(itemId, out foundItem);
        if (foundItem == null) Debug.Log("ERROR: Item with Id " + itemId + " not found.");

        return foundItem;
    }
}
