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
    private List<Worker> _workers = new List<Worker>(0);

    private void Update() {
        AssignTasks();
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

    public void AssignTasks() {
        List<Order> openOrders = new List<Order>(0);
        List<Order> issuedOrders = new List<Order>(0);

        //Give each assignee their task, building a list of unassigned tasks
        foreach (Order order in _pendingOrders) {
            if (order.assignee) {
                order.assignee.ReceiveTask(order.task);
                issuedOrders.Add(order);
            }
            else {
                openOrders.Add(order);
            }
        }

        //Assign each open task to an available worker
        int orderIndex = 0;
        int workerIndex = 0;
        while (orderIndex < openOrders.Count && workerIndex < _workers.Count) {
            Order order = openOrders[orderIndex];
            Worker worker = _workers[workerIndex];
            if (worker.assignedStation || worker.currentTask.recipe) {
                workerIndex++;
            }
            else {
                order.assignee = worker;
                worker.ReceiveTask(order.task);
                issuedOrders.Add(order);
                orderIndex++;
                workerIndex++;
            }
        }

        //Remove issued orders from pending orders
        foreach (Order order in issuedOrders) {
            _pendingOrders.Remove(order);
        }

    }

    //public Task AssignTask(Worker worker) {
    //    bool taskFound = false;
    //    Task foundTask = new Task { };
    //    foreach (Order order in _orders) {
    //        if (order.assignee == worker) {
    //            foundTask = order.task;
    //            break;
    //        }
    //        if (!taskFound && !order.assignee) {
    //            taskFound = true;
    //            foundTask = order.task;
    //        }
    //    }
    //    if (foundTask.recipe) {
    //        worker.ReceiveTask(foundTask);
    //    }
    //    return foundTask;
    //}
}
