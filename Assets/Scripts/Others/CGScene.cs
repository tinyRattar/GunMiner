using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CGScene : MonoBehaviour
{
    [SerializeField] float lifetime;
    float timer = 0.0f;
    AsyncOperation asyncOperation;
    bool waitSpace;
    [SerializeField] float timeSpaceHold;
    float timerSpaceHold;
    [SerializeField] float timeWaitSpace;
    [SerializeField] GameObject goHint;
    [SerializeField] Image imgSpaceHold;
    float timerWaitSpace;


    public void LoadScene()
    {
        asyncOperation.allowSceneActivation = true;
    }
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void Start()
    {
        asyncOperation = SceneManager.LoadSceneAsync(2);
        asyncOperation.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Input.anyKey)
        {
            waitSpace = true;
            timerWaitSpace = timeWaitSpace;
            goHint.SetActive(true);
        }
        if (timerWaitSpace > 0.0f)
        {
            timerWaitSpace -= Time.deltaTime;
            if (timerWaitSpace <= 0.0f)
            {
                waitSpace = false;
                goHint.SetActive(false);
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            timerSpaceHold += Time.deltaTime;
        }
        else
        {
            timerSpaceHold -= Time.deltaTime;
        }
        timerSpaceHold = Mathf.Clamp(timerSpaceHold, 0, timeSpaceHold);
        imgSpaceHold.fillAmount = Mathf.Clamp01(timerSpaceHold / timeSpaceHold);

        if (timer > lifetime || (waitSpace && timerSpaceHold >= timeSpaceHold))
        {
            LoadScene();
        }

    }
}
