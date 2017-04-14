using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phone_camera : MonoBehaviour {

    public Camera cam;
    public bool ph = false;

    private void OnMouseDown()
    {
        if (!ph)
        {
            cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), !ph);
            ph = !ph;
        }
    }
}
