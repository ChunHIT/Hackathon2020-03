using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public int person_id;
    private static GameObject LINE_INSTANCE;

    public List<Person> relations = new List<Person>();

    // -1 origin
    // 0 default
    // 1 normal
    // 2 angry
    // 3 big v

    public int type = 0;
    private Dictionary<Person, GameObject> relations_line = new Dictionary<Person, GameObject>();
    public static float average_distance = 6f;
    private static float my_distance;
    private static float hook_k = 3.0f;
    private static Vector3 y_axis = new Vector3(0, 1, 0);
    private static Material person_material;
    private void Awake()
    {
        if (LINE_INSTANCE == null)
        {
            LINE_INSTANCE = Resources.Load("LINE_INSTANCE") as GameObject;

            print(LINE_INSTANCE);
        }

        //设置自己的颜色
        Material mat = Instantiate(Resources.Load("person_default")) as Material;
        //mat.SetColor("_Color", new Color(40.0f / 255f, 159.0f / 255f, 202.0f / 255f, 1));
        //mat.set
        GetComponent<MeshRenderer>().material = mat;//;

        my_distance = average_distance + Random.Range(0.0f, 2.0f);
    }
    void Start()
    {

    }

    private Color target_color;

    public void SetColor(float r, float g, float b, float a)
    {
        if (gameObject.GetComponent<FadeColor>() == null)
        {
            gameObject.AddComponent<FadeColor>();
        }
        gameObject.GetComponent<FadeColor>().target_color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 force = new Vector3();
        //print(relations);
        foreach (Person p in relations)
        {
            //print(p);
            if (p == null)
            {
                continue;
            }
            Vector3 vec = p.transform.position - transform.position;
            //hook
            //f = -k * dx
            Vector3 f = -(vec.normalized * my_distance - vec) * hook_k;
            force += f;
        }
        this.GetComponent<Rigidbody>().AddForce(force);

        //关系画线
        foreach (Person p in relations)
        {
            if (p == null)
            {
                continue;
            }
            if (this.person_id < p.person_id)
            {
                if (!relations_line.ContainsKey(p))
                {
                    relations_line.Add(p, Instantiate(LINE_INSTANCE) as GameObject);
                }
                GameObject line = relations_line[p];
                Vector3 vec = p.transform.position - transform.position;
                Vector3 old_scale = line.transform.localScale;
                line.transform.position = transform.position + vec / 2;
                line.transform.localScale = new Vector3(old_scale.x, vec.magnitude, old_scale.z);
                line.transform.rotation = (Quaternion.LookRotation(y_axis, vec));
            }
        }

        if(type==2){
            transform.Find("angry_effect").gameObject.SetActive(true);
        }

    }

    private Vector3 hook_force(Vector3 p1, Vector3 p2)
    {
        Vector3 vec = p2 - p1;
        Vector3 f = -(vec.normalized * my_distance - vec) * hook_k;
        return f;

    }

    public void OnTriggerStay(Collider other)
    {
        //print(other.transform.name);
        Vector3 force = hook_force(transform.position, other.transform.position);
        //print(force);
        this.GetComponent<Rigidbody>().AddForce(force);

    }

}
