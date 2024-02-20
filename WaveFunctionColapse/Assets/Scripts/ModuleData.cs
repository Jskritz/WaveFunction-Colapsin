using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(menuName = "Wave Function Collapse/Module Data", fileName = "moduleData.asset")]
public class ModuleData : ScriptableObject
{
    
    [System.Serializable]
    private struct ModuleDataList
    {
        public ModuleDataStruct[] modules;
    }
    
    
    public List<Prototype> modules;

    public TextAsset moduleDataFile;

    private void ReadJson()
    {
        var moduleDataList = JsonUtility.FromJson<ModuleDataList>(moduleDataFile.text).modules;
        foreach (var moduleData in moduleDataList)
        {
            modules.Add(new Prototype(moduleData));
        }

    }
    

    [CustomEditor(typeof(ModuleData))]
    public class ModuleDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var moduleData = (ModuleData)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Load data from JSON"))
            {
                moduleData.ReadJson();
            }
        }
    }
}
