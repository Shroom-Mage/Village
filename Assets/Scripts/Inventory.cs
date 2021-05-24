using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    protected List<Item> _items = new List<Item>(0);
    private List<Item> _reservedItems = new List<Item>(0);

    public void AddItem(Item item) {
        _items.Add(item);
    }

    public Item RemoveItem(Item item) {
        if (_items.Remove(item)) {
            return item;
        }
        return null;
    }

    public bool ContainsItem(Item item) {
        return _items.Contains(item);
    }

    //Reserve an item
    public bool ReserveItem(Item item) {
        bool containsItem = _items.Contains(item);
        if (containsItem) {
            _items.Remove(item);
            _reservedItems.Add(item);
        }
        return containsItem;
    }

    //Return all reserved items
    public void ReturnReservedItems() {
        foreach (Item item in _reservedItems) {
            _items.Add(item);
        }
        _reservedItems.Clear();
    }
}
