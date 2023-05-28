using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_UICounter : MonoBehaviour
{
    [SerializeField] List<Sprite> listSprites;
    [SerializeField] List<SpriteRenderer> srNums;
    [SerializeField] float visValueChangeTime;
    [SerializeField] float minVisValueChangePerSecond;
    float visValueChangePerSecond;
    float visValue;
    int value;

    public void SetValue(int value)
    {
        this.value = value;
        visValueChangePerSecond = Mathf.Abs(visValue - value) / visValueChangeTime;
        if (visValueChangePerSecond < minVisValueChangePerSecond) visValueChangePerSecond = minVisValueChangePerSecond;
    }

    private void Update()
    {
        if (visValue > value)
        {
            visValue -= visValueChangePerSecond * Time.deltaTime;
            if (visValue < value)
                visValue = value;
        }
        else if(visValue < value)
        {
            visValue += visValueChangePerSecond * Time.deltaTime;
            if (visValue > value)
                visValue = value;
        }

        srNums[0].sprite = listSprites[(int)(visValue / 1000 % 10)];
        srNums[1].sprite = listSprites[(int)(visValue / 100 % 10)];
        srNums[2].sprite = listSprites[(int)(visValue / 10 % 10)];
        srNums[3].sprite = listSprites[(int)(visValue / 1 % 10)];

    }
}
