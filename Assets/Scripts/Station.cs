using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Taskboard taskboard;

    [SerializeField]
    private List<Item> _inputItems = new List<Item>(0);
    [SerializeField]
    private List<Item> _tableItems = new List<Item>(0);
    [SerializeField]
    private List<Item> _outputItems = new List<Item>(0);

    //Instruct worker to check station for materials
    public void InitiateWorkOrder(Recipe recipe) {
        taskboard.CreateTask(recipe, this);
        Debug.Log(name + " has created a task for " + recipe.displayName + ".");
    }

    public bool BeginRecipe(Recipe recipe) { //To do: Move BeginRecipe to Worker (requires Stockpile)
        //Check if requirements are met
        foreach (Item requiredItem in recipe.GetRequirements()) {
            if (_inputItems.Contains(requiredItem)) {
                //Move item from input to table
                _inputItems.Remove(requiredItem);
                _tableItems.Add(requiredItem);
                Debug.Log("Moved " + requiredItem.displayName + " to the table.");
            } else {
                //Move everything back to input
                Debug.Log("Missing " + requiredItem.displayName + " required for " + recipe.displayName + ".");
                foreach (Item pendingItem in _tableItems) {
                    _inputItems.Add(pendingItem);
                    Debug.Log("Returned " + requiredItem.displayName + " to the stockpile.");
                }
                _tableItems.Clear();
                return false;
            }
        }
        return true;
    }

    public bool CompleteWorkOrder(Recipe order) {
        List<Item> finishedItems = new List<Item>(0);
        //Check if requirements are met
        foreach (Item requiredItem in order.GetRequirements()) {
            if (_tableItems.Contains(requiredItem)) {
                //Move item from table to finished
                _tableItems.Remove(requiredItem);
                finishedItems.Add(requiredItem);
            } else {
                //Move everything back to table
                Debug.Log("Missing " + requiredItem.displayName + " required for " + order.displayName + ".");
                foreach (Item pendingItem in finishedItems) {
                    _inputItems.Add(pendingItem);
                    Debug.Log("Returned " + requiredItem.displayName + " to the stockpile.");
                }
                finishedItems.Clear();
                return false;
            }
        }
        if (finishedItems.Count > 0) {
            finishedItems.Clear();
            _outputItems.Add(order.GetResult());
        }
        return true;
    }
}
