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
    public List<List<GameObject>> _modules = new List<List<GameObject>>();
    
    static Random _rnd = new Random();

    private WaveSettings settings;

    public List<List<GameObject>> GetModules(){
        return _modules;
    }

// Start is called before the first frame update
    void Start()
    {
        if(_modules.Count == 0) BuildEmptyWave();
        if (!IsCollapsed()) InvokeRepeating(nameof(DoIterate), 0.5f, settings.speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void collapsevisually()
    {
        
        if (!IsCollapsed()) InvokeRepeating(nameof(DoIterate), 0.5f, settings.speed);
    }
    
    // Initialise the wave populating each cell with an empty module
    public void BuildEmptyWave()
    {
        if (_modules.Count != 0) ClearWave();
        settings = GetComponent<WaveSettings>();
        var pos = transform.position;
        for (int x = 0; x < settings.x / settings.scale; x++)
        {
            var xpos = (x + 0.5f) * settings.scale + pos.x;
            var row = new List<GameObject>();
            for (int z = 0; z < settings.z / settings.scale; z++)
            {
                var zpos = (z + 0.5f) * settings.scale + pos.z;
                
                var newModule = Instantiate(module, new Vector3(xpos, (pos.y + 0.5f) * settings.scale, zpos),
                    Quaternion.identity);
                newModule.GetComponent<Module>().GetPrototypes();
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
        var count = 0;
        while (!IsCollapsed())
        {
            var time = Time.realtimeSinceStartup;
            Iterate();
            Debug.Log("Completed one iteration in " + (Time.realtimeSinceStartup - time) + " seconds");
            count++;
            if (count > 1000) break;
        }
    }

    private void DoIterate()
    {
        if (IsCollapsed()) return;
        Iterate();
    }
    
    
    private void Iterate()
    {
        #region Collapsing
        List<GameObject> lowest = new List<GameObject>();
        List<List<int>> lowestCoords = new List<List<int>>();
        // find the modules with the lowest entropy (naively, so very naively)
        for (int z = 0; z < settings.z / settings.scale; z++)
        {
            for (int x = 0; x < settings.x / settings.scale; x++)
            {
                var m_module = _modules[x][z].GetComponent<Module>();
                if (m_module.isCollapsed) continue;
                if (lowest.Count == 0)
                {
                    lowest.Add(_modules[x][z]);
                    lowestCoords.Add(new List<int>() {x, z});
                    continue;
                }

                var lowestEnt = lowest.Last().GetComponent<Module>().Entropy;
                if (m_module.Entropy < lowestEnt)
                {
                    lowest.Clear();
                    lowestCoords.Clear();
                    lowest.Add(_modules[x][z]);
                    lowestCoords.Add(new List<int>() { x, z });
                }
                else if (m_module.Entropy == lowestEnt)
                {
                    lowest.Add(_modules[x][z]);
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
        #endregion
        
        #region Propagate
        // propagate the collapse
        Queue<List<int>> queue = new Queue<List<int>>();
        List<List<int>> history = new List<List<int>>();
        queue.Enqueue(lowestModuleCoords);
        var count = 0;
        while (queue.Count > 0)
        {
            
            var currCoords = queue.Dequeue();
            foreach (var neighbourDir in GetNeighbours(currCoords))
            {
                var x = currCoords[0];
                var z = currCoords[1];
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
                var current = _modules[currCoords[0]][currCoords[1]].GetComponent<Module>();
                if(neighbour.GetComponent<Module>().Constrain(current.PotentialPrototypes, (int)neighbourDir))
                {
                    var _outputstr = "";
                    foreach (var h in history)
                    {
                        _outputstr += $"({h[0]},{h[1]}), ";
                    }
                    if (!history.Any(c => c[0] == x && c[1] == z))
                    {
                        queue.Enqueue(new List<int>(){x, z});
                        history.Add(new List<int>(){x, z});
                    }
                }
                
            }

            count++;
            if (count > 100) break;
        }
        #endregion
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
        if(coords[0] + 1 < settings.x / settings.scale) neighbours.Add(NeighbourDir.Right);
        if(coords[0] - 1 >= 0) neighbours.Add(NeighbourDir.Left);
        if(coords[1] + 1 < settings.z / settings.scale) neighbours.Add(NeighbourDir.Forward);
        if(coords[1] -1 >= 0) neighbours.Add(NeighbourDir.Back);
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
                //linkedObject.Collapse();
                linkedObject.collapsevisually();
            }

            if (GUILayout.Button("Iterate"))
            {
                linkedObject.Iterate();
            }
        }
    }
}

