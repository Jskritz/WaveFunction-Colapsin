using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class Module : MonoBehaviour
{
    public List<Prototype> PotentialPrototypes;

    [CanBeNull] public Prototype SelectedPrototype;

    public bool isCollapsed = false;

    public int Entropy => PotentialPrototypes.Count;

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
        var moduleData = AssetDatabase.LoadAssetAtPath<ModuleData>("Assets/moduleData.asset");
        PotentialPrototypes = moduleData.modules;
    }

    public void Collapse()
    {
        // randomly select one of the potential prototypes to be the selected prototype
         SelectedPrototype = PotentialPrototypes[_rnd.Next(PotentialPrototypes.Count)];
         PotentialPrototypes = new List<Prototype>() { SelectedPrototype };
         isCollapsed = true;
         GetComponent<MeshFilter>().mesh = SelectedPrototype.mesh;
         transform.rotation = SelectedPrototype.rotation;
    }

    public bool Constrain(Prototype neighbour, int direction)
    {
        // constrain the list of potential prototypes based on this new fixed neighbour
        /* this module is the neighbour's neighbour to the _ -> the neighbour is this prototype's _ neighbour
         * 0: left -> right
         * 1: back -> forward
         * 2: right -> left
         * 3: forward -> back
         * return if the constrain updates the module
         */
        var updated = false;
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

            if (!validNeighbours.Contains(neighbour.id))
            {
                PotentialPrototypes.Remove(prototype);
                updated = true;
            }
        }

        return updated;
    }
    
}
