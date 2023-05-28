using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ShopCoin : MonoBehaviour
{
    [SerializeField] float timeFly;
    [SerializeField] float timeFade;
    [SerializeField] SpriteRenderer srFade;
    float timer;
    Vector3 srcPos;
    Vector3 midPos;
    Vector3 tarPos;

    public void Init(Vector3 srcPos, Vector3 midPos, Vector3 tarPos)
    {
        this.srcPos = srcPos;
        this.midPos = midPos;
        this.tarPos = tarPos;
        timer = 0.0f;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < timeFly)
        {
            this.transform.position = BezierUtil.BezierLerp(srcPos, midPos, tarPos, timer / timeFly);
        }
        else if (timer < timeFly + timeFade)
        {
            this.transform.position = tarPos;
            float scale = 1 - (timer - timeFly) / timeFade;
            srFade.color = new Color(scale, scale, scale, scale);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
