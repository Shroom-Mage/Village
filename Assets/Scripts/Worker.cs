using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void ReceiveWorkOrder(Station station, WorkOrder order) {
        Debug.Log(name + ": Work Order received.");
        _agent.SetDestination(station.transform.position);
    }

    /* Arrive At Station
     * Check station for materials needed for current order
     */
}