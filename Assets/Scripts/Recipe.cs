using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private Resource _resourceA;
    [SerializeField]
    private int _resourceAQuantity;
    [SerializeField]
    private Resource _resourceB;
    [SerializeField]
    private int _resourceBQuantity;
    [SerializeField]
    private Resource _resourceC;
    [SerializeField]
    private int _resourceCQuantity;
    [SerializeField]
    private float _labor = 0.0f;

    [SerializeField]
    private Resource _output;
}
