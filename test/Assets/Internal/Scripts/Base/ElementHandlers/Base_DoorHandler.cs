using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_DoorHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] Doors = { };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameManager.completedSteps.Length; i++)
        {
            if (GameManager.completedSteps[i] > 0)
            {
                Doors[i].SetActive(false);
            }

        }
    }

   
}
