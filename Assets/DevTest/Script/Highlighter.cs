using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Highlighter : MonoBehaviour
{
    private Color startcolor;
    // Start is called before the first frame update
    void Start()
    {
        startcolor = GetComponent<Renderer>().material.color; //save color
    }

    public void Update()
    {
        
    }

    void OnMouseOver()
    {
        //mouse is over object
        GetComponent<Renderer>().material.color = Color.yellow; //change color
    }

    void OnMouseExit()
    {
        //mouse is no longer over object
        GetComponent<Renderer>().material.color = startcolor; //revert color
    }
}
