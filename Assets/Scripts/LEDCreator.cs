using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDCreator : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefab;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = Instantiate(prefab, hit.point, Quaternion.identity) as GameObject;
                LED led = (LED)obj.GetComponent(typeof(LED));
                led.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
        }
    }
}
