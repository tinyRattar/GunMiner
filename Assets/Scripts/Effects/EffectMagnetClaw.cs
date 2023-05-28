using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMagnetClaw : MonoBehaviour
{
    [SerializeField] List<Collider2D> hitboxs;
    ContactFilter2D targetFilter;
    [SerializeField] LayerMask targetMask;
    [SerializeField] bool useTrigger;

    [Header("ÎüÊÕ")]
    [SerializeField] float absorbSpeed;
    [SerializeField] float minDistance;
    float timerWork;
    [SerializeField] GameObject goAbsorbFX;

    public void ActivateEffcet(float lastTime)
    {
        timerWork = lastTime;
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
            Claw claw = MinerManager.Instance.GetMiner().GetCurClaw();
            goAbsorbFX.SetActive(true);
            this.transform.position = claw.GetClawHeadPos();
            this.transform.rotation = claw.transform.rotation;
            timerWork -= Time.deltaTime;
            foreach (Collider2D hitbox in hitboxs)
            {
                List<Collider2D> list_colliders = new List<Collider2D>();
                Physics2D.OverlapCollider(hitbox, targetFilter, list_colliders);
                foreach (var item in list_colliders)
                {
                    if (item.tag == "ore")
                    {
                        Ore ore = item.GetComponent<Ore>();
                        if (ore)
                        {
                            claw.TryDrag(ore);
                            //Vector3 moveVec = (this.transform.position - item.transform.position);
                            //if (moveVec.magnitude > minDistance)
                            //    item.transform.Translate(moveVec.normalized * absorbSpeed * Time.deltaTime);
                        }
                    }
                }
            }
        }
        else
        {
            goAbsorbFX.SetActive(false);
        }
    }
}
