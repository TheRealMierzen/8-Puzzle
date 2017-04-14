using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moves : MonoBehaviour {

    public List<string> moveList = new List<string>();
    public GameObject con;
    public TextMesh txt;
    public GameObject mv;
    public GameObject no;
    public GameObject cnt;
    int tab = 1;

    public void addMove(string m)
    {

        moveList.Add(m);
        if (tab <= 10)
            txt.text += m + "\t";
        else
        {

            txt.text += "\n" + m + "\t";
            tab = 1;
        }

        tab++;
        cnt.GetComponent<TextMesh>().text = "Move count: " + moveList.Count.ToString(); 
    }

    private void OnMouseDown()
    {
        GetComponentInChildren<Animator>().SetBool("open", !GetComponentInChildren<Animator>().GetBool("open"));
        con.GetComponent<moveCamera>().move(new Vector3(12.59f, 5.184999f, -5.380002f));
        mv.GetComponent<MeshRenderer>().enabled = !mv.GetComponent<MeshRenderer>().enabled;
        no.GetComponent<MeshRenderer>().enabled = !no.GetComponent<MeshRenderer>().enabled;
        cnt.GetComponent<MeshRenderer>().enabled = !cnt.GetComponent<MeshRenderer>().enabled;
    }

    public void fill(List<string> s)
    {
        tab = 1;
        txt.text = "";
        moveList = s;

        for(int i = 0; i < moveList.Count; i++)
        {
            if (tab <= 10)
                txt.text += moveList[i] + "\t";
            else
            {
                txt.text += "\n" + moveList[i] + "\t";
                tab = 1;
            }
            tab++;
        }

        

    }
}
