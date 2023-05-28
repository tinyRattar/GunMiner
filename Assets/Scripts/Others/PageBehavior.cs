using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SequenceAnim
{
    [SerializeField] List<Sprite> listSR;
    int curIdx = 0;

    public void Add(Sprite sr)
    {
        listSR.Add(sr);
    }

    public Sprite GetByIdx(int idx)
    {
        return listSR[idx];
    }

    public Sprite GetNext()
    {
        curIdx++;
        if (curIdx >= listSR.Count) curIdx = 0;
        return listSR[curIdx];
    }
}

public class PageBehavior : MonoBehaviour
{
    [SerializeField] float destroyDelay;
    [SerializeField] int animStateStep;
    [SerializeField] SpriteRenderer sr;
    //[SerializeField] List<Sprite> listPages; //tmp code
    [SerializeField] List<SequenceAnim> listSequenceAnim;
    [SerializeField] float sequenceInterval;
    [SerializeField] float switchPageTime;
    float timerSequence;
    int curIdx = 0;
    [SerializeField] Animator animator;

    public void DestroySelf()
    {
        MainManager.Instance.SetGamePause(false);
        Destroy(this.gameObject, destroyDelay);
    }

    //public void SetNextPage()
    //{
    //    curIdx++;
    //    //else
    //    //{
    //    //    timerSequence = sequenceInterval;
    //    //    //sr.sprite = listPages[curIdx];
    //    //}
    //}
    // Start is called before the first frame update
    void Start()
    {
        MainManager.Instance.SetGamePause(true);
        if (animator == null)
            animator = this.GetComponent<Animator>();
        //sr.sprite = listPages[curIdx];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            curIdx++;
            if (curIdx >= listSequenceAnim.Count)
            {
                animator.Play("exit");
            }
            else
            {
                animator.Play("flip");
                timerSequence = switchPageTime;
            }
            //int curState = animator.GetInteger("state");
            //animator.SetInteger("state", curState+animStateStep);
            //Destroy(this.gameObject, destroyDelay);
        }

        if (curIdx < listSequenceAnim.Count)
        {
            timerSequence -= Time.unscaledDeltaTime;
            if (timerSequence < 0.0f)
            {
                timerSequence = sequenceInterval;
                sr.sprite = listSequenceAnim[curIdx].GetNext();
            }
        }
    }
}
