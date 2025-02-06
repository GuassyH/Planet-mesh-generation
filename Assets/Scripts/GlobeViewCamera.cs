using UnityEditor.Rendering;
using UnityEngine;

public class GlobeViewCamera : MonoBehaviour
{

    public Transform Target; 
    [Header("Rotate")]
    
    public Camera globeCam;
    public KeyCode LookKey = KeyCode.Mouse1;
    public float RotationSpeed = 2;
    public float RotateSmoothDelta = 8;
    
    [Header("Zoom")]
    public float ZoomSmoothDelta = 8;
    public float[] ZoomValues = new float[]{ 2, 4, 8, 12, 18};
    float objectRadius;
    int ZoomIndex;


    float horizontal;
    float vertical;

    Rigidbody rb;
    Vector3 RotateAddition;
    Vector3 AddedRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        ZoomIndex = ZoomValues.Length-1;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Target.GetComponent<Planet>()){
            objectRadius = Target.GetComponent<Planet>().radius;
        }
        else{
            objectRadius = Target.transform.lossyScale.x / 2;
        }
        this.transform.position = Target.position;
    
    
        
        RotateCamera();
        ZoomCamera();
    }


    void ZoomCamera(){
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0){
            if(ZoomIndex > 0){
                ZoomIndex--;
            }
        }  
        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0){
            if(ZoomIndex <= ZoomValues.Length-2){
                ZoomIndex++;
            }
        }  
    
        globeCam.transform.localPosition = Vector3.LerpUnclamped(globeCam.transform.localPosition, new Vector3(0, 0, objectRadius * -ZoomValues[ZoomIndex]), ZoomSmoothDelta * Time.deltaTime);
    
    }


    void RotateCamera(){

        if(Input.GetKey(LookKey)){
            horizontal = Input.GetAxisRaw("Mouse X");
            vertical = Input.GetAxisRaw("Mouse Y");
            RotateAddition = new Vector3(-vertical * RotationSpeed, horizontal * RotationSpeed, 0);
        }
        else{   RotateAddition = Vector3.zero;  }

        AddedRot = Vector3.LerpUnclamped(AddedRot, RotateAddition, RotateSmoothDelta * Time.deltaTime);
        
        this.transform.Rotate(AddedRot);
        this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0);

    }
 
}
