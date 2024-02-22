using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class Module : MonoBehaviour
{
    public List<Prototype> PotentialPrototypes = new List<Prototype>();

    [CanBeNull] public Prototype SelectedPrototype;

    private Renderer myRenderer;

    //[HideInInspector]
    public bool isCollapsed = false;

    public int Entropy
    {
        get { 
            if(PotentialPrototypes == null) GetPrototypes();
            return PotentialPrototypes.Count;
        }
        
    }


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
        myRenderer=GetComponent<Renderer>();
        GetPrototypes();
    }

    public void GetPrototypes()
    {
        var prototypeData = AssetDatabase.LoadAssetAtPath<PrototypeData>("Assets/prototypeData.asset");
        if (prototypeData.Prototypes.Count == 0) prototypeData.ReadJson();
        PotentialPrototypes = prototypeData.Prototypes;
    }
    
    public int GetRandomWeightedIndex(List<float> weights)
    {
        // Get the total sum of all the weights.
        float weightSum = weights.Sum();
        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Count - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }
     
            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }
     
        // No other item was selected, so return very last index.
        return index;
    }

    
    public void Collapse()
    { 
        if (PotentialPrototypes.Count == 0) GetPrototypes();
        var weights = new List<float>();
        foreach (var prototype in PotentialPrototypes)
        {
            weights.Add(prototype.weight);
        }

        var choice = GetRandomWeightedIndex(weights);
        SelectedPrototype = PotentialPrototypes[choice];
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
                if (validNeighbours.Contains(neighbour.id) && newPotentialPrototypes.All(p => prototype.id != p.id))
                {
                    newPotentialPrototypes.Add(prototype);
                }
            }

        }

        if (newPotentialPrototypes.Count == PotentialPrototypes.Count) return false;
        PotentialPrototypes = newPotentialPrototypes;

        return true;
    }
    public void MarkAsWalkable(){
        List<Prototype> walkables = new List<Prototype>();
        for(int i =0;i<PotentialPrototypes.Count;i++){
            if(PotentialPrototypes[i].walkable){
                walkables.Add(PotentialPrototypes[i]);
            }
        }
        PotentialPrototypes = walkables;
    }

    public void SetColor(Color color){
        myRenderer=GetComponent<Renderer>();
        myRenderer.material.color=color;
    }
    
}
