using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLootAbsorb : MonoBehaviour
{
    [SerializeField] List<Collider2D> hitboxs;
    ContactFilter2D targetFilter;
    [SerializeField] LayerMask targetMask;
    [SerializeField] bool useTrigger;


    [Header("ÎüÊÕ")]
    [SerializeField] float absorbSpeed;
    [SerializeField] GameObject goVisualFX;
    float timerWork;

    public void ActivateEffcet(float lastTime)
    {
        timerWork = lastTime;
        goVisualFX.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        targetFilter = new ContactFilter2D();
        targetFilter.SetLayerMask(targetMask);
        targetFilter.useTriggers = useTrigger;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerWork > 0)
        {
            timerWork -= Time.deltaTime;
            foreach (Collider2D hitbox in hitboxs)
            {
                List<Collider2D> list_colliders = new List<Collider2D>();
                Physics2D.OverlapCollider(hitbox, targetFilter, list_colliders);
                foreach (var item in list_colliders)
                {
                    if (item.tag == "loot")
                    {
                        ItemLoot loot = item.GetComponent<ItemLoot>();
                        if (loot)
                        {
                            Vector3 moveVec = (this.transform.position - item.transform.position).normalized * absorbSpeed * Time.deltaTime;
                            item.transform.Translate(moveVec);
                        }
                    }
                }
            }
            if (timerWork <= 0)
            {
                goVisualFX.SetActive(false);
            }
        }
    }
}
