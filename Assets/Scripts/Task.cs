using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task {
    public Station station;
}

public class CraftingTask : Task {
    public Recipe recipe;
}

public class HaulingTask : Task {
    public Item item;
    public Inventory from;
    public Inventory to;
}

public class HarvestingTask : Task {
    public Harvestable harvestable;
    public Stockpile stockpile;
}
