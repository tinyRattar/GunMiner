using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBehavior : MonoBehaviour
{
    [SerializeField] Text txtContent;
    [SerializeField] string text;
    int curSize;
    [SerializeField] float timePerCharacter;
    float timerTxt;
    bool inShow = false;
    [SerializeField] float showPosY;
    [SerializeField] float hidePosY;
    [SerializeField] float timeShow;
    [SerializeField] float timeTotalHideTolerance;
    [SerializeField] float timeSwitchTolerance;
    [SerializeField] Animator animator;
    float timerShow;
    [SerializeField] AudioClip audioTalk;
    [SerializeField] List<AudioClip> audioTalks; //0:universal 1:introduce 2:notEnoughMoney 3:Bye

    public bool IsTotalHide()
    {
        if (!inShow && timerShow <= timeTotalHideTolerance)
            return true;
        else
            return false;
    }
    

    public void SetShow(bool flag, int audioIdx = 0)
    {
        if (flag)
        {
            //if (audioTalk) { SEManager.Instance.PlaySE(audioTalk); }
            SEManager.Instance.PlaySE(audioTalks[audioIdx]);
        }
        if (!inShow)
        {
            if (flag)
            {
                if (timerShow <= timeSwitchTolerance)
                    animator.Play("switch");
                else
                    animator.Play("enter");
            }
        }
        else
        {
            if (flag)
                animator.Play("switch");
            else
                animator.Play("exit");
        }
        inShow = flag;
        txtContent.text = "";
    }
    public bool SetContent(string content)
    {
        bool ret = false;
        if (!inShow || text != content)
        {
            timerTxt = 0.0f;
            ret = true;
        }
        text = content;
        return ret;
    }

    private void UpdateTxt()
    {
        timerTxt += Time.deltaTime;
        curSize = (int)(timerTxt / timePerCharacter);
        if (curSize > text.Length) curSize = text.Length;
        if (curSize + 8 < text.Length && text.Substring(curSize, 8) == "<color=#")
        {
            while (curSize + 8 < text.Length)
            {
                if (text.Substring(curSize, 8) == "</color>")
                    break;
                curSize += 1;
            }
            curSize += 8;
            timerTxt = timePerCharacter * curSize;
        }

        txtContent.text = text.Substring(0, curSize);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inShow && timerShow < timeShow)
            {
                timerShow = timeShow;
                animator.Play("idle");
            }
            else
            {
                if (timerTxt < text.Length * timePerCharacter)
                    timerTxt = text.Length * timePerCharacter;
                else
                    SetShow(false);
            }
        }
        if (inShow)
            timerShow += Time.deltaTime;
        else
            timerShow -= Time.deltaTime;
        timerShow = Mathf.Clamp(timerShow, 0, timeShow);
        if (timerShow >= timeShow)
        {
            UpdateTxt();
        }
        float y = hidePosY + (showPosY - hidePosY) * timerShow / timeShow;
        Vector3 pos = this.transform.position;
        pos.y = y;
        this.transform.position = pos;
    }
}
