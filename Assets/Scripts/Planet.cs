using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public float MinutesPerDay = 1;
    public bool RetrogradeTurnDir = false;
    [Space]
    public float gravity;



    [HideInInspector] public float radius = 1;
    
    Transform cam;
    SphereGenerator sphereGenerator;
    // Start is called before the first frame update
    void Start()
    {
        sphereGenerator = this.GetComponent<SphereGenerator>();
        cam = Camera.main.transform;

        if(sphereGenerator){ radius = sphereGenerator.radius; }
        else{ radius = this.transform.lossyScale.x / 2; }
        
        this.GetComponent<SphereCollider>().radius = radius;
        this.GetComponent<SphereCollider>().isTrigger = true;
    }

    void Update()
    {
        if(MinutesPerDay > 0){  this.transform.localEulerAngles += RetrogradeTurnDir ? new Vector3(0, (Time.deltaTime * 6) / MinutesPerDay, 0) : new Vector3(0, (Time.deltaTime * 6) / -MinutesPerDay, 0); }

        
        
    }


}
