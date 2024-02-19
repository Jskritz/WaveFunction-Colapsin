using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<List<GameObject>> walkGrid = new List<List<GameObject>>();

    private RandomWalker walker;

    public GameObject slot;
    void Awake(){
        
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid(50);
        walker = GetComponent<RandomWalker>();
        walker.setGizmoGrid(walkGrid);
        Debug.Log("Starting Walk");
        walker.RandomWalk();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrid(int gridSize){
        for(int i =0 ; i<gridSize;i++){
            List<GameObject> line = new List<GameObject>();
            for(int j =0 ; j<gridSize;j++){
                GameObject cube = Instantiate(slot, new Vector3(i,0,j), Quaternion.identity);
                line.Add(cube);
            }
            walkGrid.Add(line);
        }
    }   
}
