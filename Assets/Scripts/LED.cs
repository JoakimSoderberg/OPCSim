using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : MonoBehaviour
{

    public void SetColor(Color c)
    {
        var material = this.gameObject.GetComponent<Renderer>().material;
        //Debug.Log(c);
        material.SetColor("_EmissionColor", c);
        material.SetColor("_Color", c);
        material.SetColor("_SpecColor", c);
        material.SetColor("_Albedo", c);
        material.SetColor("_MainColor", c);
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
