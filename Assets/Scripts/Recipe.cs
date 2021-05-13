using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private Item _inputA;
    [SerializeField]
    private int _inputAQuantity;
    [SerializeField]
    private Item _inputB;
    [SerializeField]
    private int _inputBQuantity;
    [SerializeField]
    private Item _inputC;
    [SerializeField]
    private int _inputCQuantity;
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
        if (_inputA) input.Add(_inputA);
        if (_inputB) input.Add(_inputB);
        if (_inputC) input.Add(_inputC);
        return input;
    }

    public Item GetResult() {
        return _output;
    }
}
