using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;    
    }
}
