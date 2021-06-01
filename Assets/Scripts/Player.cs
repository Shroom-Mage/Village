using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3.5f;
    public float jumpForce = 5.0f;
    public float gravityUp = 5.0f;
    public float gravityDown = 4.0f;

    private CharacterController _controller;
    private Controls _inputActions;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _inputActions = new Controls();
        _inputActions.Enable();
    }

    private void Update() {
        Vector2 move = _inputActions.Player.Move.ReadValue<Vector2>() * speed;
        _controller.SimpleMove(new Vector3(move.x, 0.0f, move.y));
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "Station":
                Station station = other.GetComponent<Station>();
                switch (station.type) {
                    case Station.Type.Crafting:
                        station.CreateCraftingTask();
                        break;
                    case Station.Type.Hauling:
                        break;
                    case Station.Type.Harvesting:
                        station.CreateHarvestingTask();
                        break;
                }
                break;
        }
    }

}
