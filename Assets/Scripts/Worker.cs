using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private NavMeshAgent _agent;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void ReceiveWorkOrder(Station station, Recipe order) {
        Debug.Log(name + " received work order for " + order.displayName + ".");
        _agent.SetDestination(station.transform.position);
    }

    /* Arrive At Station
     * Check station for materials needed for current order
     */
}
