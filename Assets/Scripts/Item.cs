using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    private string _displayName;

    public string displayName => _displayName;
}
