using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : Singleton<EnvManager>
{
    [SerializeField] GameObject src;
    [SerializeField] Transform genParent;
    [SerializeField] GameObject pfbSeaPlant;
    int leftSeaPlantIdx = 0;
    [SerializeField] float leftBorder;
    [SerializeField] float rightBorder;
    float lastX;
    [SerializeField] float intervalX;
    List<GameObject> listSeaPlantsGo;
    List<EnvSeaPlant> listSeaPlants;


    [Header("ÆøÅÝ")]
    [SerializeField] GameObject pfbBubble;
    [SerializeField] float timeBubbleIntervalMin;
    [SerializeField] float timeBubbleIntervalMax;
    [SerializeField] float bubbleGenLeftX;
    [SerializeField] float bubbleGenRightX;
    float timerBubbleGen;
    [SerializeField] float bubbleGenY;

    private void Start()
    {
        listSeaPlantsGo = new List<GameObject>();
        listSeaPlants = new List<EnvSeaPlant>();
        Vector3 genPosRaw = genParent.transform.position;
        genPosRaw.x = src.transform.position.x;
        for (float curX = leftBorder; curX <= rightBorder; curX += intervalX)
        {
            Vector3 genPos = genPosRaw + new Vector3(curX, 0, 0);
            GameObject go = GameObject.Instantiate(pfbSeaPlant, genPos, Quaternion.identity, genParent);
            EnvSeaPlant envSeaPlant = go.GetComponent<EnvSeaPlant>();
            envSeaPlant.RandomSprite();
            listSeaPlantsGo.Add(go);
            listSeaPlants.Add(envSeaPlant);
        }
        lastX = src.transform.position.x;
    }

    private void Update()
    {
        float curX = src.transform.position.x;
        if (curX > lastX + intervalX)
        {
            rightBorder += intervalX;
            leftBorder += intervalX;
            lastX += intervalX;
            //listSeaPlantsGo[leftSeaPlantIdx].transform.Translate(new Vector3(intervalX * listSeaPlants.Count, 0, 0));
            listSeaPlantsGo[leftSeaPlantIdx].transform.Translate(new Vector3(rightBorder - leftBorder, 0, 0));
            //listSeaPlants[leftSeaPlantIdx].RandomSprite();
            leftSeaPlantIdx += 1;
            if (leftSeaPlantIdx >= listSeaPlants.Count) leftSeaPlantIdx = 0;
        }
        else if(curX < lastX - intervalX)
        {
            rightBorder -= intervalX;
            leftBorder -= intervalX;
            lastX -= intervalX;
            //listSeaPlantsGo[listSeaPlants.Count - 1 - leftSeaPlantIdx].transform.Translate(new Vector3(-intervalX * listSeaPlants.Count, 0, 0));
            leftSeaPlantIdx -= 1;
            if (leftSeaPlantIdx < 0) leftSeaPlantIdx = listSeaPlants.Count - 1;
            listSeaPlantsGo[leftSeaPlantIdx].transform.Translate(new Vector3(-rightBorder + leftBorder, 0, 0));
            //listSeaPlants[leftSeaPlantIdx].RandomSprite();
        }

        if (timerBubbleGen < 0.0f)
        {
            timerBubbleGen = UnityEngine.Random.Range(timeBubbleIntervalMin, timeBubbleIntervalMax);
            Vector3 genPos = src.transform.position;
            genPos.y = bubbleGenY;
            genPos.x += UnityEngine.Random.Range(bubbleGenLeftX, bubbleGenRightX);
            GameObject go = GameObject.Instantiate(pfbBubble, genPos, Quaternion.identity, genParent);
        }
        else
        {
            timerBubbleGen -= Time.deltaTime;
        }
    }
}
