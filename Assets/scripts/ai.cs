using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ai : MonoBehaviour {

    public GameObject[] goal_state = new GameObject[9];
    public GameObject[] current_state = new GameObject[9];
    public List<int> c_int = new List<int>(9);
    public List<int> goal_int = new List<int>(9);

    public int depth = 0;
    public int statesVisited = 0;
    public bool found = false;

    public List<int> solution = new List<int>();
    public List<Node> open;
    public List<Node> closed;
    public GameObject control;

    public GameObject[] cubes = new GameObject[8];
    public UnityEngine.UI.Button solveButton;



    public void solve()
    {

        c_int = objToint(current_state);
        goal_int = objToint(goal_state);
        open.Add(new Node(null, -1, c_int, depth));
        Node p = open[0];

        if (solvable(c_int))
        {

            if (open[0].state.SequenceEqual(goal_int))
            {
                //begin op solution
            }

            open[0].setChildren(getChildren(open[0]));
            statesVisited = 1;

            while (open.Count > 0 && !found)
            {

                Node current = open[0];

                if (current.state.SequenceEqual(goal_int))
                {
                    solution = getSolution(current);
                    solveButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + solution.Count.ToString() + ")";
                    found = true;
                    depth = current.depth;
                    break;//klaar
                }
                statesVisited++;

                current.setChildren(getChildren(current));
                AddWithoutRepitition(current);

                for (int child = 0; child < current.children.Count; child++)
                {

                    bool inOpen = open.Contains(current.children[child]);
                    bool inClosed = closed.Contains(current.children[child]);
                    current.children[child].setMissed(current.children[child].state, goal_int);

                    if (inClosed && current.children[child].depth >= closed[closed.IndexOf(current.children[child])].getCost())
                    { }

                    if (!inOpen || open[open.IndexOf(current.children[child])].getCost() > current.children[child].depth)
                    {

                        open.Add(current.children[child]);

                    }
                }

                open.RemoveAt(0);
                //sort volgens lowest heuristic
                open.Sort(delegate (Node n1, Node n2)
                {
                    return n1.getCost().CompareTo(n2.getCost());
                });

                if (depth == 0)
                    depth++;


            }

        }
        else
        {
            solveButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Puzzle is unsolvable";
            solveButton.interactable = false;
        }

    }

    public void aiMove()
    {

        toggleAll(true);
        cubes[solution[0] - 1].GetComponent<cube_behaviour>().aiClick();
        
        solution.RemoveAt(0);
        solveButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Perform next move (" + solution.Count.ToString() + ")";
    }
	
    void toggleAll(bool toggle)
    {

        GameObject.Find("Cube0").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube1").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube2").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube3").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube4").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube5").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube6").GetComponent<cube_behaviour>().enabled = toggle;
        GameObject.Find("Cube7").GetComponent<cube_behaviour>().enabled = toggle;

    }

	// Update is called once per frame
	void Update () {

        
    }

    List<int> objToint(GameObject[] c)
    {

        List<int> ret = new List<int>(9);

        for(int i = 0; i < 9; i++)
        {
            if (c[i] != null)
            {
                int val;
                int.TryParse(c[i].gameObject.name.Substring(c[i].gameObject.name.Length - 1), out val);
                ret.Add(val + 1);
            }
            else
                ret.Add(0);
        }

        return ret;

    }

    List<int> getMovables(List<int> c)
    {
        List<int> mov = new List<int>(4);
        

        int empty = c.IndexOf(0);
        if (empty == 1 || empty == 4 || empty == 7)
        {
            try
            {
                mov.Add(c[empty - 3]);
            }
            catch (ArgumentOutOfRangeException)
            { }
            try
            {
                mov.Add(c[empty - 1]);
            }
            catch (ArgumentOutOfRangeException)
            { }
            try
            {
                mov.Add(c[empty + 1]);
            }
            catch (ArgumentOutOfRangeException)
            { }
            try
            {
                mov.Add(c[empty + 3]);
            }
            catch (ArgumentOutOfRangeException)
            { }
        }
        else if(empty == 0)
        {

            mov.Add(c[empty + 1]);
            mov.Add(c[empty + 3]);

        }
        else if (empty == 2)
        {

            mov.Add(c[empty - 1]);
            mov.Add(c[empty + 3]);

        }
        else if (empty == 3)
        {

            mov.Add(c[empty + 1]);
            mov.Add(c[empty + 3]);
            mov.Add(c[empty - 3]);

        }
        else if (empty == 5)
        {

            mov.Add(c[empty - 1]);
            mov.Add(c[empty + 3]);
            mov.Add(c[empty - 3]);

        }
        else if (empty == 6)
        {

            mov.Add(c[empty + 1]);
            mov.Add(c[empty - 3]);

        }
        else if (empty == 8)
        {

            mov.Add(c[empty - 1]);
            mov.Add(c[empty - 3]);

        }

        return mov;

    }

    int manhattanDistance(GameObject node, GameObject goalPos)
    {

        return Mathf.FloorToInt(Math.Abs(node.transform.position.x - goalPos.transform.position.x) + Math.Abs(node.transform.position.z - goalPos.transform.position.z));

    }

    int getInvCount(List<int> l)
    {

        int inv_count = 0;
        for (int i = 0; i < 9 - 1; i++)
            for (int j = i + 1; j < 9; j++)
                // Value 0 is used for empty space
                if (l[j] != 0 && l[i] != 0 && l[i] > l[j])
                    inv_count++;
        return inv_count;
    }

    bool solvable(List<int> pList)
    {
        int invCount = getInvCount(c_int);

        // return true if inversion count is even.
        return ((invCount % 2) == 0);

    }

    public List<int> getMissed(List<int> c)
    {
        List<int> missed = new List<int>(8);
        for (int i = 0; i < 9; i++)
        {

            if ((goal_int[i] != c[i]))
                if (goal_state[i] != null)
                {
                    missed.Add(goal_int[i]);
                    goal_state[i].GetComponent<cube_behaviour>().manh = manhattanDistance(goal_state[i], GameObject.Find((goal_state[i].name).ToCharArray()[4].ToString()));
                }

        }

        return missed;

    }

    List<Node> getChildren(Node par)
    {
        List<Node> tmp = new List<Node>(4);
        List<int> Mov = getMovables(par.state);
        for(int i = 0; i < Mov.Count; i++)
        {

            List<int> nState = new List<int>(par.state);


            int empty = nState.IndexOf(0);
            nState[nState.IndexOf(Mov[i])] = 0;
            nState[empty] = Mov[i];
            Node temp = new Node(par, Mov[i], nState, par.depth + 1);

            temp.missed = getMissed(temp.state).Count;

            if (temp.move != par.move)
            {
                tmp.Add(temp);
                
            }

        }
        return tmp;

    }

    void AddWithoutRepitition(Node current)
    {

        for (int i = 0; i < closed.Count; i++)
        {

            if (closed[i].state.Equals(current.state))
            {

                closed.RemoveAt(i);
                closed.Insert(i, current);

            }
            if (closed[i].getMD() > current.getMD())
            {

                closed.Insert(i, current);
                return;

            }

        }
        closed.Add(current);

    }

    void remove(List<int> st)
    {


        for(int i = 0; i < open.Count; i++)
        {

            if (open[i].state.Equals(st))
                remove(open[i].state);

        }

    }

    List<int> getSolution(Node c)
    {
        List<int> tmp = new List<int>();
        tmp.Add(c.move);

        while(c.parent.move != -1)
        {

            c = c.parent;
            tmp.Add(c.move);

        }

        tmp.Reverse();

        return tmp;

    }

}

