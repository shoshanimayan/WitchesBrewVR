using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;


public class DoorTeleport : MonoBehaviour
{
    [SerializeField]
    private AssetReference scene;
    [SerializeField]

    private SceneLoader loader;
    private void Start()
    {
        loader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
    }

    public void LoadLevel()
    {
        Debug.Log("loading...");
        loader.Load(scene);
    }
}
