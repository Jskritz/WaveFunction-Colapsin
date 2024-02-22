using UnityEditor;
using UnityEngine;


public class WaveSettings : MonoBehaviour
{
    
    [Header("Wave scale")]
    [SerializeField] [Min(1)] public int x = 5;

    [SerializeField] [Min(1)] public int z = 5;

    
    [Header("Module settings")]
    [SerializeField]
    [Tooltip("The scale of an individual module")]
    [Min(0.5f)]
    public float scale = 1;

    [Header("Display settings")]
    [Tooltip("How fast the wave will collapse, for visualisation purposes")]
    [Range(0.1f,1f)]
    public float speed = 0.5f;
    
    private int _numberOfModules;
    
    // Visualising in the editor
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // set the centre in the correct place
        var centre = transform.position + new Vector3((float)x / 2 , scale / 2, (float)z / 2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centre, new Vector3(x, scale, z));
    }

    private void OnDrawGizmosSelected()
    {
        // todo draw the individual cells
        // calculate the number of modules this shape of cell could fit
        _numberOfModules = scale > 0 ? (int)(x * z / Mathf.Pow(scale, 2)) : 0;
        
        // draw lines to mark the grid
        // lengthways
        var numberOfLines = (int)(x / scale);
        var position = transform.position;
        Gizmos.color = Color.cyan;
        for (int i = 1; i < numberOfLines; i++)
        {
            Gizmos.DrawLine(new Vector3(
                position.x + i * scale,
                position.y,
                position.z), new Vector3(
                position.x + i * scale,
                position.y,
                z));
        }
        
        // widthways
        numberOfLines = (int)(z / scale);
        for (int i = 1; i < numberOfLines; i++)
        {
            Gizmos.DrawLine(new Vector3(
                    position.x,
                    position.y,
                    position.z + i * scale), new Vector3(
                    x,
                    position.y,
                    position.z + i * scale));
        }
    }
    
    [CustomEditor(typeof(WaveSettings))]
    public class WaveSettingsEditor : Editor
    {

        public void OnSceneGUI()
        {
            var linkedObject = (WaveSettings)target;

            var position = linkedObject.transform.position;
            var x = linkedObject.x;
            var y = position.y;
            var z = linkedObject.z;
            var handlePosition = new Vector3(x, y, z);

            var newHandlePosition = Handles.PositionHandle(handlePosition, Quaternion.identity);
            if (handlePosition != newHandlePosition)
            {
             
                linkedObject.x = (int)newHandlePosition.x;
                linkedObject.z = (int)newHandlePosition.z;   
            }

            
        }
    }
#endif
    
}
