using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public Camera cam;
    public KeyCode InteractKey = KeyCode.Mouse0;

    public GlobeViewCamera globeViewCamera;

    RaycastHit hit;
    Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(InteractKey)){
            ray = cam.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit, 10000);

            if(hit.transform.GetComponent<Planet>()){
                Debug.Log("Planet Interacted With");
                globeViewCamera.Target = hit.transform;
            }

            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(cam.transform.position, hit.point), Color.green, 5);
        }
    }
}
