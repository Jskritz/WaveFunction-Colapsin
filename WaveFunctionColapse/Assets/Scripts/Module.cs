using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class Module : MonoBehaviour
{
    public List<Prototype> PotentialPrototypes = new List<Prototype>();

    [CanBeNull] public Prototype SelectedPrototype;

    [HideInInspector]
    public bool isCollapsed = false;

    public int Entropy
    {
        get { 
            if(PotentialPrototypes == null) GetPrototypes();
            return PotentialPrototypes.Count;
        }
        
    }

    static Random _rnd = new Random();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        GetPrototypes();
    }

    public void GetPrototypes()
    {
        var prototypeData = AssetDatabase.LoadAssetAtPath<PrototypeData>("Assets/prototypeData.asset");
        if (prototypeData.Prototypes.Count == 0) prototypeData.ReadJson();
        PotentialPrototypes = prototypeData.Prototypes;
    }
    
    public void Collapse()
    { 
        if (PotentialPrototypes.Count == 0) GetPrototypes();
        var choice = _rnd.Next(PotentialPrototypes.Count);
        SelectedPrototype = PotentialPrototypes[choice];
        Debug.Log($"Chose prototype {SelectedPrototype.id}");
        PotentialPrototypes = new List<Prototype>() { SelectedPrototype };
        isCollapsed = true;
        GetComponent<MeshFilter>().mesh = SelectedPrototype.mesh;
        transform.rotation = SelectedPrototype.rotation;
    }

    public bool Constrain(List<Prototype> neighbourPotentials, int direction)
    {
        // constrain the list of potential prototypes based on this new fixed neighbour
        /* this module is the neighbour's neighbour to the _ -> the neighbour is this prototype's _ neighbour
         * 0: left -> right
         * 1: back -> forward
         * 2: right -> left
         * 3: forward -> back
         * return if the constrain updates the module
         */
        if (isCollapsed) return false;
        var newPotentialPrototypes = new List<Prototype>();
        foreach (var prototype in PotentialPrototypes)
        {
            var validNeighbours = new List<string>();
            switch (direction)
            {
                case 0:
                    validNeighbours = prototype.Faces[2].validNeighbours;
                    break;
                case 1:
                    validNeighbours = prototype.Faces[3].validNeighbours;
                    break;
                case 2:
                    validNeighbours = prototype.Faces[0].validNeighbours;
                    break;
                case 3:
                    validNeighbours = prototype.Faces[1].validNeighbours;
                    break;
            }

            foreach (var neighbour in neighbourPotentials)
            {
                if (validNeighbours.Contains(neighbour.id) && !newPotentialPrototypes.Contains(neighbour))
                {
                    newPotentialPrototypes.Add(prototype);
                }
            }
            
        }

        if (newPotentialPrototypes == PotentialPrototypes) return false;
        PotentialPrototypes = newPotentialPrototypes;

        return true;
    }
    
}
