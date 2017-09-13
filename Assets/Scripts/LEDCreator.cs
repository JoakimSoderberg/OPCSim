using System.Collections;
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

    // Use this for initialization
    void Start ()
    {
        LoadLEDLayout();
    }

    void LoadLEDLayout()
    {
        var asset = Resources.Load<TextAsset>("layout");
        var led_locations = JsonHelper.getJsonArray<LEDLocation>(asset.text);
        var scale = new Vector3(100f, 100f, 100f);

        foreach (LEDLocation loc in led_locations)
        {
            var location = new Vector3(loc.point[0] * -10, loc.point[1] * -10, loc.point[2] * -10);
            //location.Scale(scale);
            var led = InstantiateLED(location);
            led.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
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
        if (Input.GetKey(KeyCode.Mouse1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var led = InstantiateLED(hit.point);
                led.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
        }
    }
}
