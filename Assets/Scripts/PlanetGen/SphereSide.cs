using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class SphereSide : MonoBehaviour
{
    [Min(1)] public int resolution;
    [HideInInspector] public float size;
    [HideInInspector] public Vector3 meshRotation;
    [HideInInspector] public bool tileUV;
    [HideInInspector] public bool dynamicResolution;

    MeshFilter meshFilter;

    [HideInInspector] public float a;
    [HideInInspector] public float b;

    // Crater
    [HideInInspector] public float rimWidth;
    [HideInInspector] public float rimSteepness;
    [HideInInspector] public float floorHeight;
    [HideInInspector] public float craterSmoothing;
    [HideInInspector] public int numCraters;

    // stride sizes
    int vec3_size = sizeof(float) * 3;
    int vec2_size = sizeof(float) * 2;
    int float_size = sizeof(float) * 1;

    // Compute Shaders
    public ComputeShader computeSphere;
    public ComputeShader computeHeight;

    Camera cam;  
    bool visible;  


    // Structs and their arrays
    struct vert{    public Vector3 position;    public Vector3 normal;  public Vector2 uv;  };
    private vert[] verts;


    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        
        if(dynamicResolution){
            if(Vector3.Distance(cam.transform.position, this.transform.position) < 100 + size){ if(resolution != 254){resolution = 254; RecalculateMesh(); } return; }
            if(Vector3.Distance(cam.transform.position, this.transform.position) < 500 + size){ if(resolution != 128){resolution = 128; RecalculateMesh(); } return; }
            if(Vector3.Distance(cam.transform.position, this.transform.position) < 1000 + size){ if(resolution != 32){resolution = 32; RecalculateMesh(); } return; }
            if(Vector3.Distance(cam.transform.position, this.transform.position) < 2000 + size){ if(resolution != 8){resolution = 8; RecalculateMesh(); } return; } else{ if(resolution != 4){resolution = 4; RecalculateMesh(); } }
        }

    }

    public void CreateMesh(){

        meshFilter = this.GetComponent<MeshFilter>();

        // Create a new mesh
        var newMesh = new Mesh{
            name = this.transform.name + " side mesh"
        };
        
        meshFilter.mesh = newMesh;
        
        VertexCalculation(newMesh);
        IndexCalculation(newMesh);
    }

    private void OnValidate() {
        RecalculateMesh();
    }

    public void RecalculateMesh(){
        meshFilter = this.GetComponent<MeshFilter>();
        VertexCalculation(meshFilter.sharedMesh);
        IndexCalculation(meshFilter.sharedMesh);
    }







    private void VertexCalculation(Mesh mesh){

        int vertex_count = (resolution + 1) * (resolution + 1);

        // Arrays
        verts = new vert[vertex_count];       
        Vector3[] vertexPos = new Vector3[vertex_count];
        Vector3[] normals = new Vector3[vertex_count];
        Vector2[] uv = new Vector2[vertex_count];
        Vector4[] tangents = new Vector4[vertex_count];
        mesh.triangles = new int[]{};



        // Create the verts Array and give it a size of the amount of verts that will be created
        
        // Create Compute Buffers with lengths of vertcount and stride length of size of all variables combined
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertex_count, vec3_size + vec3_size + vec2_size);
        ComputeBuffer calculatedVertexBuffer = new ComputeBuffer(vertex_count, vec3_size + vec3_size + vec2_size);


        // =========== // Compute Shader Magic! //

        CalculateVertexPos(vertexBuffer);
        CalculateHeight(vertexBuffer, calculatedVertexBuffer);

        // =========== // Compute Shader Magic! //


        // For the amount of vertices add all the attributes to the attributed lists
        for (int i = 0; i < vertex_count; i++){
            vertexPos[i] = verts[i].position;
            normals[i] = verts[i].normal;
            uv[i] = verts[i].uv;
            tangents[i] = new Vector4(1, 0, 0,-1f);
        }

        // Dispose of Buffers (frees memory i think)
        vertexBuffer.Dispose();
        calculatedVertexBuffer.Dispose();

        // Assign Mesh Data
        mesh.vertices = vertexPos;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.tangents = tangents;

    }






    private void CalculateVertexPos(ComputeBuffer vertexBuffer){
        
        // Get all > uncalculated < vertices
        for (int y = 0; y < 1 + resolution; y++){
            for (int x = 0; x < 1 + resolution; x++){
                vert newVert = new vert();
                newVert.position = new Vector3(x, y, size);
                verts[((resolution+1)*y) + x] = newVert;
            }
        }

        // Get rotation of vertices translated into matrix math
        Quaternion quat = Quaternion.Euler(meshRotation);
        Vector4 quatVec = new Vector4(quat.x, quat.y, quat.z, quat.w);

        // Set Variables
        // verts = vertices;
        vertexBuffer.SetData(verts);
        computeSphere.SetBuffer(0, "_vertices", vertexBuffer);
        computeSphere.SetInt("_NumVertices", verts.Length);
        computeSphere.SetInt("resolution", resolution);
        computeSphere.SetFloat("size", size);
        computeSphere.SetVector("rotation", quatVec);
        computeSphere.SetBool("tileUV", tileUV);

        // Run the compute Shader
        computeSphere.Dispatch(0, verts.Length / 16, 1, 1);


    }






    private void CalculateHeight(ComputeBuffer vertexBuffer, ComputeBuffer calculatedVertexBuffer){
        
        
        
        vertexBuffer.GetData(verts);
        
        vertexBuffer = new ComputeBuffer(verts.Length, vec3_size + vec3_size + vec2_size);
        ComputeBuffer craterBuffer = new ComputeBuffer(verts.Length, vec3_size + float_size);


        vertexBuffer.SetData(verts);

        // Assign Compute Buffers
        computeHeight.SetBuffer(0, "_vertices", vertexBuffer);
        computeHeight.SetBuffer(0, "_calculatedVertices", calculatedVertexBuffer);

        // Set Variables
        computeHeight.SetInt("_NumVertices", verts.Length);
        computeHeight.SetFloat("a", a);
        computeHeight.SetFloat("b", b);
        computeHeight.SetInt("numCraters", numCraters);
        computeHeight.SetFloat("rimSteepness", rimSteepness);
        computeHeight.SetFloat("rimWidth", rimWidth);
        computeHeight.SetFloat("floorHeight", floorHeight);
        computeHeight.SetFloat("craterSmoothing", craterSmoothing);
        
        // Run the compute Shader
        computeHeight.Dispatch(0, verts.Length / 16, 1, 1);

        // Get the vertex data and store it in Array
        calculatedVertexBuffer.GetData(verts);

    }











    private void IndexCalculation(Mesh mesh){

        List<int> Indices = new List<int>();
        Indices.Clear();
        // For every vertex add tri's 
        for (int i = 0; i < (resolution * resolution) + resolution; i++)
        {
            if((i + 1) % (resolution + 1) != 0){
                // Bottom tri
                Indices.Add(i);
                Indices.Add(i+1);
                Indices.Add(i+resolution+1);
            }
            if(i % (resolution + 1) != 0){
                // Top tri
                Indices.Add(i);
                Indices.Add(i+resolution + 1);
                Indices.Add(i+resolution);
            }
        }
        mesh.triangles = Indices.ToArray();
    }

}


