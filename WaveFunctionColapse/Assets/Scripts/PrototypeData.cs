using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "Wave Function Collapse/Prototype Data", fileName = "prototypeData.asset")]
public class PrototypeData : ScriptableObject
{
    
    [System.Serializable]
    private struct PrototypeDataList
    {
        public PrototypeDataStruct[] prototypes;
    }
    
    
    [SerializeField]
    public List<Prototype> Prototypes = new List<Prototype>();

    public TextAsset prototypeDataFile;

    public void ReadJson()
    {
        Prototypes = new List<Prototype>();
        //var prototypeDataList = JsonUtility.FromJson<PrototypeDataList>(prototypeDataFile.text).prototypes;
        var prototypeDataList = JsonConvert.DeserializeObject<PrototypeDataList>(prototypeDataFile.text).prototypes;
        foreach (var prototypeData in prototypeDataList)
        {
            Prototypes.Add(new Prototype(prototypeData));
        }
        Debug.Log("Added prototypes");
    }
    

    [CustomEditor(typeof(PrototypeData))]
    public class ModuleDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var moduleData = (PrototypeData)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Load data from JSON"))
            {
                moduleData.ReadJson();
            }
        }
    }
}
