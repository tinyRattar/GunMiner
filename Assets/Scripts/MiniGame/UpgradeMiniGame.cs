using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMiniGame : MonoBehaviour
{
    enum State
    {
        enter,
        select,
        practice,
        finish
    }

    bool inGame;
    State curState;
    [SerializeField] Animator animator;
    [SerializeField] float enterTime;
    [SerializeField] float exitTime;
    float timer;
    int curSelect;

    [SerializeField] int maxPractice;
    int curPractice;
    [SerializeField] SpriteRenderer srPractice;
    [SerializeField] List<Sprite> listSpritePracticeP1;
    [SerializeField] List<Sprite> listSpritePracticeP2;
    bool waitLeftPress;
    [SerializeField] SpriteRenderer srHintLeft;
    [SerializeField] List<Sprite> listSpriteHintLeftP1;
    [SerializeField] List<Sprite> listSpriteHintLeftP2;
    [SerializeField] SpriteRenderer srHintRight;
    [SerializeField] List<Sprite> listSpriteHintRightP1;
    [SerializeField] List<Sprite> listSpriteHintRightP2;

    [Header("µÚ¶þ´Î¶ÍÁ¶")]
    bool inSecondChange = false;
    int price = 0;
    int disableIdx = -1;
    [SerializeField] GameObject goSecondChanceHint;
    [SerializeField] FX_UICounter fxPrice;
    [SerializeField] List<SpriteRenderer> srPlayer;
    [SerializeField] Color colorEnable;
    [SerializeField] Color colorDisable;

    [SerializeField] Image imgProcess;
    [SerializeField] Text txtSelectTalk;

    [SerializeField] AudioClip audioSuccess;

    public void StartMiniGame(bool isSecondChance = false)
    {
        inGame = true;
        curSelect = 0;
        curPractice = 0;
        curState = State.enter;
        animator.Play("enter");
        timer = enterTime;

        if (!isSecondChance)
        {
            inSecondChange = false;
            disableIdx = -1;
            price = 0;
            txtSelectTalk.text = StringManager.Instance.GetText(6, curSelect, 0);
            goSecondChanceHint.SetActive(false);
            fxPrice.SetValue(0);
            srPlayer[0].color = colorEnable;
            srPlayer[1].color = colorEnable;
        }
    }

    public void StartMiniGame_SecondChance(int disableIdx, int price)
    {
        StartMiniGame();
        inSecondChange = true;
        this.disableIdx = disableIdx;
        this.price = price;
        txtSelectTalk.text = StringManager.Instance.GetText(6, curSelect, curSelect == disableIdx ? 1 : 0);
        if(disableIdx == 0)
        {
            curSelect = 1;
            animator.Play("enter2");
        }
    }

    public void FinishMiniGame()
    {
        inGame = false;
        //animator.Play("exit");
        if (curSelect == disableIdx)
            ShopManager.Instance.OnMiniGameFinish(2);
        else
            ShopManager.Instance.OnMiniGameFinish(curSelect);
    }

    private void DuringSelect()
    {
        txtSelectTalk.text = StringManager.Instance.GetText(6, curSelect, curSelect == disableIdx ? 1 : 0);
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.J))
        {
            if (curSelect == 1)
            {
                animator.Play("selectLeft");
            }
            curSelect = 0;
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L))
        {
            if(curSelect == 0)
            {
                animator.Play("selectRight");
            }
            curSelect = 1;
        }

        if (inSecondChange)
        {
            srPlayer[disableIdx].color = colorDisable;
            srPlayer[1 - disableIdx].color = colorEnable;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ShopManager.Instance.IsSaying())
            {
                return;
            }
            if(curSelect == disableIdx)
            {
                goSecondChanceHint.SetActive(false);
                FinishMiniGame();
                animator.Play("interrupt");
            }
            else if (MinerManager.Instance.GetCurMoney() >= price)
            {
                MinerManager.Instance.ChangeMoney(-price);
                curState = State.practice;
                animator.Play("showPractice");
                goSecondChanceHint.SetActive(false);
            }
            else
            {
                ShopManager.Instance.PleaseSay(4, -1, 0);
            }
        }
    }

    private void DuringPractice()
    {
        if(curSelect == 0)
        {
            if (waitLeftPress && Input.GetKeyDown(KeyCode.A))
            {
                waitLeftPress = false;
                curPractice++;
            }
            else if(!waitLeftPress && Input.GetKeyDown(KeyCode.D))
            {
                waitLeftPress = true;
                curPractice++;
            }
            int spIdx = curPractice % listSpritePracticeP1.Count;
            srPractice.sprite = listSpritePracticeP1[spIdx];
            spIdx = waitLeftPress ? 0 : 1;
            srHintLeft.sprite = listSpriteHintLeftP1[spIdx];
            srHintRight.sprite = listSpriteHintRightP1[1-spIdx];
        }
        else
        {
            if (waitLeftPress && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.J)))
            {
                waitLeftPress = false;
                curPractice++;
            }
            else if (!waitLeftPress && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L)))
            {
                waitLeftPress = true;
                curPractice++;
            }
            int spIdx = curPractice % listSpritePracticeP2.Count;
            srPractice.sprite = listSpritePracticeP2[spIdx];
            spIdx = waitLeftPress ? 0 : 1;
            srHintLeft.sprite = listSpriteHintLeftP2[spIdx];
            srHintRight.sprite = listSpriteHintRightP2[1 - spIdx];
        }
        float scale = (float)curPractice / maxPractice;
        scale = Mathf.Clamp01(scale);
        imgProcess.fillAmount = scale;

        if (curPractice > maxPractice)
        {
            animator.Play("success");
            curState = State.finish;
            timer = enterTime;
            if (audioSuccess) SEManager.Instance.PlaySE(audioSuccess);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame)
        {
            switch (curState)
            {
                case State.enter:
                    if (timer <= 0)
                    {
                        curState = State.select;
                        if (inSecondChange)
                        {
                            goSecondChanceHint.SetActive(true);
                            fxPrice.SetValue(price);
                        }
                    }
                    else
                        timer -= Time.deltaTime;
                    break;
                case State.select:
                    DuringSelect();
                    break;
                case State.practice:
                    DuringPractice();
                    break;
                case State.finish:
                    timer -= Time.deltaTime;
                    if (timer < 0.0f || Input.GetKeyDown(KeyCode.Space))
                    {
                        FinishMiniGame();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
