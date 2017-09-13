using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CenterOnLEDs : MonoBehaviour
{
    private GameObject camera_pivot;

	// Use this for initialization
	void Start ()
    {
        camera_pivot = GameObject.Find("CameraPivot");
	}

    public Vector3 CenterOfVectors(Vector3[] vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Length == 0)
        {
            return sum;
        }

        foreach (Vector3 vec in vectors)
        {
            sum += vec;
        }
        return sum / vectors.Length;
    }

    List<Vector3> FindAllLEDs()
    {
        var led_locations = new List<Vector3>();

        foreach (GameObject led in GameObject.FindGameObjectsWithTag("LEDPrefab"))
        {
            led_locations.Add(led.transform.position);
        }

        return led_locations;
    }

    // Update is called once per frame
    void Update ()
    {
        var leds = FindAllLEDs();
        camera_pivot.transform.position = CenterOfVectors(leds.ToArray());
    }
}
