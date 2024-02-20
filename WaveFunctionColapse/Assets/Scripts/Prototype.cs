using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Prototype
{
    [System.Serializable]
    public class Face
    {
        [Tooltip("Flag if this is a walkable module or not")]
        public bool walkable;

        [Tooltip("The index value of this face's connector")]
        public int connector;
        
        [Tooltip("The IDs of the neighbours that this face can have")]
        public List<string> validNeighbours;
        
        [Tooltip("Is this face symmetric or not?")]
        public bool symmetric;
        
        [Tooltip("Can this face be flipped or not?")]
        public bool flipped;
    }

    public string id;

    public bool walkable;
    
    [Tooltip("Set how likely the algorithm will be to choose this module")]
    public float weight;

    [Header("Faces")]
    public Face Left;
    
    public Face Right;
    
    public Face Forward;
    
    public Face Back;
    
    public Face[] Faces {
        get {
            return new Face[] {
                this.Left,
                this.Back,
                this.Right,
                this.Forward
            };
        }
        
    }

    [HideInInspector]
    public Quaternion rotation;
    [HideInInspector]
    public Mesh mesh;

    public Prototype(ModuleDataStruct moduleData)
    {
        id = moduleData.module_name;
        
        mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/TileSet/Tile Set/" + moduleData.mesh_name);
        
        rotation = Quaternion.Euler(0f, 90 * moduleData.mesh_rotation, 0f);

        string[] facePos = {moduleData.posX, moduleData.negX, moduleData.posY, moduleData.negY};

        for (int i = 0; i < 4; i++)
        {
            var split = facePos[i].Split();
            Faces[i].connector = int.Parse(split[0]);
            if (split.Length > 1)
            {
                Faces[i].symmetric = split[1] == "s";
                Faces[i].flipped = split[1] == "f";
            }

            Faces[i].validNeighbours = moduleData.valid_neighbours[i];
        }
        
        weight = moduleData.weight;
        walkable = moduleData.walkable;

        
    }
}
