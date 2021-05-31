using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private bool _crafting = false;
    private bool _harvesting = false;
    private float _accumulatedLabor = 0.0f;

    private Queue<Task> _tasklist;
    private Item _heldItem;
    private Transform _heldTransform;

    private NavMeshAgent _agent;

    public Station station;
    public CraftingTask craftingTask;
    public HaulingTask haulingTask;
    public HarvestingTask harvestingTask;


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
        //If the worker is harvesting an item
        else if (_harvesting) {
            _accumulatedLabor += Time.deltaTime;
            float itemCompletion = _accumulatedLabor / harvestingTask.harvestable.requiredLabor * 100.0f;
            if (_accumulatedLabor >= harvestingTask.harvestable.requiredLabor) {
                FinishHarvesting();
                _accumulatedLabor = 0.0f;
            }
        }
        //If the worker needs to get an item
        else if (haulingTask != null && !_heldItem) {
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
        else if (haulingTask != null && _heldItem) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 inventoryPosition = new Vector2(haulingTask.to.transform.position.x, haulingTask.to.transform.position.z);
            if (workerPosition == inventoryPosition) {
                //Deposit held item to inventory
                DepositItem(haulingTask.to);
                haulingTask = null;
                //Return to station if needed
                if (craftingTask != null) {
                    _agent.SetDestination(craftingTask.station.transform.position);
                }
            }
        }
        //If the worker needs to craft an item
        else if (craftingTask != null) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(craftingTask.station.transform.position.x, craftingTask.station.transform.position.z);
            if (workerPosition == stationPosition) {
                BeginCrafting();
            }
        }
        //If the worker needs to harvest an item
        else if (harvestingTask != null) {
            Vector2 workerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 stationPosition = new Vector2(harvestingTask.harvestable.transform.position.x, harvestingTask.harvestable.transform.position.z);
            if (workerPosition == stationPosition) {
                BeginHarvesting();
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

    public void ReceiveTask(Task task) {
        if (task is CraftingTask craftingTask) {
            ReceiveTask(craftingTask);
        } else if (task is HaulingTask haulingTask) {
            ReceiveTask(haulingTask);
        } else if (task is HarvestingTask harvestingTask) {
            ReceiveTask(harvestingTask);
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

    public void ReceiveTask(HarvestingTask task) {
        Debug.Log(name + " received harvesting order for " + task.harvestable.name + ".");
        harvestingTask = task;
        _agent.SetDestination(task.harvestable.transform.position);
    }

    private void BeginCrafting() {
        //Stop if worker has no crafting task
        if (craftingTask == null) {
            Debug.Log(name + " has no crafting task to begin.");
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

    private void BeginHarvesting() {
        //Stop if worker has no harvesting task
        if (harvestingTask == null) {
            Debug.Log(name + " has no harvesting task to begin.");
            return;
        }

        if (harvestingTask.harvestable.IsAvailable()) {
            Debug.Log(name + " has begun harvesting from " + harvestingTask.harvestable.name + ".");
            _harvesting = true;
        } else {
            Debug.Log(harvestingTask.harvestable.name + " has no resources available.");
        }
    }

    private void FinishCrafting() { //Should be partially moved to Station
        //Stop if worker has no crafting task
        if (craftingTask == null) {
            Debug.Log(name + " has no crafting task to complete.");
            return;
        }

        //Check requirements
        List<Item> requirements = craftingTask.recipe.GetRequirements();
        foreach (Item item in requirements) { //Temporary and buggy
            craftingTask.station.RemoveItem(item); //Replace with reservation system
        }

        //Craft the item
        Item craftedItem = craftingTask.recipe.GetResult();
        Debug.Log(name + " has finished crafting " + craftedItem.displayName + ".");

        //Create hauling task
        craftingTask.station.AddItem(craftedItem);
        haulingTask = new HaulingTask {
            item = craftedItem,
            from = craftingTask.station,
            to = craftingTask.station.output
        };

        craftingTask = null;
        _crafting = false;
    }

    private void FinishHarvesting() {
        //Stop if worker has no harvesting task
        if (harvestingTask == null) {
            Debug.Log(name + " has no harvesting task to complete.");
            return;
        }

        //Harvest the item
        Item harvestedItem = harvestingTask.harvestable.Harvest();
        Debug.Log(name + " has finished harvesting " + harvestedItem.displayName + ".");

        //Create a hauling task
        haulingTask = new HaulingTask {
            item = harvestedItem,
            from = harvestingTask.harvestable,
            to = harvestingTask.station.output
        };

        //Remove harvesting task
        harvestingTask = null;
        _harvesting = false;
    }
}
