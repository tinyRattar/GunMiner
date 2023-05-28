using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreFactory : MonoBehaviour
{
    [SerializeField] List<GameObject> pfbOres;
    
    public GameObject GenerateOre(OreInfo oreInfo, Vector3 position, Quaternion rotation, Transform genParent)
    {
        GameObject goOre = GameObject.Instantiate(pfbOres[oreInfo.PfbIndex], position, rotation, genParent);
        // tmp Code
        Ore ore = goOre.GetComponent<Ore>();
        ore.Init(oreInfo);
        ItemOre itemOre = goOre.GetComponent<ItemOre>();
        if (itemOre)
        {
            //int itemIdx = UnityEngine.Random.Range(0, 12);
            int itemIdx = LootManager.Instance.GetRandomIndexForOre();
            itemOre.SetSprite(ItemManager.Instance.GetMiniSprite(itemIdx));
            itemOre.SetItemIdx(itemIdx);
        }
        return goOre;
    }
}
