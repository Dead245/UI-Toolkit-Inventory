using System;
using System.Collections;
using System.Collections.Generic;
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
        public Item[] Item;
    }

    public ItemList theItemList = new ItemList();


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
    }
    private void GetItemInfo(string name)
    {
        //Look for item with name and return info
        Debug.Log("Item Manager recieved event to look for: " + name);
    }
}
