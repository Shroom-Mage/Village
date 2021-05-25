using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : Inventory
{
    public float regenTime = 30.0f;

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
            _node1.gameObject.SetActive(true);
        }
        if (_regenTimer2 > regenTime) {
            _node2.gameObject.SetActive(true);
        }
        if (_regenTimer3 > regenTime) {
            _node3.gameObject.SetActive(true);
        }
    }

    public Item Harvest() {
        if (_node1.gameObject.activeInHierarchy) {
            _node1.gameObject.SetActive(false);
            _regenTimer1 = 0;
        }
        return null;
    }
}
