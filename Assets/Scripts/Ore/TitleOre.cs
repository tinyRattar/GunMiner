using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TitleOreType
{
    startGame,
    showPage
}
public class TitleOre : Ore
{
    [SerializeField] TitleOreType titleOreType;
    [SerializeField] GameObject pfbPage;
    [SerializeField] Transform genAnchor;
    AsyncOperation asyncOperation;
    bool inFadeOut;
    [SerializeField] float fadeOutTime;
    [SerializeField] Animator animatorFadeOut;
    float timerFadeOut;

    [Header("ÌבÊ¾")]
    [SerializeField] GameObject hintSteam;

    public override void OnGotcha()
    {
        switch (titleOreType)
        {
            case TitleOreType.startGame:
                InputManager.Instance.SetFreezeInput(true);
                animatorFadeOut.Play("fadeOut");
                inFadeOut = true;
                break;
            case TitleOreType.showPage:
                GameObject.Instantiate(pfbPage, genAnchor.position, Quaternion.identity, genAnchor);
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
        //Destroy(this.gameObject);
    }

    public override bool OnHit(DamageInfo info)
    {
        bool ret = base.OnHit(info);
        if (hintSteam) hintSteam.SetActive(false);

        return ret;
    }

    public override void StartOnDrag(Claw srcClaw)
    {
        if (hintSteam) hintSteam.SetActive(true);
        base.StartOnDrag(srcClaw);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (titleOreType == TitleOreType.startGame)
        {
            asyncOperation = SceneManager.LoadSceneAsync(1);
            asyncOperation.allowSceneActivation = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inFadeOut)
        {
            timerFadeOut += Time.deltaTime;
            if (timerFadeOut > fadeOutTime)
            {
                asyncOperation.allowSceneActivation = true;
                inFadeOut = false;
            }
        }
    }
}
