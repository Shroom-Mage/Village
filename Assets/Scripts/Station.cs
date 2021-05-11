using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Worker assignee;
    public List<WorkOrder> orders;

    private void Start() {
        Debug.Log(name + ": Start.");
        WorkOrder order = ScriptableObject.CreateInstance<WorkOrder>();
        orders.Add(order);
        InitiateWorkOrder(order);
    }

    //Instruct worker to check station for materials
    public void InitiateWorkOrder(WorkOrder order) {
        Debug.Log(name + ": Work Order initiated.");
        assignee.ReceiveWorkOrder(this, order);
    }
}
