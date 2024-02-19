using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGizmo : MonoBehaviour
{
    private Renderer myRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        myRenderer=GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color){
        myRenderer.material.color = color;
    }
}