public class Node : MonoBehaviour
{   
    //////
    //pointers
    public Node parent;
    public List<Node> children;
    public List<Node> siblings;
    //tree
    public int depth;
    public int move; //cube moved to reach the state of this Node
    public int f;
    //extra
    public int missed;
    public List<int> state;
    //////

    public Node(Node P, int M, List<int> st, int D)
    {

        setParent(P);
        setMove(M);
        setState(st);
        setDepth(D);
    }

    //////
    public void setParent(Node p)
    {
        this.parent = p;
    }

    public void setChildren(List<Node> c)
    {
        children = c;
    }

    public void setSiblings(List<Node> s)
    {
        siblings = s;
    }
    //////
    public void setMove(int m)

    {
        move = m;
    }

    public void setDepth(int d)
    {
        depth = d;
    }

    public int getMD()
    {
        //return missed + depth ;
        int md = 0;
        for (int i = 1; i <= 9; i++) //loop through die actual blokke
        {
            //should be at
            int r1 = rowLookup(i);
            int c1 = colLookup(i);
            //is at
            int r2 = rowLookup(this.state.IndexOf(i) + 1);
            int c2 = colLookup(this.state.IndexOf(i) + 1);
            //md
            md += Math.Abs(r1 - r2) + Math.Abs(c1 - c2);

        }

        return md;
    }

    public void setState(List<int> s)
    {
        state = s;
    }

    public List<int> getState()
    {
        return this.state;
    }

    public int rowLookup(int i)
    {
        if ((i >= 1) && (i <= 3))
            return 1;
        if ((i >= 4) && (i <= 6))
            return 2;
        if ((i >= 7) && (i <= 9))
            return 3;

        return -1;
    }

    public int colLookup(int i)
    {

        if (i <= 3)
            return i;
        if (i <= 6)
            return i - 3;
        if (i <= 9)
            return i - 6;

        return -1;

    }

    public void Add(List<Node> list, Node current)
    {

        for (int i = 0; i < list.Count; i++)
        {

            if(list[i].getCost() > current.getCost())
            {

                list.Insert(i, current);
                return;
            }

        }
        list.Add(current);

    }

    public int setMissed(List<int> c, List<int> goal)
    {
        List<int> missed = new List<int>(8);
        for (int i = 0; i < 9; i++)
        {

            if ((goal[i] != c[i]))
                if (goal[i] != 0)
                {
                    missed.Add(goal[i]);                   
                }

        }

        return missed.Count;

    }

    public int getCost()
    {

        return this.depth + this.getMD();

    }
}
