using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class VRMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1, rotSpeed = 1;

    private PlayerInput playerInput;

    private Vector2 moveDir = Vector2.zero;

    private Vector2 rotDir = Vector2.zero;

    private void Start()
    {
        this.playerInput = new PlayerInput();
        this.playerInput.Enable();

        this.playerInput.Player.Move.performed += (c) => moveDir = c.ReadValue<Vector2>();
        this.playerInput.Player.Move.canceled += (c) => moveDir = c.ReadValue<Vector2>();

        this.playerInput.Player.Rot.performed += (c) =>
        {
            this.rotDir = c.ReadValue<Vector2>();
            this.rotDir = new Vector2(Mathf.Clamp(this.rotDir.x, -1, 1), Mathf.Clamp(this.rotDir.y, -1, 1));
        };
        this.playerInput.Player.Rot.canceled += (c) =>
        {
            this.rotDir = c.ReadValue<Vector2>();
            this.rotDir = new Vector2(Mathf.Clamp(this.rotDir.x, -1, 1), Mathf.Clamp(this.rotDir.y, -1, 1));
        };
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotDir.x * rotSpeed * Time.deltaTime);

        Vector3 move = this.transform.forward * this.moveDir.y + this.transform.right * this.moveDir.x;
        move.Normalize();
        this.transform.position += move * this.moveSpeed * Time.deltaTime;
    }

}
