using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private NavMeshAgent _agent;

    private float _accumulatedLabor = 0.0f;
    private Item _heldItem;
    private Transform _heldTransform;

    private State _state = State.Idle;

    private CraftingTask _craftingTask;
    private HarvestingTask _harvestingTask;
    private HaulingTask _haulingTask;

    public Station station;

    public bool isBusy => _state != State.Idle;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        switch (_state) {
            case State.Crafting:
                _accumulatedLabor += Time.deltaTime;
                if (_accumulatedLabor >= _craftingTask.recipe.requiredLabor) {
                    FinishCrafting();
                    _accumulatedLabor = 0.0f;
                }
                break;
            case State.Harvesting:
                _accumulatedLabor += Time.deltaTime;
                if (_accumulatedLabor >= _harvestingTask.harvestable.requiredLabor) {
                    FinishHarvesting();
                    _accumulatedLabor = 0.0f;
                }
                break;
            case State.FetchingItem:
                Vector2 itemPosition = new Vector2(_haulingTask.from.transform.position.x, _haulingTask.from.transform.position.z);
                if (itemPosition == new Vector2(transform.position.x, transform.position.z)) {
                    //Take item from inventory
                    bool wasItemTaken = TakeItem(_haulingTask.item, _haulingTask.from);
                    if (wasItemTaken) {
                        //Bring item to destination
                        _agent.SetDestination(_haulingTask.to.transform.position);
                        _state = State.DeliveryingItem;
                    }
                    else {
                        //Item wasn't found
                        _state = State.Idle;
                    }
                }
                break;
            case State.DeliveryingItem:
                Vector2 inventoryPosition = new Vector2(_haulingTask.to.transform.position.x, _haulingTask.to.transform.position.z);
                if (inventoryPosition == new Vector2(transform.position.x, transform.position.z)) {
                    //Deposit held item to inventory
                    DepositItem(_haulingTask.to);
                    _haulingTask = null;
                    _state = State.Idle;
                }
                break;
            case State.GoingToStation:
                Vector2 stationPosition = new Vector2(_craftingTask.station.transform.position.x, _craftingTask.station.transform.position.z);
                if (stationPosition == new Vector2(transform.position.x, transform.position.z)) {
                    BeginCrafting();
                }
                break;
            case State.GoingToHarvestable:
                Vector2 harvestablePosition = new Vector2(_harvestingTask.harvestable.transform.position.x, _harvestingTask.harvestable.transform.position.z);
                if (harvestablePosition == new Vector2(transform.position.x, transform.position.z)) {
                    BeginHarvesting();
                }
                break;
            case State.Idle:
                _agent.ResetPath();
                //_agent.SetDestination(new Vector3());
                break;
        }
    }

    public bool TakeItem(Item item, Inventory inventory = null) {
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
            return true;
        }
        return false;
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

    //Receive a generic task
    public void ReceiveTask(Task task) {
        if (task is CraftingTask craftingTask) {
            ReceiveTask(craftingTask);
        } else if (task is HarvestingTask harvestingTask) {
            ReceiveTask(harvestingTask);
        } else if (task is HaulingTask haulingTask) {
            ReceiveTask(haulingTask);
        }
    }

    //Receive a crafting task
    public void ReceiveTask(CraftingTask task) {
        Debug.Log(name + " received crafting task for " + task.recipe.displayName + ".");

        //Repost current task
        RepostTask();

        //Begin new task
        _craftingTask = task;
        _agent.SetDestination(task.station.transform.position);
        _state = State.GoingToStation;
    }

    //Receive a harvesting task
    public void ReceiveTask(HarvestingTask task) {
        Debug.Log(name + " received harvesting task for " + task.harvestable.name + ".");

        //Repost current task
        RepostTask();

        //Begin new task
        _harvestingTask = task;
        _agent.SetDestination(task.harvestable.transform.position);
        _state = State.GoingToHarvestable;
    }

    //Receive a hauling task
    public void ReceiveTask(HaulingTask task) {
        Debug.Log(name + " received hauling task for " + task.item.displayName + ".");

        //Repost current task
        RepostTask();

        //Begin new task
        _haulingTask = task;
        _agent.SetDestination(task.from.transform.position);
        _state = State.FetchingItem;
    }

    private void RepostTask() {
        if (_craftingTask != null) {
            Debug.Log(name + " reposted crafting task for " + _craftingTask.recipe.displayName + ".");
            _craftingTask.station.PostTask(_craftingTask);
            _craftingTask = null;
        }
        if (_harvestingTask != null) {
            Debug.Log(name + " reposted harvesting task for " + _harvestingTask.harvestable.name + ".");
            _harvestingTask.station.PostTask(_harvestingTask);
            _harvestingTask = null;
        }
        if (_haulingTask != null) {
            Debug.Log(name + " reposted a hauling task for " + _haulingTask.item.displayName + ".");
            _haulingTask.station.PostTask(_haulingTask);
            _haulingTask = null;
        }
        _state = State.Idle;
    }

    private void BeginCrafting() {
        //Stop if worker has no crafting task
        if (_craftingTask == null) {
            Debug.Log(name + " has no crafting task to begin.");
            return;
        }

        Debug.Log(name + " has begun crafting " + _craftingTask.recipe.displayName + ".");

        //Prepare a list of needed items
        Station taskStation = _craftingTask.station;
        bool itemsAreStillNeeded = false;
        List<Item> requirements = _craftingTask.recipe.GetRequirements();
        foreach (Item requiredItem in requirements) {
            //Attempt to reserve the item
            if (!taskStation.ReserveItem(requiredItem)) {
                //Mark the item for retrieval
                itemsAreStillNeeded = true;
                taskStation.CreateFetchTask(requiredItem);
            }
        }
        
        taskStation.ReturnReservedItems();

        if (!itemsAreStillNeeded) {
            _state = State.Crafting;
        } else {
            RepostTask();
        }
    }

    private void BeginHarvesting() {
        //Stop if worker has no harvesting task
        if (_harvestingTask == null) {
            Debug.Log(name + " has no harvesting task to begin.");
            return;
        }

        if (_harvestingTask.harvestable.IsAvailable()) {
            Debug.Log(name + " has begun harvesting from " + _harvestingTask.harvestable.name + ".");
            _state = State.Harvesting;
        } else {
            Debug.Log(_harvestingTask.harvestable.name + " has no resources available.");
        }
    }

    private void FinishCrafting() {
        //Stop if worker has no crafting task
        if (_craftingTask == null) {
            Debug.Log(name + " has no crafting task to complete.");
            return;
        }

        //Check requirements (Should happen on station)
        List<Item> requirements = _craftingTask.recipe.GetRequirements();
        foreach (Item item in requirements) { //Temporary and buggy
            _craftingTask.station.RemoveItem(item); //Replace with reservation system
        }

        //Craft the item (Should happen on station)
        Item craftedItem = _craftingTask.recipe.GetResult();
        Debug.Log(name + " has finished crafting " + craftedItem.displayName + ".");
        _craftingTask.station.AddItem(craftedItem);

        //Create hauling task
        _craftingTask.station.CreateDeliveryTask(craftedItem);

        //End the crafting task
        _craftingTask = null;
        _state = State.Idle;
    }

    private void FinishHarvesting() {
        //Stop if worker has no harvesting task
        if (_harvestingTask == null) {
            Debug.Log(name + " has no harvesting task to complete.");
            return;
        }

        //Harvest the item
        Item harvestedItem = _harvestingTask.harvestable.Harvest();
        Debug.Log(name + " has finished harvesting " + harvestedItem.displayName + ".");

        //Create a hauling task
        _harvestingTask.station.CreateTransferTask(harvestedItem, _harvestingTask.harvestable, _harvestingTask.stockpile);

        //End the harvesting task
        _harvestingTask = null;
        _state = State.Idle;
    }

    private enum State {
        Idle,
        GoingToStation,
        GoingToHarvestable,
        FetchingItem,
        DeliveryingItem,
        Crafting,
        Harvesting
    }
}
