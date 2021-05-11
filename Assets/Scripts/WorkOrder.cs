using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Work Order")]
public class WorkOrder : ScriptableObject
{
    public int _resources = 0;
    public float _labor = 0.0f;

    private bool _isComplete = false;

    //public WorkOrder(int resources, float labor) {
    //    _resources = resources;
    //    _labor = labor;
    //}
}
