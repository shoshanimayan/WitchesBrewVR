using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class VRButton : MonoBehaviour
{
    [SerializeField] private float freshhold=.1f;
    [SerializeField] private float deadZone = .025f;
    [SerializeField] private string Label;
    private bool _isPressed;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;
    private TMP_Text _label;

    public UnityEvent onPress, onRelease;

    private void Awake()
    {
        _label = transform.Find("Clicker").Find("Canvas").Find("label").GetComponent<TMP_Text>();

    }

    // Start is called before the first frame update
    void Start()
    {
        _label.text = Label;
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isPressed && GetValue() + freshhold >= 1)
        {
            Pressed();
        }

        if (_isPressed && GetValue() - freshhold <= 0)
        {
            Released();
        }
    }

    private float GetValue()
    {
        var value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;
        if (Mathf.Abs(value) < deadZone)
        { value = 0; }
        return Mathf.Clamp(value,-1f,1f);
    }

    private void Pressed()
    {
        Debug.Log("press");
        _isPressed = true;
        onPress.Invoke();

    }

    private void Released()
    {
        Debug.Log("release");

        _isPressed = false;
        onRelease.Invoke();
    }
}
