using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] List<GameObject> pfbCard;

    public Sprite GetMiniSprite(int idx)
    {
        return pfbCard[idx].GetComponent<ItemCard>().GetMiniSprite();
    }

    public GameObject GenerateCard(int pfbIdx)
    {
        GameObject goCard = GameObject.Instantiate(pfbCard[pfbIdx]);
        ItemCard itemCard = goCard.GetComponent<ItemCard>();
        itemCard.SetLastTime(LootManager.Instance.GetItemLastTime(pfbIdx));
        return goCard;
    }

}
