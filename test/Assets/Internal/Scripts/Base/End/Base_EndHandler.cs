using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Base_EndHandler : MonoBehaviour
{


    [SerializeField] private GameObject _textObject;
    [SerializeField] private float scrollSpeed=.5f;
    [SerializeField] private float scrollTime=10;
    [SerializeField] private float scrollStep = .1f;
    private Vector3 target;

    private bool ending;
    private void Awake()
    {
        target = new Vector3(_textObject.transform.position.x,3, _textObject.transform.position.z);
        _textObject.SetActive(false);
    }

    private void Update()
    {
        if(ending)
        {

            _textObject.transform.LookAt(Camera.main.transform);
            float step = scrollSpeed * Time.deltaTime;
            _textObject.transform.position = Vector3.MoveTowards(_textObject.transform.position, target, step);
            if (Vector3.Distance(_textObject.transform.position, target) < 1)
            { ending = !ending; }
        }
    }

    

    public void InitiateEnding(float waitTime)
    {
        Debug.Log("ending");
        _textObject.SetActive(true);
        ending = true;


    }

}
