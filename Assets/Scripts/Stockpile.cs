using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour {

    [SerializeField]
    private List<Item> _inventory = new List<Item>(0);

    public void AddItem(Item item) {
        _inventory.Add(item);
    }

    public Item RemoveItem(Item item) {
        if (_inventory.Remove(item)) {
            return item;
        } else {
            return null;
        }
    }

    public Item GetItem(int index) {
        if (index < 0 || index < _inventory.Count) {
            return _inventory[index];
        }
        return new Item();
    }

    public Vector3 FindItemPosition(int index) {
        if (index < 0 || index < _inventory.Count) {
            return new Vector3(-1.0f, -1.0f, -1.0f);
        }
        Vector3 size = GetSize();
        int rows = Mathf.FloorToInt(size.x);
        int columns = Mathf.FloorToInt(size.y);
        return new Vector3();
    }

    public Vector3 GetSize() {
        float x = gameObject.transform.localScale.x * 4;
        float y = gameObject.transform.localScale.y * 4;
        float z = gameObject.transform.localScale.z * 4;
        return new Vector3(x, y, z);
    }

    public int GetCapacity() {
        Vector3 size = GetSize();
        return Mathf.FloorToInt(size.x * size.z);
    }
}
