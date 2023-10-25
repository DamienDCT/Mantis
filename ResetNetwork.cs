using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ResetNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject prefabNetwork;

    private void Start()
    {
        if(NetworkManagerUI.Instance == null)
            return;


        Destroy(NetworkManagerUI.Instance.gameObject);
        StartCoroutine(SpawnObject());
    }
    

    private IEnumerator SpawnObject()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Instantiate(prefabNetwork);
    }    
}
