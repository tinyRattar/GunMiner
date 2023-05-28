using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_UITimer : MonoBehaviour
{
    [SerializeField] List<Sprite> listSprites;
    [SerializeField] List<Sprite> listSprites_warning;
    [SerializeField] List<SpriteRenderer> srMinutes;
    [SerializeField] List<SpriteRenderer> srSeconds;
    [SerializeField] SpriteRenderer srColon;
    [SerializeField] int warningThresh;
    [SerializeField] AudioClip audioWarning;
    [SerializeField] float timeChange;
    int minute;
    int second;
    float timer;
    bool inWarningFlag = false;
    bool inWarning;
    GameObject sePlayerWarning;

    public void SetValue(int minute, int second)
    {
        this.minute = minute;
        this.second = second;

        if (minute * 60 + second <= warningThresh)
        {
            SetWarning(true);
        }
        else
        {
            SetWarning(false);
        }
    }

    public void StopAudioWarning()
    {
        if (sePlayerWarning) Destroy(sePlayerWarning.gameObject);
    }

    private void SetWarning(bool flag)
    {
        if (!inWarning && flag)
        {
            inWarning = true;
            if(audioWarning) sePlayerWarning = SEManager.Instance.PlaySE(audioWarning);
        }
        else if (inWarning && !flag)
        {
            inWarning = false;
        }
    }

    private void Update()
    {
        List<Sprite> _listSprite = listSprites;
        if (inWarning)
        {
            timer += Time.deltaTime;
            if (timer > timeChange)
            {
                timer = 0.0f;
                inWarningFlag = !inWarningFlag;
            }
            if (inWarningFlag)
                _listSprite = listSprites_warning;
        }

        srMinutes[0].sprite = _listSprite[minute / 10];
        srMinutes[1].sprite = _listSprite[minute % 10];
        srSeconds[0].sprite = _listSprite[second / 10];
        srSeconds[1].sprite = _listSprite[second % 10];
        srColon.sprite = _listSprite[10];
    }
}
