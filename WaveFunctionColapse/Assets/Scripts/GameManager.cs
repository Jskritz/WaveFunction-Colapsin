using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<List<GameObject>> walkGrid = new List<List<GameObject>>();

    public Collapser collapser;

    private RandomWalker walker;

    public Character player;
    public GameObject Coin;

    //public GameObject slot;
    void Awake(){
        
    }
    // Start is called before the first frame update
    void Start()
    {
        walker= GetComponent<RandomWalker>();
        Debug.Log("walker is "+walker==null?true:false);
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
        Debug.Log("positioning player and coins");
        InitialScene();
    }

    private void InitialScene()
    {
        Debug.Log("walker is "+(walker??null?true:false));
        List<List<GameObject>> modules=collapser._modules;
        GameObject playerPos = modules[(int)walker.start.x][(int)walker.start.y];
        player.SetPosition(playerPos.transform.position+Vector3.up*10);
        List<Vector2> waypoints = walker.waypoints;
        for(int i=0;i<waypoints.Count;i++){
            GameObject coinPos = modules[(int)waypoints[i].x][(int)waypoints[i].y];
            Instantiate(Coin,coinPos.transform.position+Vector3.up*10,Quaternion.identity);
        }
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
