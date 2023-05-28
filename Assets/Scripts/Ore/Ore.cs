using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OreOnHitBehavior
{
    none,
    destory,
    convert,
    canDrag
}

public enum OreType
{
    junk,
    fish,
    clam,
    pearl,
    other
}

public class Ore : MonoBehaviour
{
    [SerializeField] OreType oreType;
    [SerializeField] int moneyValue;
    [SerializeField] int scoreValue;
    [SerializeField] protected float weight;
    [SerializeField] protected bool cannotDrag = false;
    [SerializeField] float breakPoint;
    [SerializeField] bool autoSize;
    [SerializeField] protected List<Sprite> listSprites;
    [SerializeField] OreOnHitBehavior oreOnHitBehavior;
    protected Claw claw;
    [SerializeField] float convertProp;
    [SerializeField] OreInfo convertOreInfo;
    [SerializeField] SpriteRenderer convertSR;
    [SerializeField] Sprite convertSprite;
    [SerializeField] GameObject vfxConvert;
    [SerializeField] bool canDragByNet = true;

    public bool IsOnDrag()
    {
        if (claw)
            return true;
        else
            return false;
    }
    public bool CanDragByNet()
    {
        return canDragByNet;
    }
    public void ForceRemove()
    {
        if (claw)
            claw.TryRefreshDragingOre(this, OreOnHitBehavior.destory);
        DestroySelf();
    }

    private void DestroySelf(bool dropCoin = false, float luckBounus = 0.0f)
    {
        if (dropCoin)
        {
            int dropNum = moneyValue;
            if (UnityEngine.Random.Range(0, 1.0f) < luckBounus)
            {
                dropNum = dropNum * 10;
            }
            for (int i = 0; i < dropNum; i++)
            {
                GameObject goLoot = LootManager.Instance.GenerateOnce(0, this.transform.position, Quaternion.identity);
            }
        }
        OreManager.Instance.UnregistOre(this);
        Destroy(this.gameObject);
    }


    public virtual bool OnHit(DamageInfo info)
    {
        if (info.DType == DamageType.explode || (info.DType == DamageType.melee && info.DamageValue >= breakPoint))
        {
            switch (oreOnHitBehavior)
            {
                case OreOnHitBehavior.none:
                    break;
                case OreOnHitBehavior.destory:
                    if (claw)
                        claw.TryRefreshDragingOre(this, oreOnHitBehavior);
                    DestroySelf(true);
                    return true;
                case OreOnHitBehavior.convert:
                    if(oreType == OreType.clam)
                        AchievementManager.Instance.ChangeValue(RecordType.clamDestroyNum, 1);
                    if (UnityEngine.Random.Range(0, 1.0f) < convertProp)
                    {
                        GameObject go = OreManager.Instance.GenerateOre(convertOreInfo, this.transform.position);
                        Ore new_ore = go.GetComponent<Ore>();
                        if (claw)
                        {
                            claw.TryRefreshDragingOre(this, oreOnHitBehavior);
                            claw.StartDragOre(new_ore);
                        }
                        DestroySelf(true);
                    }
                    else
                    {
                        if (claw)
                            claw.TryRefreshDragingOre(this, OreOnHitBehavior.destory);
                        DestroySelf(true);
                    }
                    break;
                case OreOnHitBehavior.canDrag:
                    cannotDrag = false;
                    convertSR.sprite = convertSprite;
                    claw.TryRefreshDragingOre(this, oreOnHitBehavior);
                    if (vfxConvert) GameObject.Instantiate(vfxConvert, this.transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
        return false;
    }

    public virtual void Init(OreInfo info)
    {
        moneyValue = info.MoneyValue;
        scoreValue = info.ScoreValue;
        weight = info.Weight;
        // tmp code
        if (autoSize)
            this.transform.localScale = Vector3.one * weight * 0.5f;
        int ranIdx = UnityEngine.Random.Range(0, listSprites.Count);
        this.GetComponentInChildren<SpriteRenderer>().sprite = listSprites[ranIdx];
        OreManager.Instance.RegistOre(this);
        //if (moneyValue >= 10)
        //{
        //    this.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        //}else if (moneyValue >= 100)
        //{
        //    this.GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1.0f);
        //}
    }
    public float GetWeight() { return weight; }
    public bool IsCannotDrag() { return cannotDrag; }

    public virtual void StartOnDrag(Claw srcClaw)
    {
        claw = srcClaw;
        // do nothing now
    }

    public virtual void OnGotcha()
    {
        if(oreType == OreType.fish)
            AchievementManager.Instance.ChangeValue(RecordType.fishGotchaNum, 1);
        else if (oreType == OreType.junk)
            AchievementManager.Instance.ChangeValue(RecordType.junkGotchaNum, 1);
        MinerManager.Instance.ChangeMoney(moneyValue);
        MinerManager.Instance.ChangeScore(scoreValue);
        DestroySelf();
    }

}
