using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

struct stepOnGrid{
    public Vector2 position;
    public float distance;
    public int range;
}

public class RandomWalker :MonoBehaviour
{
    private List<List<GameObject>> gizmoGrid;
    private List<Vector2> walked = new List<Vector2>();

    public Vector2 start;
    public Vector2 end;
    public List<Vector2> waypoints;

    public void setGizmoGrid(List<List<GameObject>> grid){
        gizmoGrid=grid;
    }
    void Start(){
        
    }
    public void RandomWalk(){
        PaintTargets();
        int gridSize = gizmoGrid.Count;
        Vector2 currentPos = start;
        Vector2 currentTarget=waypoints[0];
        int waypointIndex=0;
        bool walkEnded = false;
        bool gotEnd = false;
        bool gotWaypoints = false;
        //int count =0;
        while(walkEnded == false){
            Vector2 nextStep;
            List<Vector2> possibleSteps = ComputePossibleSteps(currentPos,gridSize);
            if(possibleSteps.Count == 0){
                nextStep = ComputeClosestWaypoint(currentTarget);
            }
            else{
                nextStep = ComputeNextStep(possibleSteps,currentTarget);
            }
            
            walked.Add(nextStep);
            currentPos=nextStep;
            //Debug.Log(currentPos);
            // process next steps
            if(nextStep == currentTarget){
                if(waypointIndex<waypoints.Count){
                    currentTarget = waypoints[waypointIndex];
                    waypointIndex++;
                }
                else{
                    currentTarget=end;
                }
                gotEnd = walked.Contains(end);
                gotWaypoints =true;
                for(int i=0;i<waypoints.Count;i++){
                    gotWaypoints = gotWaypoints && walked.Contains(waypoints[i]);
                }
            }
            // check if whole path contains all points
            if(gotEnd && gotWaypoints){
                walkEnded=true;
            }
            // if(count>100){
            //     walkEnded=true;
            // }
            // count++;
        }
        Debug.Log("walked this many tiles: "+walked.Count);
        StartCoroutine(DoTheWalk());
    }

    private void PaintTargets()
    {
        int slotIndex = -1;
        GameObject slot = new GameObject();
        for(int i =0; i<gizmoGrid.Count;i++){
            slot = gizmoGrid[(int)end.x][(int)end.y];
            slot.GetComponent<Module>().SetColor(Color.yellow);
            // slotIndex = gizmoGrid[i].FindIndex(e=> e.transform.position.x == end.x & e.transform.position.z == end.y);
            // //Debug.Log("Current slot is: "+slotIndex);
            // if(slotIndex >-1){
            //     slot = gizmoGrid[end.x][end.y];
            //     slot.GetComponent<Module>().SetColor(Color.yellow);
            // }
        }
        for(int step=0;step<waypoints.Count;step++){
            //Debug.Log("starting step n "+step);
            slot = gizmoGrid[(int)waypoints[step].x][(int)waypoints[step].y];
            slot.GetComponent<Module>().SetColor(Color.yellow);
        //     slotIndex = -1;
        //     slot = new GameObject();
        //     for(int i =0; i<gizmoGrid.Count;i++){
        //         slotIndex = gizmoGrid[i].FindIndex(e=> e.transform.position.x == waypoints[step].x & e.transform.position.z == waypoints[step].y);
        //         //Debug.Log("Current slot is: "+slotIndex);
        //         if(slotIndex >-1){
        //             slot = gizmoGrid[i][slotIndex];
        //             slot.GetComponent<Module>().SetColor(Color.yellow);
        //         }
        //     }
        }
    }

    private Vector2 ComputeClosestWaypoint(Vector2 target)
    {
        Vector2 closest= new Vector2(-gizmoGrid.Count,-gizmoGrid.Count);
        float minDistance = Vector2.Distance(closest,target);
        for(int i=0;i<walked.Count;i++){
            float distance = Vector2.Distance(walked[i],target);
            if(distance<minDistance){
                minDistance = distance;
                closest=walked[i];
            }
        }
        return closest;
    }

