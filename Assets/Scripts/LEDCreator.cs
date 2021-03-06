﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LEDLocation
{
    public float[] point;
}

/// <summary>
/// Arrays in the root of a JSON file is not allowed so we need this.
/// </summary>
public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}


public class LEDCreator : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefab;
    private GameObject camera_pivot;

    public static List<LED> all_leds;
    public GameObject led_location;

    // Use this for initialization
    void Start ()
    {
        camera_pivot = GameObject.Find("CameraPivot");
        led_location = GameObject.Find("LedLocation");
        LoadLEDLayout();
    }

    void LoadLEDLayout()
    {
        var asset = Resources.Load<TextAsset>("layout");
        var led_locations = JsonHelper.getJsonArray<LEDLocation>(asset.text);
        float scale = 10.0f;

        foreach (LEDLocation loc in led_locations)
        {
            var location = new Vector3(loc.point[0] * scale, loc.point[1] * scale, loc.point[2] * scale);
            var led = InstantiateLED(location);
            led.transform.SetParent(this.led_location.transform);
            led.SetColor(Color.black);
        }

        // TODO: This is a very hacky way of placing the LEDs :D
        led_location.transform.Rotate(0, 0, -180);
        led_location.transform.localScale = new Vector3(16, 11, 10);
        led_location.transform.position = new Vector3(4.4f, 9.3f, 10f);

        LEDCreator.all_leds = FindAllLEDs();
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

    List<LED> FindAllLEDs()
    {
        var leds = new List<LED>();
        var led_locations = new List<Vector3>();

        foreach (GameObject led_obj in GameObject.FindGameObjectsWithTag("LEDPrefab"))
        {
            led_locations.Add(led_obj.transform.position);

            LED led = led_obj.GetComponent<LED>();
            leds.Add(led);
        }

        camera_pivot.transform.position = CenterOfVectors(led_locations.ToArray());

        return leds;
    }

    LED InstantiateLED(Vector3 point)
    {
        GameObject obj = Instantiate(prefab, point, Quaternion.identity) as GameObject;
        LED led = (LED)obj.GetComponent(typeof(LED));
        led.SetColor(Color.black);
        return led;
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*
        if (Input.GetKey(KeyCode.Mouse1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var led = InstantiateLED(hit.point);
                led.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
        }
        */
    }
}
