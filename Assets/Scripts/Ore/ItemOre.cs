using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOre : Ore
{
    [SerializeField] int itemIdx;
    [SerializeField] GameObject pfbItemLoot;
    public override void OnGotcha()
    {
        bool ret = ItemManager.Instance.TryGetCard(itemIdx);
        if (ret)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // tmp code
            GameObject go = LootManager.Instance.GenerateOnce(1, this.transform.position, Quaternion.identity);
            ItemLoot itemLoot = go.GetComponent<ItemLoot>();
            itemLoot.SetItemCardIdx(itemIdx);
            Destroy(this.gameObject);
        }
        base.OnGotcha();
    }

    public void SetSprite(Sprite sp)
    {
        this.GetComponentInChildren<SpriteRenderer>().sprite = sp;
    }
    public void SetItemIdx(int idx)
    {
        itemIdx = idx;
    }

}
