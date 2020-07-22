using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelector : MonoBehaviour
{
    public GameObject ButtonPrefab; //generate more buttons
    public GameObject Car; //model to be looked at
    public GameObject ScrollContent; //scrollview holding buttons
    public Transform[] carParts; //array of car parts
    public Camera ViewCamera; //viewing camera
    public GameObject focus; //object focused on
    public float zoomspeed = 5.0f; //zooming sensitivity, can be adjusted
    public float rotatespeed = 2.0f;
    public Text titleText; //overview text
    public LineRenderer label; //line pointing from text label to car part
    public Vector3 meshLocation; //center of part model
    public int maxZoomIn = 2; //min camera distance from part
    public int maxZoomOut = 7; //max camera distance from part
    public TextMesh floatingText; //text pointing to car part

    void Start()
    {
        //get all car parts and put them in array
        carParts = Car.GetComponentsInChildren<Transform>();
        focus = carParts[0].gameObject;

        //set UI elements initially
        titleText.text = carParts[0].gameObject.name + " Overview";
        floatingText.text = carParts[0].gameObject.name;
        floatingText.gameObject.SetActive(false);

        for (int i = 1; i < carParts.Length; i++)
        {
            //add mesh collider and highlighter to all car parts
            carParts[i].gameObject.AddComponent<MeshCollider>();
            carParts[i].gameObject.AddComponent<Highlighter>();

            //instantiate buttons for each car part, put in scrollview, and assign listener
            GameObject newButton = Instantiate(ButtonPrefab) as GameObject;
            newButton.transform.SetParent(ScrollContent.transform, false);
            newButton.GetComponentInChildren<Text>().text = carParts[i].gameObject.name;
            newButton.SetActive(true);
            Transform temp = carParts[i];
            newButton.GetComponent<Button>().onClick.AddListener(() => zoomIn(temp));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check to see which part is being hovered over
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit)
        {
            //check if left mouse clicked
            if (Input.GetMouseButton(0))
            {
                //make sure ground is not selected
                if (!hitInfo.transform.gameObject.name.Equals("GroundPlane"))
                {
                    //change view to selected part
                    zoomIn(hitInfo.transform.gameObject.transform);
                }
            }
        }

        //draw floating text and line from text to part
        floatingText.transform.rotation = Quaternion.LookRotation(floatingText.transform.position - Camera.main.transform.position);
        label.SetPosition(0, floatingText.GetComponent<MeshRenderer>().bounds.center);
        label.SetPosition(1, meshLocation);

        //keep camera facing towards part
        transform.LookAt(meshLocation);

        //rotate camera if right mouse is held and dragged
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(meshLocation, transform.up, -Input.GetAxis("Mouse X") * rotatespeed);

            transform.RotateAround(meshLocation, transform.right, -Input.GetAxis("Mouse Y") * rotatespeed);
        }

        //calculate distance from camera to focused object
        float dist = Vector3.Distance(transform.position, focus.transform.position);

        //auto zoom if switched object and too close/far
        if (dist > maxZoomOut)
        {
            transform.position += transform.forward * zoomspeed * Time.deltaTime;
        }
        if (dist < maxZoomIn)
        {
            transform.position -= transform.forward * zoomspeed * Time.deltaTime;
        }

        //zoom in and zoom out with scrollwheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //checks to prevent camera bouncing when at zoom limits
            if (Vector3.Distance(transform.position + transform.forward * zoomspeed * Input.GetAxis("Mouse ScrollWheel"), focus.transform.position) > maxZoomIn)
            {
                if (Vector3.Distance(transform.position + transform.forward * zoomspeed * Input.GetAxis("Mouse ScrollWheel"), focus.transform.position) < maxZoomOut)
                {
                    transform.position += transform.forward * zoomspeed * Input.GetAxis("Mouse ScrollWheel");
                }
            }
        }
    }

    void zoomIn(Transform part) //selects part as new focus
    {
        floatingText.gameObject.SetActive(true);
        focus = part.gameObject; //set selected part to be focused
        //get center of mesh from object
        meshLocation = new Vector3(part.gameObject.GetComponent<MeshRenderer>().bounds.center.x, part.gameObject.GetComponent<MeshRenderer>().bounds.center.y, part.gameObject.GetComponent<MeshRenderer>().bounds.center.z);
        floatingText.transform.position = Vector3.Scale(meshLocation, new Vector3(1.5f, 1.5f, 1.5f)); //move floating text

        floatingText.text = part.gameObject.name; //set floating text name
        if (Vector3.Distance(meshLocation, new Vector3(0f, 0f, 0f)) == 0) //check if object is centered on origin
        {
            //set camera to have bird's eye view if object at origin (edge case)
            transform.position = new Vector3(0f, (float)maxZoomOut, 0f);
            floatingText.transform.position = new Vector3(0f, (float)maxZoomOut/2, 0f);
        }
        //set camera position
        transform.position = meshLocation + meshLocation * (2 / Vector3.Distance(meshLocation, meshLocation * 2));
    }
}
