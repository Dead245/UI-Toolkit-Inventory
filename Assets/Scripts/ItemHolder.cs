using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public int itemID;

    private void Awake()
    {
        itemID = -1;
    }
}
