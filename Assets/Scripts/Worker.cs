using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private bool _crafting = false;
    private float _accumulatedLabor = 0.0f;

    private NavMeshAgent _agent;

    public Station station;
    public CraftingTask craftingTask;
    public HaulingTask haulingTask;
    private Item _heldItem;
    private Transform _heldTransform;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        //If the worker is crafting an item
        if (_crafting) {
            _accumulatedLabor += Time.deltaTime;
            float itemCompletion = _accumulatedLabor / craftingTask.recipe.requiredLabor * 100.0f;
            if (_accumulatedLabor >= craftingTask.recipe.requiredLabor) {
                FinishCrafting();
                _accumulatedLabor = 0.0f;
            }
        }
        //If the worker needs to get an item
        else if (haulingTask.item && !_heldItem) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 inventoryPosition = new Vector2(haulingTask.from.transform.position.x, haulingTask.from.transform.position.z);
            if (workerPosition == inventoryPosition) {
                //Take item from inventory
                TakeItem(haulingTask.item, haulingTask.from);
                //Bring item to destination
                _agent.SetDestination(haulingTask.to.transform.position);
            }
        }
        //If the worker needs to return with an item
        else if (haulingTask.item && _heldItem) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 inventoryPosition = new Vector2(haulingTask.to.transform.position.x, haulingTask.to.transform.position.z);
            if (workerPosition == inventoryPosition) {
                //Deposit held item to inventory
                DepositItem(haulingTask.to);
                haulingTask = new HaulingTask { };
                //Return to station if needed
                if (craftingTask.station) {
                    _agent.SetDestination(craftingTask.station.transform.position);
                }
            }
        }
        //If the worker needs to craft an item
        else if (craftingTask.recipe) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(craftingTask.station.transform.position.x, craftingTask.station.transform.position.z);
            if (workerPosition == stationPosition) {
                BeginCrafting();
            }
        }
        //If there's nothing else to do
        else {
            //Go home
            _agent.SetDestination(new Vector3());
        }
    }

    public void TakeItem(Item item, Inventory inventory = null) {
        //Drop our current item
        if (_heldItem) {
            DropItem();
        }
        //Remove the item from the inventory if specified
        if (inventory) {
            item = inventory.RemoveItem(item);
        }
        //Hold the item if it's valid
        if (item) {
            _heldItem = item;
            _heldTransform = Instantiate(item.prefab, transform);
            _heldTransform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
        }
    }

    public void DepositItem(Inventory inventory) {
        //Deposit the held item to the inventory if one is held
        if (_heldItem) {
            inventory.AddItem(_heldItem);
            Destroy(_heldTransform.gameObject);
            _heldItem = null;
            _heldTransform = null;
        }
    }

    public void DropItem() {
        if (_heldItem && _heldTransform) {
            _heldTransform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            _heldTransform.parent = null;
            _heldTransform = null;
            _heldItem = null;
        }
    }

    public void ReceiveTask(CraftingTask task) {
        Debug.Log(name + " received crafting order for " + task.recipe.displayName + ".");
        craftingTask = task;
        _agent.SetDestination(task.station.transform.position);
    }

    public void ReceiveTask(HaulingTask task) {
        Debug.Log(name + " received hauling order for " + task.item.displayName + ".");
        haulingTask = task;
        _agent.SetDestination(task.from.transform.position);
    }

    private void BeginCrafting() {
        //Stop if worker has no crafting task
        if (!craftingTask.recipe || !craftingTask.station) {
            Debug.Log(name + " has no task to begin.");
            return;
        }
        Debug.Log(name + " has begun crafting " + craftingTask.recipe.displayName + ".");
        //Prepare a list of needed items
        Station taskStation = craftingTask.station;
        bool itemsAreStillNeeded = false;
        List<Item> requirements = craftingTask.recipe.GetRequirements();
        foreach (Item requiredItem in requirements) {
            //Attempt to reserve the item
            if (!taskStation.ReserveItem(requiredItem)) {
                //Return all reserved items if we don't have all requirements
                itemsAreStillNeeded = true;
                taskStation.ReturnReservedItems();
                //Create a hauling task for the item
                HaulingTask task = new HaulingTask {
                    item = requiredItem,
                    from = taskStation.input,
                    to = taskStation
                };
                ReceiveTask(task);
                break;
            }
        }
        //If nothing else is needed, start working
        if (!itemsAreStillNeeded) {
            _crafting = true;
        }
    }

    private void FinishCrafting() { //Should be partially moved to Station
        Debug.Log(name + " has finished crafting " + craftingTask.recipe.displayName + ".");
        List<Item> requirements = craftingTask.recipe.GetRequirements();
        foreach (Item item in requirements) { //Temporary and buggy
            craftingTask.station.RemoveItem(item); //Replace with reservation system
        }
        Item craftedItem = craftingTask.recipe.GetResult();
        craftingTask.station.AddItem(craftedItem);
        haulingTask = new HaulingTask {
            item = craftedItem,
            from = craftingTask.station,
            to = craftingTask.station.output
        };
        craftingTask = new CraftingTask { };
        _crafting = false;
    }
}
