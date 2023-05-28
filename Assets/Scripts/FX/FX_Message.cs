using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FX_Message : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] float enterTime;
    [SerializeField] float existTime;
    [SerializeField] float exitTime;
    float timer;

    public void Init(string msg)
    {
        text.text = msg;
        timer = 0.0f;
        text.color = new Color(1, 1, 1, 0.0f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float scale = 0.0f;
        if (timer < enterTime)
        {
            scale = timer / enterTime;
            scale = Mathf.Clamp01(scale);
        }
        else if(timer<enterTime+existTime)
        {
            scale = 1;
        }
        else if (timer < enterTime + existTime + exitTime)
        {
            scale = 1 - (timer - enterTime - existTime) / exitTime;
            scale = Mathf.Clamp01(scale);
        }
        else
        {
            Destroy(this.gameObject);
        }
        text.color = new Color(1, 1, 1, scale);
    }
}
