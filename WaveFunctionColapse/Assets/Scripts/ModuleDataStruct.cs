using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ModuleDataStruct
{
    public string module_name;
    public string mesh_name;
    public int mesh_rotation;
    public string posX;
    public string negX;
    public string posY;
    public string negY;
    public string constrain_to;
    public string constrain_from;
    public float weight;
    public bool walkable;
    public List<List<string>> valid_neighbours;
}