using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonGraph : MonoBehaviour
{
    public static Dictionary<Person, List<Person>> full_map;

    public static List<Person> known_perple = new List<Person>();

    private static GameObject PERSON_INSTANCE;
    public static float spread_rate = 0.5f;
    public static float spread_angry_rate = 0.3f;

    private void Awake()
    {
       
    }

    public static List<Person> Spread()
    {
        List<Person> s = new List<Person>();
        foreach (Person p in full_map.Keys)
        {
            if (p.type == -1 || p.type == 1 || p.type ==2)
            {
                foreach (Person neig in p.relations)
                {
                    if (neig.type == 0 && Random.Range(0.0f, 1.0f) < spread_rate)
                    {
                        s.Add(neig);
                    }
                }
            }
        }
        foreach (Person p in s)
        {
            if (Random.Range(0.0f, 1.0f) < spread_angry_rate)
            {//angry
                p.type = 2;
                p.SetColor(212, 52, 38, 255);
            }
            else
            {
                p.type = 1;
                p.SetColor(212, 146, 40, 255);
            }

            if(!known_perple.Contains(p)){
                known_perple.Add(p);
            }

        }
        return s;
    }

    private static int idCounter = 0;
    public static Dictionary<Person, List<Person>> GenMap(int size)
    {
        if(PERSON_INSTANCE == null){
             PERSON_INSTANCE = Resources.Load("Person") as GameObject;
        }

        GameObject[,] objs_map = new GameObject[size, size];
        Dictionary<Person, List<Person>> dic = new Dictionary<Person, List<Person>>();
        //创建物体
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {

                if ((i == size / 2 && j == size / 2) || Random.Range(0, 10) > 2)
                {
                    Vector3 pos = new Vector3((i - size / 2) * Person.average_distance, 0, (j - size / 2) * Person.average_distance);
                    Vector3 pos_bias = new Vector3(Random.Range(0, 2), 0, Random.Range(0, 2));
                    pos = pos + pos_bias;
                    GameObject p = Instantiate(PERSON_INSTANCE);
                    p.transform.position = pos;

                    Person pc = p.GetComponent<Person>();

                    pc.person_id = idCounter++;
                    p.transform.name = "P" + (idCounter - 1);
                    objs_map[i, j] = p;
                    dic.Add(pc, new List<Person>());
                }
            }
        }

        Game.main_character = objs_map[size / 2, size / 2].GetComponent<Person>();
        Game.main_character.type = -1;

        //创建关系
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (objs_map[i, j] != null)
                {
                    GameObject p = objs_map[i, j];
                    bool connect = false;
                    for (int m = 0; m <= 1; m++)
                    {
                        for (int n = 0; n <= 1; n++)
                        {
                            if ((m != 0 || n != 0) && Random.Range(0, 20) > 3 && i + m >= 0 && i + m < (size) && j + n >= 0 && j + n < (size) && objs_map[i + m, j + n] != null)
                            {
                                GameObject neighbor = objs_map[i + m, j + n];
                                //双向添加
                                Person p1c = p.GetComponent<Person>();
                                Person p2c = neighbor.GetComponent<Person>();
                                p1c.relations.Add(p2c);
                                p2c.relations.Add(p1c);
                                //p.GetComponent<Person>().relations.Add(neighbor.GetComponent<Person>());
                                //neighbor.GetComponent<Person>().relations.Add(p.GetComponent<Person>());
                                dic[p1c].Add(p2c);
                                dic[p2c].Add(p1c);
                            }
                        }
                    }
                    if (!connect)
                    {
                        for (int m = 0; m <= 1; m++)
                        {
                            if (connect)
                            {
                                break;
                            }
                            for (int n = 0; n <= 1; n++)
                            {
                                if (connect)
                                {
                                    break;
                                }
                                if ((m != 0 || n != 0) && i + m >= 0 && i + m < (size) && j + n >= 0 && j + n < (size) && objs_map[i + m, j + n] != null)
                                {
                                    GameObject neighbor = objs_map[i + m, j + n];
                                    //双向添加
                                    Person p1c = p.GetComponent<Person>();
                                    Person p2c = neighbor.GetComponent<Person>();
                                    dic[p1c].Add(p2c);
                                    dic[p2c].Add(p1c);
                                    p1c.relations.Add(p2c);
                                    p2c.relations.Add(p1c);
                                    //p.GetComponent<Person>().relations.Add(neighbor.GetComponent<Person>());
                                    //neighbor.GetComponent<Person>().relations.Add(p.GetComponent<Person>());
                                    connect = true;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }
        List<Person> toRemove = new List<Person>();
        foreach (Person g in dic.Keys)
        {
            if (dic[g].Count == 0)
            {
                toRemove.Add(g);
            }
        }
        foreach (Person g in toRemove)
        {
            dic.Remove(g);
            Destroy(g);
        }


        return dic;

    }

}
