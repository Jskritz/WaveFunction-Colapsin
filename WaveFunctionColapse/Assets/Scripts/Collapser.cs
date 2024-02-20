using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Collapser : MonoBehaviour
{
    public GameObject module;
    //[HideInInspector]
    public List<List<GameObject>> modules = new List<List<GameObject>>();
    
    // Start is called before the first frame update
    void Start()
    {
        BuildEmptyWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<List<GameObject>> getGrid(){
        return modules;
    }
    
    // Initialise the wave populating each cell with an empty module
    public void BuildEmptyWave()
    {
        if (modules.Count != 0) ClearWave();
        var settings = GetComponent<WaveSettings>();
        var pos = transform.position;
        for (int x = 0; x < settings.x; x++)
        {
            var xpos = x + pos.x + settings.scale / 2;
            var row = new List<GameObject>();
            for (int z = 0; z < settings.z; z++)
            {
                var zpos = z + pos.z + settings.scale / 2;
                
                var newModule = Instantiate(module, new Vector3(xpos, pos.y, zpos),
                    Quaternion.identity);
                newModule.transform.parent = this.transform;
                row.Add(newModule);
            }
            modules.Add(row);
        }
    }

    private void ClearWave()
    {
        foreach (var row in modules)
        {
            foreach (var oldModule in row)
            {
                #if UNITY_EDITOR
                DestroyImmediate(oldModule);
                #else
                Destroy(OldModule);
                #endif
                
            }

            modules = new List<List<GameObject>>();
        }
    }

    [CustomEditor(typeof(Collapser))]
    public class CollapserEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var linkedObject = (Collapser)target;

            if (GUILayout.Button("Make Empty Wave"))
            {
                linkedObject.BuildEmptyWave();
            }

            if (GUILayout.Button("Clear Wave"))
            {
                linkedObject.ClearWave();
            }
        }
    }
}

