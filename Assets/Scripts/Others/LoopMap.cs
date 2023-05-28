using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMap : MonoBehaviour
{
    [SerializeField] GameObject src;
    [SerializeField] float maxDistance;
    [SerializeField] float width;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dis = src.transform.position - this.transform.position;
        if (dis.x > maxDistance)
        {
            this.transform.Translate(new Vector3(width * 3, 0, 0));
        }
        else if (dis.x < -maxDistance)
        {
            this.transform.Translate(new Vector3(-width * 3, 0, 0));
        }
    }
}
