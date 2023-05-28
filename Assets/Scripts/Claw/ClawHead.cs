using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawHead : MonoBehaviour
{
    [SerializeField] Claw claw;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ore")
        {
            Ore ore = collision.GetComponent<Ore>();
            if (ore)
            {
                claw.TryDrag(ore);
            }
        }else if (collision.tag == "entity")
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity)
            {
                claw.OnHitEntity(entity);
            }
        }
        else if (collision.tag == "clawHitable")
        {
            IClawHitable obj = collision.GetComponent<IClawHitable>();
            claw.OnHitObject(obj);
        }
    }
}
