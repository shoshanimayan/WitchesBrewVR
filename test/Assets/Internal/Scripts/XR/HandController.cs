using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference controllerActionGrip;
    [SerializeField] InputActionReference controllerActionTrigger;
    [SerializeField] Mesh closedHand;
    [SerializeField] Mesh openHand;

 
    private void Awake()
    {
        controllerActionGrip.action.started += Toggle;
        controllerActionTrigger.action.started += Toggle;
        controllerActionGrip.action.canceled += Untoggle;
        controllerActionTrigger.action.canceled += Untoggle;

    }

    private void Toggle(InputAction.CallbackContext context)
    {
        GetComponent<ActionBasedController>().model.GetComponent<MeshFilter>().sharedMesh = closedHand;
     
    }
    private void Untoggle(InputAction.CallbackContext context)
    {
        GetComponent<ActionBasedController>().model.GetComponent<MeshFilter>().sharedMesh = openHand;
    }





}
