using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with: "+other.gameObject.tag);
        if(other.gameObject.CompareTag("Player")){
            // add score somehow?
            Destroy(this.gameObject);
        }
    }
}
