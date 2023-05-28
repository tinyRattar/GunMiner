using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSeaPlant : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] List<Sprite> listSprites;

    public void RandomSprite()
    {
        int idx = UnityEngine.Random.Range(0, listSprites.Count);
        sr.sprite = listSprites[idx];
    }
}
