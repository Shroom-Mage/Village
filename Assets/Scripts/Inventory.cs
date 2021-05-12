using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> slots;

    public class Slot {
        public Item item;
        public int quantity;
    }
}
