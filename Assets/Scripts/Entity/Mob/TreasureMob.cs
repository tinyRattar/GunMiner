using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMob : Mob
{
    [SerializeField] Vector3 moveDirect;
    GameObject genAnchor;
    protected override void Move()
    {
        this.transform.Translate(moveDirect * speed * Time.fixedDeltaTime);
        if (!lookAtTarget)
        {
            Utils.FlipGameObject(topSR, moveDirect.x < 0);
        }
    }

    public void SetMoveDirect(Vector3 direct)
    {
        moveDirect = direct;
    }
    public void SetGenAnchor(GameObject anchor)
    {
        genAnchor = anchor;
    }

    protected override void OnExceedRemoveDistance()
    {
        MobManager.Instance.GenerateTreasureMob(genAnchor);
        base.OnExceedRemoveDistance();
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        BasicBehavior();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSR();
    }
}
