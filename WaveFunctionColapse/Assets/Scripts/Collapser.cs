using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class Collapser : MonoBehaviour
{
    public GameObject module;
    private List<List<GameObject>> _modules;
    
    static Random _rnd = new Random();

    private WaveSettings settings;

// Start is called before the first frame update
    void Start()
    {
        BuildEmptyWave();
        Collapse();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Initialise the wave populating each cell with an empty module
    public void BuildEmptyWave()
    {
        if (_modules.Count != 0) ClearWave();
        settings = GetComponent<WaveSettings>();
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
            _modules.Add(row);
        }
    }

    private void ClearWave()
    {
        foreach (var row in _modules)
        {
            foreach (var oldModule in row)
            {
                #if UNITY_EDITOR
                DestroyImmediate(oldModule);
                #else
                Destroy(OldModule);
                #endif
                
            }

            _modules = new List<List<GameObject>>();
        }
    }

    private void Collapse()
    {
        while (!IsCollapsed())
        {
            Iterate();
        }
    }

    private void Iterate()
    {
        List<GameObject> lowest = new List<GameObject>();
        List<List<int>> lowestCoords = new List<List<int>>();
        // find the modules with the lowest entropy (naively, so very naively)
        for (int z = 0; z < settings.z; z++)
        {
            for (int x = 0; x < settings.x; x++)
            {
                if (lowest[0].GetComponent<Module>() == null)
                {
                    lowest.Add(_modules[z][x]);
                    lowestCoords.Add(new List<int>() {x, z});
                    continue;
                }
                var m_module = _modules[z][x].GetComponent<Module>();
                if (m_module.Entropy <= lowest[0].GetComponent<Module>().Entropy)
                {
                    lowest.Add(_modules[z][x]);
                    lowestCoords.Add(new List<int>() { x, z });
                }
            }
        }
        // randomly select a lowest entropy module and collapse it
        var choice = _rnd.Next(lowest.Count);
        var lowestModule = lowest[choice];
        var lowestModuleCoords = lowestCoords[choice];
        var lowestModuleModule = lowestModule.GetComponent<Module>();
        lowestModuleModule.Collapse();
        
        // propagate the collapse
        Stack<List<int>> stack = new Stack<List<int>>();
        stack.Push(lowestModuleCoords);
        while (stack.Count > 0)
        {
            var currCoords = stack.Pop();
            foreach (var neighbourDir in GetNeighbours(currCoords))
            {
                var x = lowestModuleCoords[0];
                var z = lowestModuleCoords[1];
                switch (neighbourDir)
                {
                    case NeighbourDir.Left:
                        x -= 1;
                        break;
                    case NeighbourDir.Back:
                        z -= 1;
                        break;
                    case NeighbourDir.Right:
                        x += 1;
                        break;
                    case NeighbourDir.Forward:
                        z += 1;
                        break;
                
                }
                var neighbour = _modules[x][z];
                if (neighbour.GetComponent<Module>().Constrain(lowestModuleModule.SelectedPrototype, (int)neighbourDir))
                {
                    stack.Push(new List<int>() { x, z });
                }
            }
        }
    }
    
    private bool IsCollapsed()
    {
        foreach (var row in _modules)
        {
            foreach (var moduleObject in row)
            {
                var mod = moduleObject.GetComponent<Module>();
                if (!mod.isCollapsed)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private List<NeighbourDir> GetNeighbours(List<int> coords)
    {
        var neighbours = new List<NeighbourDir>();
        if(coords[0] + 1 < settings.x) neighbours.Add(NeighbourDir.Left);
        if(coords[0] - 1 > 0) neighbours.Add(NeighbourDir.Right);
        if(coords[1] + 1 < settings.z) neighbours.Add(NeighbourDir.Forward);
        if(coords[1] -1 > 0) neighbours.Add(NeighbourDir.Back);
        return neighbours;
    }
    
    private enum NeighbourDir
    {
        Left,
        Back,
        Right,
        Forward
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

            if (GUILayout.Button("Do it!"))
            {
                linkedObject.Collapse();
            }
        }
    }
}

