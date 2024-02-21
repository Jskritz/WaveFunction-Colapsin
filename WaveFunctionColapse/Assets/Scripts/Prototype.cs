using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Prototype : IEquatable<Prototype>
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

    public Prototype(PrototypeDataStruct prototypeData)
    {
        Left = new Face();
        Right = new Face();
        Forward = new Face();
        Back = new Face();
        id = prototypeData.prototype_name;

        
        mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Tile Set/" + prototypeData.mesh_name + ".fbx");
        
        rotation = Quaternion.Euler(0f, -90 * prototypeData.mesh_rotation, 0f);

        string[] facePos = {prototypeData.posX, prototypeData.negX, prototypeData.posY, prototypeData.negY};

        for (int i = 0; i < 4; i++)
        { 
            Faces[i].connector = (int)Char.GetNumericValue(facePos[i][0]);
            if (facePos[i].Length > 1)
            {
                Faces[i].symmetric = facePos[i][1] == 's';
                Faces[i].flipped = facePos[i][1] == 'f';
            }

            Faces[i].validNeighbours = prototypeData.valid_neighbours[i];
        }
        
        weight = prototypeData.weight;
        walkable = prototypeData.Walkable;

        
    }

    public bool Equals(Prototype other)
    {
        return this.id == other.id;
    }
}
