using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] List<ItemSlot> itemSlots;
    [SerializeField] List<ItemCard> holdCards;
    [SerializeField] int maxHold = 1;
    [SerializeField] ItemFactory itemFactory;

    [SerializeField] GameObject uiHoldCard;

    [SerializeField] AudioClip audioUseCardSuccess;
    [SerializeField] AudioClip audioUseCardFailed;

    public Sprite GetMiniSprite(int idx)
    {
        return itemFactory.GetMiniSprite(idx);
    }
    public bool TryGetCard(ItemCard card)
    {
        if (holdCards.Count >= maxHold)
            return false;
        else
        {
            holdCards.Add(card);
            return true;
        }
    }

    public bool TryGetCard(int idx)
    {
        // todo:check it
        GameObject goCard = itemFactory.GenerateCard(idx);
        holdCards.Add(goCard.GetComponent<ItemCard>());
        if (holdCards.Count > maxHold)
        {
            if (TryUseCard(0, maxHold))
                return true;
            else if (TryUseCard(1, maxHold))
                return true;
            else
            {
                holdCards.RemoveAt(maxHold);
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool TryUseCard(int slotIdx, int holdIdx = 0)
    {
        bool ret;
        if (holdCards.Count <= holdIdx)
        {
            ret = false;
        }
        else
        {
            ret = itemSlots[slotIdx].TryUseCard(holdCards[holdIdx]);
            if (ret)
                holdCards.RemoveAt(holdIdx);
        }

        if (ret)
        { 
            if (audioUseCardSuccess) SEManager.Instance.PlaySE(audioUseCardSuccess);
            AchievementManager.Instance.ChangeValue(RecordType.itemUsedNum, 1);
        }
        else
        { 
            if (audioUseCardFailed) SEManager.Instance.PlaySE(audioUseCardFailed); 
        }

        return ret;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    TryUseCard(0);
        //}
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    TryUseCard(1);
        //}

        //tmp code
        if (holdCards.Count > 0)
        {
            uiHoldCard.SetActive(true);
            uiHoldCard.GetComponent<Image>().sprite = holdCards[0].GetSprite();
        }
        else
        {
            uiHoldCard.SetActive(false);
        }
    }

    public void UseCardByInput(int index)
    {
        TryUseCard(index);
    }
}
