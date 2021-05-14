using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private bool _working = false;
    private float _accumulatedLabor = 0.0f;

    private NavMeshAgent _agent;

    public Taskboard taskboard;
    public Task currentTask;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        //If the worker is working on a task
        if (_working) {
            _accumulatedLabor += Time.deltaTime;
            float itemCompletion = _accumulatedLabor / currentTask.recipe.requiredLabor * 100.0f;
            Debug.Log("Villager is " + itemCompletion + "% finished working on " + currentTask.recipe.displayName + ".");
            if (_accumulatedLabor >= currentTask.recipe.requiredLabor) {
                currentTask.station.CompleteWorkOrder(currentTask.recipe);
                currentTask.recipe = null;
                currentTask.station = null;
                _working = false;
                _accumulatedLabor = 0.0f;
            }
        }
        //If the worker is on the way to a task
        else if (!_working && currentTask.station) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(currentTask.station.transform.position.x, currentTask.station.transform.position.z);
            if (workerPosition == stationPosition) {
                ArriveAtStation();
            }
        }
        //If the worker has no task
        else if (!_working) {
            taskboard.AssignTask(this); //To do: Remove this
        }
    }

    public void ReceiveTask(Task task) {
        Debug.Log(name + " received work order for " + task.recipe.displayName + ".");
        currentTask = task;
        _agent.SetDestination(task.station.transform.position);
    }

    private void ArriveAtStation() {
        Debug.Log(name + " has begun work on " + currentTask.recipe.displayName + ".");
        _working = currentTask.station.BeginRecipe(currentTask.recipe);
    }
}
