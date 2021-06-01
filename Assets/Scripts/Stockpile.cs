using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : Inventory
{
    public Vector3 FindItemPosition(int index) {
        if (index < 0 || index < count) {
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
