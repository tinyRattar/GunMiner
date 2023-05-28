using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    ItemCard curCard;
    [SerializeField] Transform uiCardAnchor;
    [SerializeField] Image uiCard;
    [SerializeField] GameObject vfx_useCard;
    [SerializeField] float vfx_genOffset;

    [SerializeField] SpriteRenderer srBase;
    [SerializeField] List<Sprite> listBaseSprites;
    [SerializeField] List<Vector3> listPos;

    [SerializeField] AudioClip audioStageChange;

    int lastIdx;

    public bool TryUseCard(ItemCard card)
    {
        if (curCard)
        {
            return false;
        }
        else
        {
            lastIdx = 0;
            uiCard.fillAmount = 1;
            uiCard.transform.localPosition = listPos[0];
            srBase.sprite = listBaseSprites[0];
            curCard = card;
            uiCard.sprite = card.GetSprite();
            card.OnStartUse();
            Vector3 genPos = uiCardAnchor.transform.position + new Vector3(UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), 0);
            if (vfx_useCard) GameObject.Instantiate(vfx_useCard, genPos, Quaternion.identity, uiCardAnchor.transform);
            return true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curCard)
        {
            float progress = curCard.OnUsing();
            //if(progress>1.0f)
            //{
            //    curCard.OnEndUse();
            //    curCard = null;
            //}
            //else
            //{
            //    uiCard.fillAmount = 1 - progress;
            //}
;           int progressIdx = (int)(progress * listPos.Count);
            if(progressIdx == listPos.Count - 1)
            {
                Vector3 genPos = uiCardAnchor.transform.position + new Vector3(UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), 0);
                if (vfx_useCard) GameObject.Instantiate(vfx_useCard, genPos, Quaternion.identity, uiCardAnchor.transform);
                curCard.OnEndUse();
                curCard = null;
                uiCard.fillAmount = 0;
                srBase.sprite = null;
            }
            else
            {
                if (lastIdx != progressIdx)
                {
                    lastIdx = progressIdx;
                    uiCard.transform.localPosition = listPos[progressIdx];
                    srBase.sprite = listBaseSprites[progressIdx];
                    Vector3 genPos = uiCardAnchor.transform.position + new Vector3(UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), UnityEngine.Random.Range(-vfx_genOffset, vfx_genOffset), 0);
                    if (vfx_useCard) GameObject.Instantiate(vfx_useCard, genPos, Quaternion.identity, uiCardAnchor.transform);
                    if (audioStageChange) SEManager.Instance.PlaySE(audioStageChange);
                }
            }
        }
    }
}
