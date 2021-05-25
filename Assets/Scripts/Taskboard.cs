using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskboard : MonoBehaviour
{
    private class Order {
        public CraftingTask task;
        public Worker assignee;
    }

    private List<Order> _pendingOrders = new List<Order>(0);
    [SerializeField]
    private List<Stockpile> _stockpiles = new List<Stockpile>(0);
    [SerializeField]
    private List<Harvestable> _harvestables = new List<Harvestable>(0);
    [SerializeField]
    private List<Worker> _workers = new List<Worker>(0);



    private void Update() {
        AssignOrders();
    }

    public CraftingTask CreateCraftingTask(Recipe recipe, Station station, Worker assignee = null) {
        //Create a task from the recipe and station
        CraftingTask task = new CraftingTask {
            recipe = recipe,
            station = station
        };

        //Bind the task to an order
        Order order = new Order();
        order.task = task;
        order.assignee = assignee;
        if (assignee) assignee.craftingTask = task;

        //Add the order to pending
        _pendingOrders.Add(order);

        return task;
    }

    public HaulingTask CreateHaulingTask(Item item, Inventory from, Inventory to, Worker assignee = null) {
        //Create a task from the item and inventories
        HaulingTask task = new HaulingTask {
            item = item,
            from = from,
            to = to
        };

        //Bind the task to an order

        //Add the order to pending

        return task;
    }

    public HarvestingTask CreateHarvestingTask(Harvestable harvestable, Stockpile stockpile) {
        //Create a task from the harvestable and stockpile
        HarvestingTask task = new HarvestingTask {
            harvestable = harvestable,
            stockpile = stockpile
        };
        return task;
    }

    private void AssignOrders() {
        List<Order> issuedOrders = new List<Order>(0);

        //Assign each open task to an available worker
        int orderIndex = 0;
        int workerIndex = 0;
        while (orderIndex < _pendingOrders.Count && workerIndex < _workers.Count) {
            Order order = _pendingOrders[orderIndex];
            Worker worker = _workers[workerIndex];
            if ((worker.station == order.task.station) || (!worker.station && !worker.craftingTask.recipe)) {
                //If this is the worker's station or the worker has no station and no task,
                //assign them to this task
                order.assignee = worker;
                worker.ReceiveTask(order.task);
                issuedOrders.Add(order);
                orderIndex++;
                workerIndex++;
            } else {
                //Otherwise, skip this worker
                workerIndex++;
            }
        }

        //Remove issued orders from pending orders
        foreach (Order order in issuedOrders) {
            _pendingOrders.Remove(order);
        }
    }
}
