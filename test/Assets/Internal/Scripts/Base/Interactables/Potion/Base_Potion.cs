using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Potion : MonoBehaviour
{
    public Vector3 origin;
    public int index;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            transform.position = origin;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            GameManager.completedSteps[index]++;
            other.gameObject.GetComponent<Base_WaterColor>().ApplyColor();
            Destroy(gameObject);
        }
    }
}
