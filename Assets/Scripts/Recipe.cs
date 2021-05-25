using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private List<Item> _requirements;
    [SerializeField]
    private float _labor;

    [SerializeField]
    private Item _output;
    [SerializeField]
    private int _outputQuantity;

    public string displayName => _output.displayName;
    public float requiredLabor => _labor;

    public List<Item> GetRequirements() {
        List<Item> input = new List<Item>();
        foreach (Item item in _requirements) {
            input.Add(item);
        }
        return input;
    }

    public Item GetResult() {
        return _output;
    }
}
