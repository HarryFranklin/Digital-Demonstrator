#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerSystemManager))]
public class PowerSystemManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PowerSystemManager manager = (PowerSystemManager)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Control Panel", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Emergency Shutdown"))
        {
            manager.emergencyShutdown = true;
        }
        
        if (GUILayout.Button("Reset Turbines to Automatic"))
        {
            manager.ResetTurbinesToAutomatic();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Turbine Control", EditorStyles.boldLabel);
        
        float speedValue = EditorGUILayout.Slider("Wind Speed", 10f, 0f, 20f);
        
        if (GUILayout.Button("Set All Turbines to Speed"))
        {
            manager.SetAllTurbineWindSpeeds(speedValue);
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Handle Battery Full Scenario"))
        {
            manager.HandleBatteryFullScenario();
        }
    }
}
#endif