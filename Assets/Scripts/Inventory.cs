using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<Item> _items = new List<Item>(0);
    private List<Item> _reservedItems = new List<Item>(0);

    public int count => _items.Count + _reservedItems.Count;

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

    public Item GetItemAt(int index) {
        if (index < 0 || index < _items.Count) {
            return _items[index];
        }
        return null;
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
