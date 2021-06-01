using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskboard : MonoBehaviour
{
    private List<Task> _pendingTasks = new List<Task>(0);
    [SerializeField]
    private List<Worker> _workers = new List<Worker>(0);

    private void Update() {
        AssignTasks();
    }

    public CraftingTask CreateCraftingTask(Station station, Recipe recipe, Worker assignee = null) {
        //Create a task from the recipe
        CraftingTask task = new CraftingTask {
            station = station,
            recipe = recipe
        };

        if (assignee) {
            //Give the assignee the task if there is one
            assignee.ReceiveTask(task);
        } else {
            //Otherwise add the task to pending
            _pendingTasks.Add(task);
        }

        return task;
    }

    public HaulingTask CreateHaulingTask(Station station, Item item, Inventory from, Inventory to, Worker assignee = null) {
        //Create a task from the item and inventories
        HaulingTask task = new HaulingTask {
            station = station,
            item = item,
            from = from,
            to = to
        };

        if (assignee) {
            //Give the assignee the task if there is one
            assignee.ReceiveTask(task);
        } else {
            //Otherwise add the task to pending
            _pendingTasks.Add(task);
        }

        return task;
    }

    public HarvestingTask CreateHarvestingTask(Station station, Harvestable harvestable, Stockpile stockpile, Worker assignee = null) {
        //Create a task from the harvestable and stockpile
        HarvestingTask task = new HarvestingTask {
            station = station,
            harvestable = harvestable,
            stockpile = stockpile
        };

        if (assignee) {
            //Give the assignee the task if there is one
            assignee.ReceiveTask(task);
        } else {
            //Otherwise add the task to pending
            _pendingTasks.Add(task);
        }
        return task;
    }

    public void PostTask(Task task, Worker assignee = null) {
        if (assignee) {
            //Give the assignee the task if there is one
            assignee.ReceiveTask(task);
        } else {
            //Otherwise add the task to pending
            _pendingTasks.Add(task);
        }
    }

    private void AssignTasks() {
        List<Task> issuedTasks = new List<Task>(0);

        //Assign each open task to an available worker
        int taskIndex = 0;
        int workerIndex = 0;
        while (taskIndex < _pendingTasks.Count && workerIndex < _workers.Count) {
            Task task = _pendingTasks[taskIndex];
            Worker worker = _workers[workerIndex];
            if (!worker.isBusy && (worker.station == task.station || !worker.station)) {
                //If this worker has no task and
                //either the task's station is their station or they have no station
                worker.ReceiveTask(task);
                issuedTasks.Add(task);
                taskIndex++;
                workerIndex++;
            } else {
                //Otherwise, skip this worker
                workerIndex++;
            }
        }

        //Remove issued tasks from pending tasks
        foreach (Task task in issuedTasks) {
            _pendingTasks.Remove(task);
        }
    }
}
