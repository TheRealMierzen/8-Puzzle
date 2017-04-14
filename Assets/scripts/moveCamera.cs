using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour {

    Vector3 newPos;
    Quaternion newRot;
    public Canvas p;

    // Use this for initialization
    void Start () {

        newPos = transform.position;
        newRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        move();

        if (Vector3.Distance(transform.position, GameObject.Find("smartphone_n4h").transform.position) < 5.17f)
            p.enabled = true;
        else if (p.enabled == true)
            p.enabled = false;
	}

    void move()
    {
        Vector3 upView = new Vector3(6.5f, 8.462955f, -5.380001f);
        Vector3 normalView = new Vector3(6.499998f, 8.462955f, -13.55032f);

        Quaternion upRot = Quaternion.Euler(90,0,0);
        Quaternion normalRot = Quaternion.Euler(40.592f, 0, 0);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            newPos = upView;
            newRot = upRot;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            newPos = normalView;
            newRot = normalRot;
        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime);
    }

    public void move(Vector3 movesPos)
    {

        Vector3 normalView = new Vector3(6.499998f, 8.462955f, -13.55032f);

        Quaternion upRot = Quaternion.Euler(90, 0, 0);
        Quaternion normalRot = Quaternion.Euler(40.592f, 0, 0);

        if (GameObject.Find("Page (1)").GetComponent<Animator>().GetBool("open") == true) // forward
        {
            newPos = movesPos;
            newRot = upRot;
        }
        else if (GameObject.Find("Page (1)").GetComponent<Animator>().GetBool("open") == false) // backwards
        {
            newPos = normalView;
            newRot = normalRot;
        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime);
    }

    public void move(Vector3 phonePos, Quaternion rot, bool phone)
    {

        Vector3 normalView = new Vector3(6.499998f, 8.462955f, -13.55032f);

        Quaternion normalRot = Quaternion.Euler(40.592f, 0, 0);

        if (phone) // forward
        {
            newPos = phonePos;
            newRot = rot;
        }
        else if (!phone) // backwards
        {
            newPos = normalView;
            newRot = normalRot;
        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime);
    }
}
