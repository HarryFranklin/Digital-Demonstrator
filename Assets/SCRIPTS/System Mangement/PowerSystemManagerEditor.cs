#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerSystemManager))]
public class PowerSystemManagerEditor : Editor
{
    private bool showAdvancedSettings = true;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PowerSystemManager manager = (PowerSystemManager)target;
        
        // ---------- CONTROL PANEL ----------
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
        
        // ---------- TURBINE CONTROL ----------
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Turbine Control", EditorStyles.boldLabel);
        
        float speedValue = EditorGUILayout.Slider("Wind Speed", 10f, 0f, 20f);
        
        if (GUILayout.Button("Set All Turbines to Speed"))
        {
            manager.SetAllTurbineWindSpeeds(speedValue);
        }
        
        // ---------- SMART POWER MANAGEMENT ----------
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Smart Power Management", EditorStyles.boldLabel);
        
        // Toggle for demand matching
        bool currentState = manager.enableDemandMatching;
        bool newState = EditorGUILayout.Toggle("Enable Demand Matching", currentState);
        
        if (currentState != newState)
        {
            manager.SetDemandMatchingEnabled(newState);
        }
        
        // Advanced settings for demand matching
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
        
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;
            
            // Battery target settings
            manager.targetBatteryChargePercentage = EditorGUILayout.Slider(
                "Target Battery %", manager.targetBatteryChargePercentage, 10f, 100f);
                
            manager.batteryChargeBuffer = EditorGUILayout.Slider(
                "Battery Buffer %", manager.batteryChargeBuffer, 0f, 20f);
                
            manager.minTurbineSpeed = EditorGUILayout.Slider(
                "Min Turbine Speed", manager.minTurbineSpeed, 0f, 5f);
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Match Power to Current Demand"))
        {
            manager.MatchPowerToLoad();
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Handle Battery Full Scenario"))
        {
            manager.HandleBatteryFullScenario();
        }
    }
}
#endif