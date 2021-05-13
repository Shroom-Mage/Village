using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private Recipe _targetOrder;
    private Station _targetStation;
    private float _accumulatedLabor = 0.0f;
    private bool _working = false;

    private NavMeshAgent _agent;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (_working) {
            _accumulatedLabor += Time.deltaTime;
            float itemCompletion = _accumulatedLabor / _targetOrder.requiredLabor * 100.0f;
            Debug.Log("Villager is " + itemCompletion + "% finished working on " + _targetOrder.displayName + ".");
            if (_accumulatedLabor >= _targetOrder.requiredLabor) {
                _targetStation.CompleteWorkOrder(_targetOrder);
                _targetOrder = null;
                _targetStation = null;
                _working = false;
                _accumulatedLabor = 0.0f;
            }
        }
        else if (!_working && _targetStation) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(_targetStation.transform.position.x, _targetStation.transform.position.z);
            if (workerPosition == stationPosition) {
                ArriveAtStation();
            }
        }
    }

    public void ReceiveWorkOrder(Station station, Recipe order) {
        Debug.Log(name + " received work order for " + order.displayName + ".");
        _targetOrder = order;
        _targetStation = station;
        _agent.SetDestination(station.transform.position);
    }

    private void ArriveAtStation() {
        Debug.Log(name + " has begun work on " + _targetOrder.displayName + ".");
        _working = _targetStation.BeginWorkOrder(_targetOrder);
    }
}
