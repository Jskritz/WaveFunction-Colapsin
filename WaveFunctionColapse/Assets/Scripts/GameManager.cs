using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<List<GameObject>> walkGrid = new List<List<GameObject>>();

    public Collapser collapser;

    private RandomWalker walker;

    public WaveSettings settings;

    //public GameObject slot;
    void Awake(){
        
    }
    // Start is called before the first frame update
    void Start()
    {
        var m_camera = Camera.main;
        // set the camera to the right place
        m_camera.transform.position = new Vector3(
            (float)settings.x / 2,
            10,
            
            (float)settings.z / 2);
        
        RandomWalker walker= GetComponent<RandomWalker>();
        //GenerateGrid(50);
        Debug.Log("building empty wave");
        collapser.BuildEmptyWave();
        Debug.Log("random walking");
        walker.RandomWalk();
        walker.VisualizeTheWalk();
        Debug.Log("apply  walking");
        walker.DoTheWalk();
        Debug.Log("collapsing");
        collapser.collapsevisually();
        //collapser.Collapse();
        //StartCoroutine(StartWalk());
    }

    IEnumerator StartWalk(){
        yield return new WaitForSeconds(1.0f);
        walker = GetComponent<RandomWalker>();
        walkGrid = collapser.GetModules();
        Debug.Log("Size of grid :"+walkGrid.Count);
        walker.setGizmoGrid(walkGrid);
        Debug.Log("Starting Walk");
        walker.RandomWalk();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    // void GenerateGrid(int gridSize){
    //     for(int i =0 ; i<gridSize;i++){
    //         List<GameObject> line = new List<GameObject>();
    //         for(int j =0 ; j<gridSize;j++){
    //             GameObject cube = Instantiate(slot, new Vector3(i,0,j), Quaternion.identity);
    //             line.Add(cube);
    //         }
    //         walkGrid.Add(line);
    //     }
    // }   
}
