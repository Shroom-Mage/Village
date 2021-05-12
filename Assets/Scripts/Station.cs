using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Worker assignee;
    public List<Recipe> orders;

    private Inventory _input;
    private Inventory _output;

    //Instruct worker to check station for materials
    public void InitiateWorkOrder(Recipe order) {
        Debug.Log(name + " has initiated a work order for " + order.displayName + ".");
        orders.Add(order);
        assignee.ReceiveWorkOrder(this, order);
    }
}
