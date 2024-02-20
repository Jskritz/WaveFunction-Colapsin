using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    [System.Serializable]
    public class Face
    {
        [Tooltip("Flag if this is a walkable module or not")]
        public bool walkable;

        [Tooltip("The index value of this face's connector")]
        public int connector;

        [HideInInspector]
        public Module[] validNeighbours;
        
        [Tooltip("Is this face symmetric or not?")]
        public bool symmetric;
        
        [Tooltip("Can this face be flipped or not?")]
        public bool flipped;
    }
    
    [Tooltip("Set how likely the algorithm will be to choose this module")]
    public float weight;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
