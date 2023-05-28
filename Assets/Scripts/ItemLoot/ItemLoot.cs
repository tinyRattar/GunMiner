using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoot : MonoBehaviour
{
    [SerializeField] int goldValue;
    bool canLoot = true;
    [SerializeField] int itemCardIdx;
    [SerializeField] SpriteRenderer sr;

    [Header("µôÂäÓëÆ¯¸¡")]
    [SerializeField] float fallAccelerate;
    [SerializeField] float floatAccelerate;
    [SerializeField] float decayEnterWater;
    [SerializeField] float vanishSpeed;
    [SerializeField] float seaY;
    float curSpeed;
    bool inFall = true;
    [Header("·É³ö")]
    [SerializeField] float ranMinFlyOutSpeed;
    [SerializeField] float ranMaxFlyOutSpeed;
    [SerializeField] float ranMinFlyOutSpeedY;
    [SerializeField] float ranMaxFlyOutSpeedY;
    [SerializeField] float underSeaY;
    [SerializeField] float ranMinFlyOutSpeedY_underSea;
    [SerializeField] float ranMaxFlyOutSpeedY_underSea;
    float flyOutSpeed;
    float curFlyOutSpeed;
    [SerializeField] float timeFlyOut;
    float timerFlyOut;



    [Header("ÒôÐ§")]
    [SerializeField] AudioClip audioGotcha;

    public void ForceRemove()
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        LootManager.Instance.UnregistLoot(this);
        Destroy(this.gameObject);
    }

    public virtual bool OnGotcha()
    {
        if (canLoot)
        {
            if (itemCardIdx >= 0)
            {
                bool ret = ItemManager.Instance.TryGetCard(itemCardIdx);
                if (!ret)
                {
                    StartShift();
                    return false;
                }
            }
            MinerManager.Instance.ChangeMoney(goldValue);
            canLoot = false;
            if (audioGotcha) SEManager.Instance.PlaySE(audioGotcha);
            DestroySelf();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartShift(float ySpeed = 20.0f)
    {
        curSpeed = ySpeed + UnityEngine.Random.Range(ranMinFlyOutSpeedY, ranMaxFlyOutSpeedY);
        if(this.transform.position.y < underSeaY)
        {
            curSpeed = UnityEngine.Random.Range(ranMinFlyOutSpeedY_underSea, ranMaxFlyOutSpeedY_underSea);
        }
        canLoot = false;
        timerFlyOut = timeFlyOut;
        flyOutSpeed = UnityEngine.Random.Range(ranMinFlyOutSpeed, ranMaxFlyOutSpeed);
        flyOutSpeed = UnityEngine.Random.Range(0, 1.0f) > 0.5f ? flyOutSpeed : -flyOutSpeed;
        curFlyOutSpeed = flyOutSpeed;
        inFall = true;
    }

    public void SetItemCardIdx(int index)
    {
        itemCardIdx = index;
        if (itemCardIdx >= 0)
            sr.sprite = ItemManager.Instance.GetMiniSprite(itemCardIdx);
    }

    /// <summary>
    /// called by fixed update
    /// </summary>
    protected void FlyOut()
    {
        if (timerFlyOut > 0.0f)
        {
            timerFlyOut -= Time.fixedDeltaTime;
            if (timerFlyOut < 0.0f)
            {
                canLoot = true;
            }
            else
            {
                curFlyOutSpeed = timerFlyOut / timeFlyOut * flyOutSpeed;
                this.transform.Translate(new Vector3(curFlyOutSpeed, 0, 0) * Time.fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// called by fixed update
    /// </summary>
    protected void FallAndFloat()
    {
        FlyOut();
        float curY = this.gameObject.transform.position.y;
        if (inFall)
        {
            if (curY > seaY)
            {
                curSpeed -= fallAccelerate;
            }
            else
            {
                curSpeed += floatAccelerate;
            }
        }

        float moveDistance = curSpeed * Time.fixedDeltaTime;
        float tarY = curY + moveDistance;
        if((curY < seaY) ^ (tarY < seaY))
        {
            if (Mathf.Abs(curSpeed) > Mathf.Abs(vanishSpeed))
                curSpeed = curSpeed * decayEnterWater;
            else
            {
                curSpeed = 0;
                tarY = seaY;
                inFall = false;
            }
        }
        this.transform.Translate(new Vector3(0, tarY - curY, 0));
    }
    private void OnDestroy()
    {
        if (LootManager.Instance)
        {
            LootManager.Instance.UnregistLoot(this);
        }
    }
    private void FixedUpdate()
    {
        FallAndFloat();
    }

}
