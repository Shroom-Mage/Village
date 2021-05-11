using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Worker assignee;
    public List<Recipe> orders;
    public Recipe recipe;

    private void Start() {
        Debug.Log(name + ": Start.");
        orders.Add(recipe);
        InitiateWorkOrder(recipe);
    }

    private void OnMouseDown() {
    }

    //Instruct worker to check station for materials
    public void InitiateWorkOrder(Recipe order) {
        Debug.Log(name + ": Work Order initiated.");
        assignee.ReceiveWorkOrder(this, order);
    }
}
