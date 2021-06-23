using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementController : LocomotionProvider
{
    public float gravityMultiplier = 1f;
    public float speed = 1f;
    private CharacterController characterController = null;
    private GameObject head = null;
    [SerializeField] InputActionReference controllerJoyStick;

    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
        controllerJoyStick.action.performed += TryMove;
    }

    private void TryMove(InputAction.CallbackContext obj)
    {
        Vector2 position = obj.ReadValue<Vector2>();
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);
        direction = Quaternion.Euler(headRotation) * direction;
        Vector3 movement = direction * speed;
        characterController.Move(movement * Time.deltaTime);
    }

    private void Start()
    {
        PositionController();
    }

    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;
        characterController.Move(gravity * Time.deltaTime);
    }
    void Update()
    {
        PositionController();
        ApplyGravity();
    }


    private void PositionController()
    {
        float headHight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHight;
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;
        characterController.center = newCenter;
    }

   




}
