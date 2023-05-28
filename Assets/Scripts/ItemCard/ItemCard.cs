using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCard : MonoBehaviour
{
    [SerializeField] int idx;
    [SerializeField] protected float lastTime;
    float timerUsed;
    [SerializeField] Sprite srCard;
    [SerializeField] Sprite srCardMini;

    public Sprite GetSprite()
    {
        return srCard;
    }

    public Sprite GetMiniSprite()
    {
        return srCardMini;
    }
    public void SetLastTime(float time)
    {
        lastTime = time;
    }
    public virtual void OnStartUse()
    {
    }
    
    /// <summary>
    /// called by update()
    /// </summary>
    public virtual float OnUsing()
    {
        timerUsed += Time.deltaTime;
        return timerUsed / lastTime;
    }

    public virtual void OnEndUse()
    {
        Destroy(this.gameObject);
    }
}
