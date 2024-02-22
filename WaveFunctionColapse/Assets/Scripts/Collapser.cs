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
    public List<List<GameObject>> _moduleObjects = new List<List<GameObject>>();
    private List<List<Module>> _modules = new List<List<Module>>();
    
    
    static Random _rnd = new Random();
    
    private WaveSettings settings;

    public List<List<GameObject>> GetModules(){
        return _moduleObjects;
    }

// Start is called before the first frame update
    void Start()
    {
        //if(_moduleObjects.Count == 0) BuildEmptyWave();
        //if(_moduleObjects == null) Debug.Log("oh no, i have no modules");
        //if (!IsCollapsed()) InvokeRepeating(nameof(DoIterate), 0.1f, settings.speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollapseVisually()
    {
        
        if (!IsCollapsed()) InvokeRepeating(nameof(DoIterate), 0.1f, settings.speed);
    }
    
    // Initialise the wave populating each cell with an empty module
    public void BuildEmptyWave()
    {
        if(_moduleObjects == null)ClearWave();
        else if (_moduleObjects.Count != 0) ClearWave();
        settings = GetComponent<WaveSettings>();
        var pos = transform.position;
        for (int x = 0; x < settings.x / settings.scale; x++)
        {
            var xpos = (x + 0.5f) * settings.scale + pos.x;
            var row = new List<GameObject>();
            var mrow = new List<Module>();
            for (int z = 0; z < settings.z / settings.scale; z++)
            {
                var zpos = (z + 0.5f) * settings.scale + pos.z;
                
                var newModule = Instantiate(module, new Vector3(xpos, (pos.y + 0.5f) * settings.scale, zpos),
                    Quaternion.identity);
                var newModuleModule = newModule.GetComponent<Module>();
                newModuleModule.GetPrototypes();
                newModule.transform.parent = this.transform;
                row.Add(newModule);
                mrow.Add(newModuleModule);
            }
            _moduleObjects.Add(row);
            _modules.Add(mrow);
        }
    }

    public void ClearWave()
    {
        foreach (var row in _moduleObjects)
        {
            foreach (var oldModule in row)
            {
                #if UNITY_EDITOR
                DestroyImmediate(oldModule);
                #else
                Destroy(OldModule);
                #endif
                
            }

            _moduleObjects = new List<List<GameObject>>();
        }
    }

    public void Collapse()
    {
        var count = 0;
        while (!IsCollapsed())
        {
            var time = Time.realtimeSinceStartup;
            Iterate();
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
        List<Module> lowest = new List<Module>();
        List<List<int>> lowestCoords = new List<List<int>>();
        var time = Time.realtimeSinceStartup;
        // find the modules with the lowest entropy (naively, so very naively)
        for (int z = 0; z < settings.z / settings.scale; z++)
        {
            for (int x = 0; x < settings.x / settings.scale; x++)
            {
                var m_module = _modules[x][z];
                if (m_module.isCollapsed) continue;
                if (lowest.Count == 0)
                {
                    lowest.Add(_modules[x][z]);
                    lowestCoords.Add(new List<int>() {x, z});
                    continue;
                }

                var lowestEnt = lowest.Last().Entropy;
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
        //Debug.Log("Chose a module to collapse in " + (Time.realtimeSinceStartup - time) + "seconds");
        time = Time.realtimeSinceStartup;
        lowestModule.Collapse();
        //Debug.Log("Collapsed that module in " + (Time.realtimeSinceStartup - time) + "seconds");
        #endregion
        
        #region Propagate
        // propagate the collapse
        Queue<List<int>> queue = new Queue<List<int>>();
        List<List<int>> history = new List<List<int>>();
        queue.Enqueue(lowestModuleCoords);
        time = Time.realtimeSinceStartup;
        var count = 0;
        while (queue.Count > 0)
        {
            
            var currCoords = queue.Dequeue();
            //Debug.Log($"Starting at cell ({currCoords[0]},{currCoords[1]}):");
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
                var current = _modules[currCoords[0]][currCoords[1]];
                var innerTime = Time.realtimeSinceStartup;
                if(neighbour.Constrain(current.PotentialPrototypes, (int)neighbourDir))
                {
                    if (!history.Any(c => c[0] == x && c[1] == z))
                    {
                        queue.Enqueue(new List<int>(){x, z});
                        history.Add(new List<int>(){x, z});
                    }
                }
                //Debug.Log($"Constrained in {Time.realtimeSinceStartup - innerTime} seconds");
            }

            count++;
            if (count > 100) break;
        }
        
        //Debug.Log("Propagated the collapse in "+ (Time.realtimeSinceStartup - time) + "seconds, visited: " + count + " cells");
        #endregion
    }
    
    private bool IsCollapsed()
    {
        foreach (var row in _modules)
        {
            foreach (var _module in row)
            {
                if (!_module.isCollapsed)
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
                linkedObject.CollapseVisually();
            }

            if (GUILayout.Button("Iterate"))
            {
                linkedObject.Iterate();
            }
        }
    }
}

