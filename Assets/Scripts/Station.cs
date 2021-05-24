using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Inventory
{
    public Taskboard taskboard;

    public Stockpile input;
    public Stockpile output;

    public void InitiateCraftingOrder(Recipe recipe) {
        taskboard.CreateCraftingTask(recipe, this);
        Debug.Log(name + " has created a task for " + recipe.displayName + ".");
    }

    //public bool CompleteWorkOrder(Recipe order) {
    //    List<Item> finishedItems = new List<Item>(0);
    //    //Check if requirements are met
    //    foreach (Item requiredItem in order.GetRequirements()) {
    //        if (_items.Contains(requiredItem)) {
    //            //Move item from table to finished
    //            _items.Remove(requiredItem);
    //            finishedItems.Add(requiredItem);
    //        } else {
    //            //Move everything back to table
    //            Debug.Log("Missing " + requiredItem.displayName + " required for " + order.displayName + ".");
    //            foreach (Item pendingItem in finishedItems) {
    //                _inputItems.Add(pendingItem);
    //                Debug.Log("Returned " + requiredItem.displayName + " to the stockpile.");
    //            }
    //            finishedItems.Clear();
    //            return false;
    //        }
    //    }
    //    if (finishedItems.Count > 0) {
    //        finishedItems.Clear();
    //        _outputItems.Add(order.GetResult());
    //    }
    //    return true;
    //}
}
