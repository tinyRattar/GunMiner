using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchieveMark : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Text txtNickname;

    public void Init(Sprite sprite, string nickname)
    {
        txtNickname.text = nickname;
        image.sprite = sprite;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
