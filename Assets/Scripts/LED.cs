using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : MonoBehaviour
{

    public void SetColor(Color c)
    {
        this.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
    }

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }
}
