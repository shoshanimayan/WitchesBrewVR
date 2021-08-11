using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_WaterColor : MonoBehaviour
{

    [SerializeField] private Material[] _materials;
    [SerializeField] private Color[] _colors;

    private ParticleSystem _particleSystem;
    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        ApplyColor(false);
    }

  

    

    public void ApplyColor(bool smoke=true)
    {
        int index=0;
        for (int i = 0; i < GameManager.completedSteps.Length; i++)
        {
            if (GameManager.completedSteps[i] >= 2)
            {
                index++;
            }
        }

        gameObject.GetComponent<Renderer>().material = _materials[index];
        if (smoke)
        {
            //ParticleSystemRenderer pr = _particleSystem.GetComponent<ParticleSystemRenderer>();
            ParticleSystem.MainModule main = _particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(_colors[index]);
           // pr.material= _materials[index]; 
            _particleSystem.Play();

        }
    }
}
