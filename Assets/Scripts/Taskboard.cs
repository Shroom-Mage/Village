using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskboard : MonoBehaviour
{
    private class Order {
        public Task task;
        public Worker assignee;
    }

    private List<Order> orders = new List<Order>(0);

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
        orders.Add(order);
        assignee.currentTask = task;
        return task;
    }

    public Task AssignTask(Worker worker) {
        bool taskFound = false;
        Task foundTask = new Task { };
        foreach (Order order in orders) {
            if (order.assignee == worker) {
                foundTask = order.task;
                break;
            }
            if (!taskFound && !order.assignee) {
                taskFound = true;
                foundTask = order.task;
            }
        }
        if (foundTask.recipe) {
            worker.ReceiveTask(foundTask);
        }
        return foundTask;
    }

    /* To do:
     * 
     *  Change the taskboard to search each worker instead
     *  of having each worker search the taskboard.
     *  For each order,
     *  if it has an assignee,
     *  have that assignee receive the task.
     *  Build a list of unassigned orders,
     *  then search the list of workers,
     *  giving each unassigned task to the next taskless worker.
     */
}
