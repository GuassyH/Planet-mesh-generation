using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class SphereGenerator : MonoBehaviour
{

    [Header("Sphere mesh")]
    [Range(1, 256)] public int resolution;
    public bool dynamicResolution = true;
    [Min(0.1f)] public float radius = 1;
    [Header("Material")]
    public Material sphereMat;
    public bool tileMaterial = false;
    [Header("Temp")]
    [Range(0, 0.5f)] public float a;
    [Range(-10, 10)] public float b;
    [Header("Compute Shaders")]
    public ComputeShader computeSphere;
    public ComputeShader computeHeight;

    string[] sideNames = { "front", "back", "left", "right", "top", "bottom" };
 
    public void CreateSides(){
    
        DeleteSides();

        for (int i = 0; i < 6; i++)
        {
            // Meshfilters front, back, left, right, up, down
            SphereSide side = new GameObject().AddComponent<SphereSide>();
            side.resolution = resolution;
            side.size = radius;

            side.GetComponent<MeshRenderer>().material = sphereMat;
            side.transform.parent = this.transform;
            side.transform.localPosition = Vector3.zero;

            side.name = sideNames[i];

            Vector3 sideRotation;
            // Rotate based on side information
            switch (i)
            {
                case 0:
                    sideRotation = new Vector3(0, 0, 0);
                    break;
                case 1:
                    sideRotation = new Vector3(0, 180, 0);
                    break;
                case 2:
                    sideRotation = new Vector3(0, 90, 0);
                    break;
                case 3:
                    sideRotation = new Vector3(0, -90, 0);
                    break;
                case 4:
                    sideRotation = new Vector3(-90, 0, 0);
                    break;
                case 5:
                    sideRotation = new Vector3(90, 0, 0);
                    break;
                default:
                    sideRotation = new Vector3(0, 0, 0);
                    break;
            }

            side.meshRotation = sideRotation;
            side.computeSphere = computeSphere;
            side.computeHeight = computeHeight;
            side.dynamicResolution = dynamicResolution;
            side.tileUV = tileMaterial;
            side.CreateMesh();

        }
    
    }



    public void DeleteSides(){
        if(transform.GetComponentsInChildren<SphereSide>().Length > 0){
            foreach(SphereSide child in transform.GetComponentsInChildren<SphereSide>()){
                DestroyImmediate(child.transform.gameObject);
            }
        }
    }

    private void OnValidate() {
        UpdateSides();
    }

    public void UpdateSides(){
        foreach(SphereSide child in transform.GetComponentsInChildren<SphereSide>()){
            child.resolution = resolution;
            child.size = radius;
            child.tileUV = tileMaterial;
            child.dynamicResolution = dynamicResolution;
            child.a = a;
            child.b = b;
            child.GetComponent<MeshRenderer>().material = sphereMat;
            

            child.RecalculateMesh();
        }
    }

    public void OptimizeMeshes(){
        foreach(SphereSide child in transform.GetComponentsInChildren<SphereSide>()){
            child.GetComponent<MeshFilter>().sharedMesh.Optimize();
        }
    }


}
