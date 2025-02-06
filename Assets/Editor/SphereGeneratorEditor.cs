using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SphereGenerator))]
public class SphereGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        SphereGenerator sphereGenerator = (SphereGenerator)target;
        
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("4")){
            sphereGenerator.resolution = 4;
            sphereGenerator.UpdateSides();
        }        
        if(GUILayout.Button("16")){
            sphereGenerator.resolution = 16;
            sphereGenerator.UpdateSides();
        }        
        if(GUILayout.Button("64")){
            sphereGenerator.resolution = 64;
            sphereGenerator.UpdateSides();
        }                
        if(GUILayout.Button("128")){
            sphereGenerator.resolution = 128;
            sphereGenerator.UpdateSides();
        }                
        if(GUILayout.Button("254")){
            sphereGenerator.resolution = 254;
            sphereGenerator.UpdateSides();
        }        
        
        GUILayout.EndHorizontal();
        
        
        base.OnInspectorGUI();

        
        GUILayout.Space(30);
        if(GUILayout.Button("Optimize Mesh")){
            sphereGenerator.OptimizeMeshes();
        }       
        GUILayout.Space(20);
        if(GUILayout.Button("Create Sides")){
            sphereGenerator.CreateSides();
        }        
        if(GUILayout.Button("Delete Sides")){
            sphereGenerator.DeleteSides();
        }

    }
}