    IEnumerator DoTheWalk(){
        for(int step=0;step<walked.Count;step++){
            Debug.Log("starting step n "+step);
            int slotIndex = -1;
            //GameObject slot = new GameObject();
            Debug.Log("steping on coordinates : "+(int)walked[step].x+","+(int)walked[step].y);
            GameObject slot = gizmoGrid[(int)walked[step].x][(int)walked[step].y];
            Debug.Log("Found this slot: ",slot);
            slot.GetComponent<Module>().SetColor(Color.red);
            // for(int i =0; i<gizmoGrid.Count;i++){
            //     slotIndex = gizmoGrid[i].FindIndex(e=> e.transform.position.x == walked[step].x & e.transform.position.z == walked[step].y);
            //     Debug.Log("Current slot is: "+i+","+slotIndex);
            //     if(slotIndex >-1){
            //         slot = gizmoGrid[i][slotIndex];
            //         slot.GetComponent<Module>().SetColor(Color.red);
            //     }
            // }
            yield return new WaitForSeconds(0.2f);
            slot.GetComponent<Module>().SetColor(Color.green);
        }
        
    }
    private Vector2 ComputeNextStep(List<Vector2> possibleSteps,  Vector2 currentTarget)
    {
        Vector2 nextStep;
        List<float> distances = EvaluateDistanceToTarget(possibleSteps,currentTarget);
        //nextStep = ChooseNaiveStep(possibleSteps);
        nextStep = ChooseStep(distances,possibleSteps);
        return nextStep;
    }

    private  Vector2 ChooseNaiveStep( List<Vector2> possibleSteps)
    {
        float rng = UnityEngine.Random.Range(0,possibleSteps.Count);
        for(int i =0; i<possibleSteps.Count;i++){
            if(rng<i+1){
                return possibleSteps[i];
            }
        }
        return new Vector2(-1,-1);
    }

    private  Vector2 ChooseStep(List<float> distances, List<Vector2> possibleSteps)
    {
        List<stepOnGrid> possibilities = new List<stepOnGrid>();
        float totalRange=0;
        //Debug.Log("size of distances : "+distances.Count);
        for(int i=0;i<distances.Count;i++){
            stepOnGrid step = new stepOnGrid
            {
                position = possibleSteps[i],
                distance = distances[i],
                range = 0
            };
            possibilities.Add(step);
            totalRange = totalRange+i+1;
        }
        possibilities.Sort((x,y)=> x.distance.CompareTo(y.distance));
        //Debug.Log("size of possibilities : "+possibilities.Count);
        // for(int i=0;i<possibilities.Count;i++){
        //     Debug.Log("this is item "+i+" : "+possibilities[i].distance);
        // }
        
        float rng = UnityEngine.Random.Range(0f,totalRange);
        //  Debug.Log("Maximum range is :"+totalRange);
        //  Debug.Log("random generated is: "+rng);
        int range=0;
        for(int i = 0;i<possibilities.Count;i++){
            range = range+i+1;
            //Debug.Log("Tested this range: "+range);
            if(rng<=range){
                //Debug.Log("Chose item "+(possibilities.Count - i-1)+" with distance : "+possibilities[possibilities.Count - i-1].distance);
                return possibilities[possibilities.Count - i-1].position;
            }
        }
        return new Vector2(-1,-1);
    }

    private  List<float> EvaluateDistanceToTarget(List<Vector2> possibleSteps, Vector2 currentTarget)
    {
        List<float> distances = new List<float>();
        for(int i =0; i<possibleSteps.Count;i++){
            distances.Add(Vector2.Distance(currentTarget,possibleSteps[i]));
        }
        return distances;
    }

    private List<Vector2> ComputePossibleSteps(Vector2 currentPos, int gridSize){
        List<Vector2> possible = new List<Vector2>();
        if(currentPos.x+1<gridSize){
            Vector2 step = currentPos+new Vector2(1,0);
            if(!walked.Contains(step)){
                possible.Add(step);
            }
            
        }
        if(currentPos.y+1<gizmoGrid[0].Count){
            Vector2 step = currentPos+new Vector2(0,1);
            if(!walked.Contains(step)){
                possible.Add(step);
            }
        }
        if(currentPos.x-1>0){
            Vector2 step = currentPos-new Vector2(1,0);
            if(!walked.Contains(step)){
                possible.Add(step);
            }
        }
        if(currentPos.y-1>0){
            Vector2 step = currentPos-new Vector2(0,1);
            if(!walked.Contains(step)){
                possible.Add(step);
            }
        }
        return possible;
    }
}
