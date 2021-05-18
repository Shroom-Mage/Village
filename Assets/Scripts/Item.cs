using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    private string _displayName;

    [SerializeField]
    private Transform _prefab;

    public string displayName => _displayName;
}
