using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class Base_PotionHandler : MonoBehaviour
{
    [SerializeField] private AssetReference[] prefabReference ={};
    [SerializeField] private Transform[] prefabPlaces = { };
 

    void Start()
    {
        
        for (int i = 0; i < GameManager.completedSteps.Length; i++)
        {
            if (GameManager.completedSteps[i] == 1)
            {
                int index = i;
                Addressables.InstantiateAsync(prefabReference[i], prefabPlaces[i].position,Quaternion.identity).Completed+= handler =>OnLoadDone(handler,index);
            }
        }
    }


    private void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj, int index )
    {
        if (obj.Result == null)
        {
            Debug.LogError("failed to load in addressable asset");
        }
        else 
        {
            GameObject temp_potionObj = obj.Result;
            Base_Potion temp_potionClass = temp_potionObj.GetComponent<Base_Potion>();
            temp_potionClass.origin = temp_potionObj.transform.position;
            temp_potionClass.eulerOrigin = temp_potionObj.transform.localEulerAngles;

            temp_potionClass.index = index;
        }
    }


}
