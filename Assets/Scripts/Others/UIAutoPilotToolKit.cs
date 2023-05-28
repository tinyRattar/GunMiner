using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAutoPilotToolKit : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SequenceAnim idleSequence;
    [SerializeField] Sprite spLeft;
    [SerializeField] Sprite spRight;
    [SerializeField] Sprite spBoth;
    [SerializeField] Sprite spNone;
    [SerializeField] float timeInterval;
    float timer;
    bool inIdle = false;

    public void SetState(int ret)
    {
        if (ret == 0)
            SetLeft();
        else if (ret == 1)
            SetRight();
        else
            SetNone();
    }

    public void SetIdle()
    {
        inIdle = true;
    }

    public void SetLeft()
    {
        sr.sprite = spLeft;
        inIdle = false;
    }
    public void SetRight()
    {
        sr.sprite = spRight;
        inIdle = false;
    }
    public void SetNone()
    {
        sr.sprite = spNone;
        inIdle = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inIdle)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                sr.sprite = idleSequence.GetNext();
                timer = timeInterval;
            }
        }
    }
}
