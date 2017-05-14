using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Cyberconian.Unity;
using UnityEngine.UI;

public class controller : MonoBehaviour {

    public GameObject[] positions = new GameObject[9];
    public Camera cam;
    public GameObject emptyPos;
    public Canvas canv;
    public GameObject winning;
    public int moves;   
    public GameObject fold;
    public Canvas phoneCanvas;
    public GameObject phone;
    public GameObject clock;
    public Canvas startsc;
    public GameObject input;
    public GameObject[] goal = new GameObject[9];
    public Canvas aiC;

    public Button lMBut;
    public Button lPBut;
    public TextMesh cnt;


    bool custom = false;
    bool ph = false;
    bool win;
    float timer = 0;

    // Use this for initialization
    void Start()
    {

        
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), !ph);
            ph = !ph;
        }

        timer += Time.deltaTime;

        if (timer < 59)
            clock.GetComponent<SevenSegmentDriver>().Data = (Mathf.RoundToInt(timer)).ToString();
        else
        {
            string time;
            int min = Mathf.FloorToInt(Mathf.RoundToInt(timer) / 60);
            int sec = Mathf.RoundToInt(timer) - (min * 60);
            if(sec < 10)
                time = min.ToString() + "0" + sec;
            else
                time = min.ToString() + sec;

            clock.GetComponent<SevenSegmentDriver>().Data = time;
        }
    }

    //update occurs after cube is moved. Updates to the spot it is moved to
    public void updatePos(GameObject c, GameObject actual)
    {

        int indexC = System.Array.IndexOf(positions, c);
        int newPos = System.Array.IndexOf(positions, null);

        positions[indexC] = null;
        positions[newPos] = c;
        
        foreach(GameObject cube in positions)
        {
            if(cube != null)
                if(cube.GetComponent<cube_behaviour>().enabled == true)
                    cube.GetComponent<cube_behaviour>().enabled = false;

        }

        getMovable(GameObject.Find(indexC.ToString()));
        emptyPos = GameObject.Find(indexC.ToString());

        moves += 1;

        checkWin();

        string cubeName = positions[newPos].name;
        cubeName = cubeName.Substring(4);
        int no;
        int.TryParse(cubeName, out no);
        no += 1;

        fold.GetComponent<moves>().addMove(no.ToString() + " to " + (newPos + 1).ToString());

    }

    public void updatePos(GameObject c, GameObject actual, bool ia)
    {

        int indexC = System.Array.IndexOf(positions, c);
        int newPos = System.Array.IndexOf(positions, null);

        positions[indexC] = null;
        positions[newPos] = c;

        emptyPos = GameObject.Find(indexC.ToString());

        moves += 1;

        checkWin();

        string cubeName = positions[newPos].name;
        cubeName = cubeName.Substring(4);
        int no;
        int.TryParse(cubeName, out no);
        no += 1;

        fold.GetComponent<moves>().addMove(no.ToString() + " to " + (newPos + 1).ToString());

    }

    //initialize play field
    void initializeField()
    {
 
        List<int> numbers = new List<int> (9);
        for (int i = 0; i < 8; i++)
        {
            numbers.Add(i);
        }

        int empty = UnityEngine.Random.Range(0, numbers.Count);

        for (int i = 0; i < positions.Length; i++)
        {
            if (i != empty)
            {
                int thisNumber = UnityEngine.Random.Range(0, numbers.Count);
                positions[i] = GameObject.Find("Cube" + numbers[thisNumber].ToString());
                positions[i].transform.position = GameObject.Find(i.ToString()).transform.position;
                numbers.RemoveAt(thisNumber);
            }
        }

        //move cubes to pos
        for (int tel = 0; tel <= 8; tel++)
        {
            if (tel != empty)
            {
                GameObject other = GameObject.Find(tel.ToString());
                positions[tel].transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
            }
        }
        
        getMovable(GameObject.Find(empty.ToString()));
        emptyPos = GameObject.Find(empty.ToString());

        GetComponent<ai>().current_state = positions;
        //GetComponent<ai>().enabled = true;
    }

    //enable moveables
    void getMovable(GameObject empty)
    {

        RaycastHit hitup;
        RaycastHit hitdown;
        RaycastHit hitleft;
        RaycastHit hitright;

        Ray upRay = new Ray(empty.transform.position, Vector3.forward);
        Ray downRay = new Ray(empty.transform.position, -Vector3.forward);
        Ray leftRay = new Ray(empty.transform.position, -Vector3.right);
        Ray rightRay = new Ray(empty.transform.position, Vector3.right);

        if(Physics.Raycast(upRay, out hitup, 3))
        {

            if (hitup.collider.gameObject.tag == "Cube")
            {
                hitup.collider.gameObject.GetComponent<cube_behaviour>().enabled = true;
                hitup.collider.gameObject.GetComponent<cube_behaviour>().setAxis("down");
                hitup.collider.gameObject.GetComponent<cube_behaviour>().setEmpty(empty.transform.position);
            }

        }

        if (Physics.Raycast(downRay, out hitdown, 3))
        {

            if (hitdown.collider.gameObject.tag == "Cube")
            { 
                hitdown.collider.gameObject.GetComponent<cube_behaviour>().enabled = true;
                hitdown.collider.gameObject.GetComponent<cube_behaviour>().setAxis("up");
                hitdown.collider.gameObject.GetComponent<cube_behaviour>().setEmpty(empty.transform.position);
            }

        }

        if (Physics.Raycast(leftRay, out hitleft, 3))
        {

            if (hitleft.collider.gameObject.tag == "Cube")
            {
                hitleft.collider.gameObject.GetComponent<cube_behaviour>().enabled = true;
                hitleft.collider.gameObject.GetComponent<cube_behaviour>().setAxis("right");
                hitleft.collider.gameObject.GetComponent<cube_behaviour>().setEmpty(empty.transform.position);
            }

        }

        if (Physics.Raycast(rightRay, out hitright, 3))
        {

            if (hitright.collider.gameObject.tag == "Cube")
            { 
                hitright.collider.gameObject.GetComponent<cube_behaviour>().enabled = true;
                hitright.collider.gameObject.GetComponent<cube_behaviour>().setAxis("left");
                hitright.collider.gameObject.GetComponent<cube_behaviour>().setEmpty(empty.transform.position);
            }

        }

    }

    void moveForward()
    {

        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0.31f, transform.position.y, 0), Time.deltaTime);
        
    }
   
    public void save()
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerData data = new PlayerData();


        string[] posis = new string[9];
        for(int i = 0; i < 9; i++)
        {
            if (positions[i] != null)
                posis[i] = positions[i].name;
            else
            {
                posis[i] = "";
                data.empt = i;
            }

        }

        data.pois = posis;
        data.time = timer;
        data.ms = moves;
        data.mvs = GameObject.Find("Folder").GetComponent<moves>().moveList;

        bf.Serialize(file, data);

        file.Close();

        cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), false);
        phone.GetComponent<phone_camera>().ph = false;
        lPBut.interactable = true;
    }

    public void load()
    {
        int empty = -1;
        aiC.enabled = false;

        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {

            startsc.enabled = false;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < 9; i++)
            {
                if (data.pois[i] != "")
                    positions[i] = GameObject.Find(data.pois[i]);
                else
                {
                    positions[i] = null;
                    this.emptyPos = GameObject.Find(i.ToString());
                }

            }



            moves = data.ms;
            timer = data.time;
            GameObject.Find("Folder").GetComponent<moves>().moveList = data.mvs;
            GameObject.Find("Folder").GetComponent<moves>().fill(data.mvs);
            cnt.text = "Move count:" + data.mvs.Count.ToString();

            empty = data.empt;
            
            
            
            //move cubes na hulle plekke toe
            for (int tel = 0; tel <= 8; tel++)
            {
                if (tel != empty)
                {
                    GameObject other = GameObject.Find(tel.ToString());
                    positions[tel].transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
                }
            }
            getMovable(emptyPos);
        }

        cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), false);
        phone.GetComponent<phone_camera>().ph = false;
    }

    void checkWin()
    {
        
        if (!custom)
        {
            win = true;
            //open in last spot
            for (int tel = 0; tel < 9; tel++)
            {

                if (positions[tel] != GameObject.Find("Cube" + tel.ToString()))
                    win = false;

            }
            //open in first spot
            if (!win)
            {

                for (int tel = 1; tel < 9; tel++)
                {

                    if (positions[tel] != GameObject.Find("Cube" + (tel - 1).ToString()))
                        win = false;

                }

            }
        }
        else if(custom)
        {

            win = true;
            for (int i = 0; i < 9; i++)
            {

                if (positions[i] != goal[i])
                {
                    win = false;
                    break;
                }
            }
        }

        if (win)
        {

            int actTime;
            string num;

            num = GameObject.Find("7SDG").GetComponent<SevenSegmentDriver>().Input;
            num = num.Remove(num.Length - 1, 1);
            int.TryParse(num, out actTime);
            int sec = Mathf.FloorToInt(Mathf.RoundToInt(timer));

            
            float timey = timer;
            if (timer > 59)
            {
                float min = Mathf.FloorToInt((float)timer / 60);
                float seconds = Mathf.FloorToInt(timey - (min * 60));
                string mins;
                if (min > 1)
                    mins = min.ToString() + " minutes ";
                else
                    mins = min.ToString() + " minute ";
                winning.gameObject.GetComponent<Text>().text = "YOU WIN\nIt took you " + min.ToString() + " minutes and " + seconds.ToString()+" seconds\nand " + moves.ToString() + " moves";
            }
            else
            {
                winning.gameObject.GetComponent<Text>().text = "YOU WIN\nIt took you " + sec.ToString() + " seconds\nand " + moves.ToString() + " moves";
            }
            canv.GetComponent<Canvas>().enabled = true;
            
        }

    }

    public void restart()
    {

        Application.LoadLevel(Application.loadedLevel);

    }

    public void quit()
    {
        Application.Quit();
    }

    public void resume()
    {

        cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), false);
        phone.GetComponent<phone_camera>().ph = false;

    }

    public void solve()
    {

        cam.GetComponent<moveCamera>().move(new Vector3(-0.8665018f, 4.5235f, -6.037f), Quaternion.Euler(90, 0, 42.788f), false);
        phone.GetComponent<phone_camera>().ph = false;
        GetComponent<ai>().solve();
        aiC.enabled = true;

    }

    public Button rnd;
    public Button choose;
    public Button q;
    public Button l;

    public void startRandom()
    {
        initializeField();
        startsc.enabled = false;
        timer = 0;
    }

    public void setLayout()
    {

        rnd.gameObject.SetActive(false);
        q.gameObject.SetActive(false);
        l.gameObject.SetActive(false);
        choose.gameObject.SetActive(false);
        input.SetActive(true);
        
    }

    public InputField c1;
    public InputField c2;
    public InputField c3;
    public InputField c4;
    public InputField c5;
    public InputField c6;
    public InputField c7;
    public InputField c8;
    public InputField e;

    public void startLayout()
    {

        if (c1.text.Length == 1 && c2.text.Length == 1 && c3.text.Length == 1 && c4.text.Length == 1 && c5.text.Length == 1 && c6.text.Length == 1 && c7.text.Length == 1 && c8.text.Length == 1 && e.text.Length == 1)
        {

            List<string> nos = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9"};

            bool valid = true;

            if(nos.Contains(c1.text))
                nos.Remove(c1.text);
            if (nos.Contains(c2.text))
                nos.Remove(c2.text);
            if (nos.Contains(c3.text))
                nos.Remove(c3.text);
            if (nos.Contains(c4.text))
                nos.Remove(c4.text);
            if (nos.Contains(c5.text))
                nos.Remove(c5.text);
            if (nos.Contains(c6.text))
                nos.Remove(c6.text);
            if (nos.Contains(c7.text))
                nos.Remove(c7.text);
            if (nos.Contains(c8.text))
                nos.Remove(c8.text);

            if (nos.Count != 1)
                valid = false;

            if (valid)
            {
                startsc.enabled = false;
                timer = 0;
                moves = 0;
                GameObject.Find("Folder").GetComponent<moves>().moveList.Clear();
                GameObject.Find("Count").GetComponent<TextMesh>().text = "Move count: 0";
                GameObject.Find("mvList").GetComponent<TextMesh>().text = "";

                Array.Clear(positions, 0, positions.Length);

                positions[Convert.ToInt16(c1.text) - 1] = GameObject.Find("Cube0");
                positions[Convert.ToInt16(c2.text) - 1] = GameObject.Find("Cube1");
                positions[Convert.ToInt16(c3.text) - 1] = GameObject.Find("Cube2");
                positions[Convert.ToInt16(c4.text) - 1] = GameObject.Find("Cube3");
                positions[Convert.ToInt16(c5.text) - 1] = GameObject.Find("Cube4");
                positions[Convert.ToInt16(c6.text) - 1] = GameObject.Find("Cube5");
                positions[Convert.ToInt16(c7.text) - 1] = GameObject.Find("Cube6");
                positions[Convert.ToInt16(c8.text) - 1] = GameObject.Find("Cube7");

                for (int tel = 0; tel < 9; tel++)
                {
                    if (positions[tel] != null)
                        positions[tel].transform.position = GameObject.Find(tel.ToString()).transform.position;
                    else
                    {
                        emptyPos = GameObject.Find(tel.ToString());
                    }
                }

                getMovable(emptyPos);
                GetComponent<ai>().current_state = positions;
                GetComponent<ai>().goal_state = setGoal(e.text);
                goal = setGoal(e.text);
                custom = true;
            }
        }
    }

    GameObject[] setGoal (string endEmpty)
    {

        GameObject[] g = new GameObject[9];
        int end = Convert.ToInt16(endEmpty) - 1;
        int c = 0;

        for(int i = 0; i < 9; i++)
        {

            if (i != end)
            {
                
                g[i] = GameObject.Find("Cube" + c.ToString());
                c++;

            }
            else
                g[i] = null;

        }

        return g;
    }

    public void back()
    {

        rnd.gameObject.SetActive(true);
        q.gameObject.SetActive(true);
        choose.gameObject.SetActive(true);
        l.gameObject.SetActive(true);
        input.SetActive(false);
        
    }

    private void OnLevelWasLoaded(int level)
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {

            lMBut.interactable = true;
            lPBut.interactable = true;

        }
        else
        {
            lMBut.interactable = true;
            lPBut.interactable = true;
        }
    }
}

[Serializable]
class PlayerData
{

    public string[] pois = new string[9];
    public float time;
    public int ms;
    public List<string> mvs;
    public int empt;
}