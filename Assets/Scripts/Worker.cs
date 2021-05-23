using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private bool _working = false;
    private float _accumulatedLabor = 0.0f;

    private NavMeshAgent _agent;

    public Station station;
    public Task currentTask;
    private Item _heldItem;
    private List<Task> _neededItems;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        //If the worker is working on a task
        if (_working) {
            _accumulatedLabor += Time.deltaTime;
            float itemCompletion = _accumulatedLabor / currentTask.recipe.requiredLabor * 100.0f;
            if (_accumulatedLabor >= currentTask.recipe.requiredLabor) {
                Debug.Log(name + " has completed work on " + currentTask.recipe.displayName + ".");
                currentTask.station.CompleteWorkOrder(currentTask.recipe);
                currentTask.recipe = null;
                currentTask.station = null;
                _working = false;
                _accumulatedLabor = 0.0f;
                currentTask = new Task { };
            }
        }
        //If the worker is on the way to a task
        else if (!_working && currentTask.station) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(currentTask.station.transform.position.x, currentTask.station.transform.position.z);
            if (workerPosition == stationPosition) {
                //ArriveAtStation();
                BeginTask();
            }
        }
    }

    public void TakeItem(Item item) {
        if (_heldItem) {
            DropItem();
        }
        if (item) {
            _heldItem = Instantiate(item, transform);
            _heldItem.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
        }
    }

    public Item DropItem() {
        Item droppedItem = _heldItem;
        if (_heldItem) {
            _heldItem.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
            _heldItem.transform.parent = null;
            _heldItem = null;
        }
        return droppedItem;
    }

    public void ReceiveTask(Task task) {
        Debug.Log(name + " received work order for " + task.recipe.displayName + ".");
        currentTask = task;
        _agent.SetDestination(task.station.transform.position);
    }

    private void ArriveAtStation() {
        Debug.Log(name + " has begun work on " + currentTask.recipe.displayName + ".");
        _working = currentTask.station.BeginRecipe(currentTask.recipe);
        if (!_working) {
            currentTask = new Task { };
        }
    }

    public void BeginTask() {
        //Stop if worker has no task
        if (!currentTask.recipe || !currentTask.station) {
            Debug.Log(name + " has no task to begin.");
            return;
        }
        Debug.Log(name + " has begun work on " + currentTask.recipe.displayName + ".");
        //Prepare a list of needed items
        List<Item> neededItems = new List<Item>(0);
        List<Item> requirements = currentTask.recipe.GetRequirements();
        foreach (Item requiredItem in requirements) {
            if (!requirements.Contains(requiredItem)) {
                neededItems.Add(requiredItem);
                //Create a hauling task for the item
            }
        }
        //If all requirements are present,
        //Set _working to true
    }
}
