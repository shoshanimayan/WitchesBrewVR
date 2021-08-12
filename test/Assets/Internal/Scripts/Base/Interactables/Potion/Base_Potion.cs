using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Potion : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 eulerOrigin;
    public int index;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            transform.position = origin;
            transform.localEulerAngles = eulerOrigin;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
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
