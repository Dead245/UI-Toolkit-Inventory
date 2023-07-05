using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the reading of item data and setting them to list
public class ItemManager : MonoBehaviour
{
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
        //[Future Reference] List name needs to be the same as the top of the json file, case sensitive.
    }

    public ItemList theItemList = new ItemList();

    // Start is called before the first frame update
    void Start()
    {
        theItemList = JsonUtility.FromJson<ItemList>(itemJSON.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
