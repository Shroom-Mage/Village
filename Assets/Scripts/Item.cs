using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item")]
public class Item : MonoBehaviour
{
    [SerializeField]
    private string _displayName;

    public string displayName => _displayName;
}
