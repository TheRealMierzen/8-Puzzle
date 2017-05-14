using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class cube_behaviour : MonoBehaviour {

    public GameObject control;
    public string axis;

    private bool dragging = false;
    private float distance;
    public bool moving = false;
    public int manh;

    public GameObject endMarker;
    public UnityEngine.UI.Button solveB;

    Vector3 pos;
    public Vector3 emptyPos;

    public GameObject[] locs = new GameObject[9];

    void Start()
    {
        pos = transform.position;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
        endMarker = getNearest();
        moving = true;

        if(endMarker.transform.position != pos)
            control.GetComponent<controller>().updatePos(gameObject, endMarker);

        if (control.GetComponent<ai>().solution.Count != 0)
        {
            if (gameObject.name.Substring(4) == (control.GetComponent<ai>().solution[0] - 1).ToString())
            {
                control.GetComponent<ai>().solution.RemoveAt(0);
                solveB.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + control.GetComponent<ai>().solution.Count.ToString() + ")";
            }
            else
            {
                control.GetComponent<ai>().solution.Insert(0, Convert.ToInt16(gameObject.name.Substring(4)) + 1);
                solveB.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + control.GetComponent<ai>().solution.Count.ToString() + ")";
            }
        }
    }

    void OnMouseOver()
    {
        
        if (Input.GetMouseButtonUp(1) && GetComponent<cube_behaviour>().enabled == true)
        {
            
            endMarker = control.GetComponent<controller>().emptyPos;
            endMarker = GameObject.Find(endMarker.gameObject.name);

            moving = true;
            move();
            move();
            move();
            control.GetComponent<controller>().updatePos(gameObject, endMarker);

            if (control.GetComponent<ai>().solution.Count != 0)
            {
                if (gameObject.name.Substring(4) == (control.GetComponent<ai>().solution[0] - 1).ToString())
                {
                    control.GetComponent<ai>().solution.RemoveAt(0);
                    solveB.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + control.GetComponent<ai>().solution.Count.ToString() + ")";
                }
                else
                {
                    control.GetComponent<ai>().solution.Insert(0, Convert.ToInt16(gameObject.name.Substring(4)) + 1);
                    solveB.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + control.GetComponent<ai>().solution.Count.ToString() + ")";
                }
            }

        }
    }

    void Update()
    {
        if (dragging)
        {
            //right = red right/left x
            //forward = blue up/down z
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            rayPoint.y = 0.525f;

            if (axis == "up")
            {
                transform.position = new Vector3(transform.position.x, 0.525f, rayPoint.z);

                if (transform.position.z < pos.z)
                    transform.position = new Vector3(transform.position.x, transform.position.y, pos.z);

                if(transform.position.z > emptyPos.z)
                    transform.position = new Vector3(transform.position.x, transform.position.y, emptyPos.z);
            }
            if (axis == "down")
            {
                transform.position = new Vector3(transform.position.x, 0.525f, rayPoint.z);

                if (transform.position.z > pos.z)
                    transform.position = new Vector3(transform.position.x,transform.position.y, pos.z);

                if (transform.position.z < emptyPos.z)
                    transform.position = new Vector3(transform.position.x, transform.position.y, emptyPos.z);
            }

            if (axis == "right")
            {
                transform.position = new Vector3(rayPoint.x, 0.525f, transform.position.z);

                if (transform.position.x < pos.x)
                    transform.position = new Vector3(pos.x, transform.position.y, transform.position.z);

                if (transform.position.x > emptyPos.x)
                    transform.position = new Vector3(emptyPos.x, transform.position.y, transform.position.z);
            }

            if (axis == "left")
            {
                transform.position = new Vector3(rayPoint.x, 0.525f, transform.position.z);

                if (transform.position.x > pos.x)
                    transform.position = new Vector3(pos.x, transform.position.y, transform.position.z);

                if (transform.position.x < emptyPos.x)
                    transform.position = new Vector3(emptyPos.x, transform.position.y, transform.position.z);

            }

        }

        if (moving)
            move();
    }

    public void aiClick()
    {

        endMarker = control.GetComponent<controller>().emptyPos;
        endMarker = GameObject.Find(endMarker.gameObject.name);

        moving = true;
        move();
        move();
        move();
        move();
        move();
        move();
        move();
        move();
        move();
        move();
        control.GetComponent<controller>().updatePos(gameObject, endMarker, true);

    }

    GameObject getNearest()
    {

        GameObject closest = locs[0];
        float dist = 99;

        foreach(GameObject pos in locs)
        {
            float distance = Vector3.Distance(gameObject.transform.position, pos.transform.position);

            if (distance < dist)
            {

                closest = pos;
                dist = distance;

            }

        }

        return closest;

    }

    public void move()
    {

        if (moving == true)   
            transform.position = Vector3.Lerp(transform.position, endMarker.transform.position, 0.25f);

        float closeEnough = 0.1f;
        if (Vector3.Distance(transform.position, endMarker.transform.position) < closeEnough)
        {
            moving = false;
            transform.position = endMarker.transform.position;
            pos = transform.position;
            
        }

        if (transform.position == endMarker.transform.position)
        {
            moving = false;
            pos = transform.position;
        }
    }

    public void setAxis(string ax)
    {

        axis = ax;

    }

    public void setEmpty(Vector3 pos)
    {

        emptyPos = pos;

    }

}
