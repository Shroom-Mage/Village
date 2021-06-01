using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Inventory
{
    public enum Type {
        Crafting,
        Hauling,
        Harvesting
    }

    public Type type;

    public Taskboard taskboard;

    public Stockpile input;
    public Stockpile output;

    [SerializeField]
    private Recipe _recipe;

    [SerializeField]
    private List<Harvestable> _harvestables;

    public CraftingTask CreateCraftingTask() {
        Debug.Log(name + " has created a task to craft " + _recipe.displayName + ".");
        return taskboard.CreateCraftingTask(this, _recipe);
    }

    public HaulingTask CreateFetchTask(Item item) {
        Debug.Log(name + " has created a task to retrieve " + item.displayName + ".");
        return taskboard.CreateHaulingTask(this, item, input, this);
    }

    public HaulingTask CreateDeliveryTask(Item item) {
        Debug.Log(name + " has created a task to deliver " + item.displayName + ".");
        return taskboard.CreateHaulingTask(this, item, this, output);
    }

    public HaulingTask CreateTransferTask(Item item, Inventory from, Inventory to) {
        Debug.Log(name + " has created a task to transfer " + item.displayName + ".");
        return taskboard.CreateHaulingTask(this, item, from, to);
    }

    public HarvestingTask CreateHarvestingTask() {
        Debug.Log(name + " has created a task to harvest from " + _harvestables[0] + ".");
        return taskboard.CreateHarvestingTask(this, _harvestables[0], output);
    }

    public void PostTask(Task task) {
        taskboard.PostTask(task);
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
