using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : Inventory
{
    [SerializeField]
    private Item _primaryItem;
    [SerializeField]
    private List<Item> _bonusItems;

    public float regenTime = 30.0f;
    public float requiredLabor = 3.0f;

    [SerializeField]
    private Transform _node1;
    [SerializeField]
    private Transform _node2;
    [SerializeField]
    private Transform _node3;

    private float _regenTimer1 = 0.0f;
    private float _regenTimer2 = 0.0f;
    private float _regenTimer3 = 0.0f;

    private void Update() {
        _regenTimer1 += Time.deltaTime;
        _regenTimer2 += Time.deltaTime;
        _regenTimer3 += Time.deltaTime;

        if (_regenTimer1 > regenTime) {
            SetNodeActive(1, true);
        }
        if (_regenTimer2 > regenTime) {
            SetNodeActive(2, true);
        }
        if (_regenTimer3 > regenTime) {
            SetNodeActive(3, true);
        }
    }

    public Item Harvest() {
        if (GetNodeActive(1)) {
            SetNodeActive(1, false);
            AddItem(_primaryItem);
            return _primaryItem;
        } else if (GetNodeActive(2)) {
            SetNodeActive(2, false);
            AddItem(_primaryItem);
            return _primaryItem;
        } else if (GetNodeActive(3)) {
            SetNodeActive(3, false);
            AddItem(_primaryItem);
            return _primaryItem;
        }
        return null;
    }

    public bool IsAvailable() {
        return GetNodeActive(1) || GetNodeActive(2) || GetNodeActive(3);
    }

    private bool GetNodeActive(int number) {
        GameObject node;
        switch (number) {
            case 1:
                node = _node1.gameObject;
                break;
            case 2:
                node = _node2.gameObject;
                break;
            case 3:
                node = _node3.gameObject;
                break;
            default:
                return false;
        }
        return node.activeInHierarchy;
    }

    private void SetNodeActive(int number, bool state) {
        switch (number) {
            case 1:
                _node1.gameObject.SetActive(state);
                _regenTimer1 *= state ? 1 : 0;
                break;
            case 2:
                _node2.gameObject.SetActive(state);
                _regenTimer2 *= state ? 1 : 0;
                break;
            case 3:
                _node3.gameObject.SetActive(state);
                _regenTimer3 *= state ? 1 : 0;
                break;
        }
    }
}
