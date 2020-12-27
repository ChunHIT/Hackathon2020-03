using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeColor : MonoBehaviour
{
    // Start is called before the first frame update

    private float start_time;
    public Color target_color;
    public Color old_color;
    void Start()
    {
        start_time = Time.time;
        old_color = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target_color ==null){
            return;
        }
        
        float t = Time.time - start_time;
        if (t < 1.0f)
        {
            float proportion = (t / 1.0f);
            float r = Mathf.Lerp(old_color.r, target_color.r, proportion);
            float g = Mathf.Lerp(old_color.g, target_color.g, proportion);
            float b = Mathf.Lerp(old_color.b, target_color.b, proportion);
            float a = Mathf.Lerp(old_color.a, target_color.a, proportion);
            GetComponent<Renderer>().material.color = new Color(r,g,b,a);
        }
        else {
            Destroy(this);
        }
    }
}
