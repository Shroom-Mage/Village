using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskboard : MonoBehaviour
{
    private class Order {
        public Task task;
        public Worker assignee;
    }

    private List<Order> _pendingOrders = new List<Order>(0);
    [SerializeField]
    private List<Stockpile> _stockpiles = new List<Stockpile>(0);
    [SerializeField]
    private List<Worker> _workers = new List<Worker>(0);



    private void Update() {
        AssignOrders();
    }

    public Task CreateTask(Recipe recipe, Station station, Worker assignee = null) {
        //Create a task from the recipe and station
        Task task = new Task {
            recipe = recipe,
            station = station
        };
        //Bind the task to the worker
        Order order = new Order();
        order.task = task;
        order.assignee = assignee;
        _pendingOrders.Add(order);
        if (assignee) assignee.currentTask = task;
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
            if ((worker.station == order.task.station) || (!worker.station && !worker.currentTask.recipe)) {
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
