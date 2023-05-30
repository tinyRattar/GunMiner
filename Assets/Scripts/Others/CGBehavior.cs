using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGBehavior : MonoBehaviour
{
    [SerializeField] List<SequenceAnim> listSequenceAnim;
    [SerializeField] float sequenceInterval;
    float timerSequence;
    [SerializeField] SpriteRenderer sr;
    int curIdx;
    [SerializeField] Animator animator;

    [Header("展示数据")]
    [SerializeField] Text txtKillNum;
    [SerializeField] Text txtMoney;
    [SerializeField] Text txtItemUse;
    [SerializeField] Text txtClamDestory;
    [SerializeField] Text txtLevelClear;
    [SerializeField] Text txtTotal;
    [SerializeField] GameObject highScoreMark;
    bool isHighScore;

    [Header("展示成就")]
    bool inAchieveShow;
    [SerializeField] GameObject pfbAchieveMark;
    [SerializeField] List<Sprite> listAchieveMarkSprites;
    [SerializeField] float timeAchieveMarkInverval;
    [SerializeField] Transform achievenMarkGenAnchor;
    [SerializeField] Vector3 achievenMarkGenOffset;
    int curAchieveIdx;
    float timerAchieveMark;
    List<Achievement> listAchieves;

    [Header("贴纸")]
    bool inRewardShow;
    [SerializeField] float timeRewardEnterInterval;
    int curRewardIdx;
    float timerReward;
    List<int> listRewardNew;

    [Header("空格提示")]
    [SerializeField] GameObject goSpaceHint;

    [Header("调试")]
    [SerializeField] bool startShowAchieve;

    public void StartCG(int idx)
    {
        curIdx = idx;
        animator.Play("enter");
        UpdateData();
    }

    public void StartAchieveShow()
    {
        inAchieveShow = true;
        curAchieveIdx = 0;
        listAchieves = AchievementManager.Instance.CheckAchievement();
    }

    public void StartRewardShow()
    {
        inRewardShow = true;
        curRewardIdx = 0;
        listRewardNew = AchievementManager.Instance.GetListRewardNew();
    }

    private void UpdateData()
    {
        txtKillNum.text = AchievementManager.Instance.GetValue(RecordType.killNum).ToString() + " * 10";
        txtMoney.text = AchievementManager.Instance.GetValue(RecordType.earnMoney).ToString() + " * 10";
        txtItemUse.text = AchievementManager.Instance.GetValue(RecordType.itemUsedNum).ToString() + " * 30";
        txtClamDestory.text = AchievementManager.Instance.GetValue(RecordType.clamDestroyNum).ToString() + " * 50";
        txtLevelClear.text = AchievementManager.Instance.GetValue(RecordType.levelClear).ToString() + " *500";
        txtTotal.text = AchievementManager.Instance.GetScore(out isHighScore).ToString();
    }

    private void DuringAchieveShow()
    {
        if (inAchieveShow)
        {
            timerAchieveMark -= Time.deltaTime;
            if (timerAchieveMark <= 0)
            {
                timerAchieveMark = timeAchieveMarkInverval;
                GenerateAchieveMark();
            }
        }
    }

    private void GenerateAchieveMark()
    {
        if (curAchieveIdx < listAchieves.Count) {
            Achievement achieve = listAchieves[curAchieveIdx];
            Vector3 genPos = achievenMarkGenAnchor.position;
            genPos += curAchieveIdx * achievenMarkGenOffset;
            GameObject go = GameObject.Instantiate(pfbAchieveMark, genPos, Quaternion.identity, achievenMarkGenAnchor);
            Sprite sprite = listAchieveMarkSprites[achieve.SpriteIndex];
            string nickname = achieve.Nickname;
            go.GetComponent<AchieveMark>().Init(sprite, nickname);
            curAchieveIdx++;
        }
        else
        {
            FinishAchieveShow();
        }
    }

    private void FinishAchieveShow()
    {
        if (isHighScore)
        {
            highScoreMark.SetActive(true);
        }
        inAchieveShow = false;
        StartRewardShow();
        //animator.Play("waitExit");
    }

    private void DuringRewardGet()
    {
        if (inRewardShow)
        {
            timerReward -= Time.deltaTime;
            if (timerReward <= 0)
            {
                timerReward = timeRewardEnterInterval;
                if (curRewardIdx < listRewardNew.Count)
                {
                    AchievementManager.Instance.StartRewardEnterAnim(listRewardNew[curRewardIdx]);
                    curRewardIdx++;
                }
                else
                {
                    FinishRewardShow();
                }
            }
        }
    }
    private void FinishRewardShow()
    {
        inRewardShow = false;
        MainManager.Instance.SetSpaceRestart();
        goSpaceHint.SetActive(true);
        //animator.Play("waitExit");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startShowAchieve)
        {
            startShowAchieve = false;
            StartAchieveShow();
        }

        timerSequence -= Time.unscaledDeltaTime;
        if (timerSequence < 0.0f)
        {
            timerSequence = sequenceInterval;
            sr.sprite = listSequenceAnim[curIdx].GetNext();
        }
        DuringAchieveShow();
        DuringRewardGet();
    }
}
