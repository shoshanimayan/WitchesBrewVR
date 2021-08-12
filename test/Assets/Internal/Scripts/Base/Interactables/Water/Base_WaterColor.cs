using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_WaterColor : MonoBehaviour
{

    [SerializeField] private Material[] _materials;
    [SerializeField] private Color[] _colors;
    [SerializeField]private ParticleSystem _particleSystem;
    private Base_EndHandler _end;

    private void Awake()
    {
        _end = GetComponent<Base_EndHandler>();

        ApplyColor(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ApplyColor(true);
        }
    }


    public void ApplyColor(bool smoke=true)
    {
        int index=-1;
        for (int i = 0; i < GameManager.completedSteps.Length; i++)
        {
            if (GameManager.completedSteps[i] >= 2)
            {
                index++;
            }
        }
        if (index >= 0)
        {
            Debug.Log(index);
            Debug.Log(_materials.Length);
            gameObject.GetComponent<Renderer>().material = _materials[index];
            if (smoke)
            {
                ParticleSystem.MainModule main = _particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(_colors[index].r, _colors[index].g, _colors[index].b, 100));
                _particleSystem.gameObject.SetActive(true);

            }
            if (index >= GameManager.completedSteps.Length-1)
            {
                _end.InitiateEnding(_particleSystem.main.duration);
            }
        }
    }
}
