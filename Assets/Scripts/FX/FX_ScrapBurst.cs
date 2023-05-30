using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ScrapBurst : MonoBehaviour
{
    [SerializeField] List<Sprite> list_scraps;
    [SerializeField] int num;
    [SerializeField] float speedMin;
    [SerializeField] float speedMax;
    [SerializeField] float lifetimeMin;
    [SerializeField] float lifetimeMax;
    [SerializeField] GameObject pfb_scrap;

    [SerializeField] float emitAngleMin = 0.0f;
    [SerializeField] float emitAngleMax = 360.0f;
    //[SerializeField] float particleRotateMin = 0.0f;
    //[SerializeField] float particleRotateMax = 360.0f;
    [SerializeField] float genOffsetLeft = 0.0f;
    [SerializeField] float genOffsetRight = 0.0f;
    [SerializeField] float genOffsetUp = 0.0f;
    [SerializeField] float genOffsetDown = 0.0f;

    [SerializeField] GameObject addOnFx;

    public void StartBurst()
    {
        if (addOnFx) GameObject.Instantiate(addOnFx, this.transform.position, Quaternion.identity);
        for (int i = 0; i < num; i++)
        {
            Vector3 genPos = this.transform.position;
            genPos.x += Random.Range(-genOffsetLeft, genOffsetRight);
            genPos.y += Random.Range(-genOffsetUp, genOffsetDown);
            //float rotateAngle = Random.Range(0, 360.0f);
            float rotateAngle = 0.0f;
            float emitAngle = Random.Range(emitAngleMin, emitAngleMax);
            int idx = Random.Range(0, list_scraps.Count);
            GameObject go = GameObject.Instantiate(pfb_scrap, genPos, Quaternion.Euler(new Vector3(0.0f, 0.0f, rotateAngle)), this.transform);
            FX_ScrapParticle scrap = go.GetComponent<FX_ScrapParticle>();
            scrap.SetSprite(list_scraps[idx]);
            scrap.Init(Quaternion.Euler(new Vector3(0.0f, 0.0f, emitAngle)) * Vector2.up, Random.Range(speedMin, speedMax), Random.Range(lifetimeMin, lifetimeMax));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartBurst();
        Destroy(this.gameObject, lifetimeMax);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
