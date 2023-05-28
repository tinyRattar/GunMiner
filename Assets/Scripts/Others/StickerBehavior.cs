using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerBehavior : MonoBehaviour
{
    [SerializeField] Animator animator;
    Vector3 srcPos;
    Vector3 tarPos;
    bool inGotAnim = false;
    [SerializeField] float timeShow;
    [SerializeField] float timeFly;
    float timer = 0.0f;

    public void OnGot(Vector3 srcPos)
    {
        tarPos = this.transform.position;
        this.transform.position = srcPos;
        this.srcPos = srcPos;
        animator.Play("enter");
        inGotAnim = true;
    }

    public void SetShowAtLoad()
    {
        animator.Play("show");
    }

    // Update is called once per frame
    void Update()
    {
        if (inGotAnim)
        {
            timer += Time.deltaTime;
            if (timer < timeShow)
            {
                // do nothing
            }
            else if (timer < timeShow + timeFly)
            {
                float scale = (timer - timeShow) / timeFly;
                this.transform.position = Vector3.Lerp(srcPos, tarPos, scale);
            }
            else
            {
                this.transform.position = tarPos;
                inGotAnim = false;
            }
        }
    }
}
