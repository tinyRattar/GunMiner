using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    [SerializeField] private List<GameObject> listBtns;
    [SerializeField] private GameObject returnNavi;
    bool isEnable = false;
    int idxCurrentSelect = 0;

    float audioValue;
    [SerializeField] float audioValue_step;
    int idxResolution; // 0:1080p 1:2K 2:720p
    List<(int, int)> listResolution;
    bool isFullscreen;

    [SerializeField] Image imgAudio;

    [SerializeField] Image imgResolution;
    [SerializeField] List<Sprite> spResolution;
    [SerializeField] Image imgFullscreen;
    [SerializeField] List<Sprite> spFullscreen;

    [SerializeField] Text txtInfo;
    [SerializeField] List<string> listInfo;

    [SerializeField] AudioClip audioButton;

    [SerializeField] GameObject goHint;

    [Header("删除存档")]
    [SerializeField] GameObject goDeleteHint;
    [SerializeField] bool canDeleteSaveFile;
    [SerializeField] float timeDeleteSaveFile;
    [SerializeField] float timerDeleteSaveFile = 0.0f;
    [SerializeField] Image imageDeleteHint;
    [SerializeField] Text txtDeleteHint;
    [SerializeField] string strDeleted;

    [Header("单人模式")]
    [SerializeField] bool canSwitchSoloMode;
    [SerializeField] Text txtSoloMode;
    [SerializeField] string strSoloModeOn;
    [SerializeField] string strSoloModeOff;
    [SerializeField] Color txtColorOn;
    [SerializeField] Color txtColorOff;

    // tmp UI control
    [SerializeField] Text txtAudio;
    [SerializeField] Text txtResolution;
    [SerializeField] Text txtFullscreen;

    private enum SettingMenuState
    {
        audio = 0,
        resolution = 1,
        fullscreen = 2,
        restart = 3,
        home = 4,
        exit = 5
    }

    private enum ResolutionMode
    {
        r_1080p = 0,
        r_2K = 1,
        r_720p = 2
    }

    public void SetEnable(bool flag)
    {
        if (flag)
        {
            isEnable = true;
            eventSystem.SetSelectedGameObject(listBtns[0]);
            idxCurrentSelect = 0;
        }
        else
        {
            isEnable = false;
            eventSystem.SetSelectedGameObject(returnNavi);
        }
        if(goHint) goHint.SetActive(isEnable);
        if (goDeleteHint) goDeleteHint.SetActive(isEnable);
        this.gameObject.SetActive(isEnable);
    }

    public bool GetEnable()
    {
        return isEnable;
    }

    public void SwitchEnable()
    {
        isEnable = !isEnable;
        SetEnable(isEnable);
    }

    public void AdjustAudioValue(float value, bool loop = false)
    {
        audioValue = audioValue + value;
        if (audioValue > 1.0f) audioValue = 0;
        audioValue = Mathf.Clamp01(audioValue);
        if (audioButton) SEManager.Instance.PlaySE(audioButton);
        SaveAudioValue();
    }
    public void AdjustResolution(int value)
    {
        idxResolution = (listResolution.Count + idxResolution + value) % listResolution.Count;
        if (audioButton) SEManager.Instance.PlaySE(audioButton);
        RefreshScreen();
    }

    public void AdjustFullscreen()
    {
        isFullscreen = !isFullscreen;
        if (audioButton) SEManager.Instance.PlaySE(audioButton);
        RefreshScreen();
    }

    public void RefreshScreen(bool save=true)
    {
        int height = listResolution[idxResolution].Item2;
        int width = listResolution[idxResolution].Item1;
        Screen.SetResolution(width, height, isFullscreen);
        if (save)
        {
            PlayerPrefs.SetInt("resolutionMode", idxResolution);
            PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        }
    }

    private void SaveAudioValue()
    {
        SEManager.Instance.SetVolume(audioValue);
        SEManager.Instance.SetVolumeBGM(audioValue);
        PlayerPrefs.SetFloat("audioValue", audioValue);
    }

    private void Awake()
    {
        listResolution = new List<(int, int)>();
        listResolution.Add((1920, 1080));
        listResolution.Add((3840, 2160));
        listResolution.Add((1280, 720));

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UpdateUI()
    {
        txtAudio.text = ((int)(audioValue * 10 + 0.5) * 10).ToString();
        int height = listResolution[idxResolution].Item2;
        int width = listResolution[idxResolution].Item1;
        txtResolution.text = width.ToString() + " * " + height.ToString();
        txtFullscreen.text = isFullscreen ? "Fullscreen" : "Window";

        imgAudio.fillAmount = audioValue;
        imgResolution.sprite = spResolution[idxResolution];
        imgFullscreen.sprite = spFullscreen[isFullscreen ? 0 : 1];
    }
    // Start is called before the first frame update
    void Start()
    {
        audioValue = PlayerPrefs.GetFloat("audioValue", 1.0f);
        idxResolution = PlayerPrefs.GetInt("resolutionMode", 0);
        isFullscreen = PlayerPrefs.GetInt("fullscreen", 0) > 0;

        if (txtSoloMode)
        {
            if (InputManager.Instance.GetAutoPilotState())
            {
                txtSoloMode.text = strSoloModeOn;
                txtSoloMode.color = txtColorOn;
            }
            else
            {
                txtSoloMode.text = strSoloModeOff;
                txtSoloMode.color = txtColorOff;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (audioButton) SEManager.Instance.PlaySE(audioButton);
            }
            //if (Input.GetKeyDown(KeyCode.Escape))
            //    SwitchEnable();
            //else
            //{
            GameObject curSelected = eventSystem.currentSelectedGameObject;
            if (curSelected == null)
            {
                idxCurrentSelect = 0;
                eventSystem.SetSelectedGameObject(listBtns[idxCurrentSelect]);
            }
            else
            {
                for (int i = 0; i < listBtns.Count; i++)
                {
                    if (curSelected == listBtns[i])
                    {
                        idxCurrentSelect = i;
                        break;
                    }
                    if (i == listBtns.Count - 1)
                    {
                        idxCurrentSelect = 0;
                        eventSystem.SetSelectedGameObject(listBtns[idxCurrentSelect]);
                    }
                }
            }
            txtInfo.text = listInfo[idxCurrentSelect];
            switch (idxCurrentSelect)
            {
                case (int)SettingMenuState.audio:
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.I))
                        AdjustAudioValue(audioValue_step);
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.K))
                        AdjustAudioValue(-audioValue_step);
                    else if (Input.GetKeyDown(KeyCode.Space))
                        AdjustAudioValue(audioValue_step, true);
                    break;
                case (int)SettingMenuState.resolution:
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.I))
                        AdjustResolution(1);
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.K))
                        AdjustResolution(-1);
                    else if (Input.GetKeyDown(KeyCode.Space))
                        AdjustResolution(1);
                    break;
                case (int)SettingMenuState.fullscreen:
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.K))
                        AdjustFullscreen();
                    else if (Input.GetKeyDown(KeyCode.Space))
                        AdjustFullscreen();
                    break;
                case (int)SettingMenuState.restart:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        MainManager.Instance.Restart();
                    }
                    break;
                case (int)SettingMenuState.home:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        MainManager.Instance.ReturnHome();
                    }
                    break;
                case (int)SettingMenuState.exit:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        MainManager.Instance.ExitGame();
                    }
                    break;
                default:
                    break;
            }

            if (canDeleteSaveFile)
            {
                if (Input.GetKey(KeyCode.F12))
                {
                    timerDeleteSaveFile += Time.unscaledDeltaTime;
                    if (timerDeleteSaveFile > timeDeleteSaveFile)
                    {
                        canDeleteSaveFile = false;
                        txtDeleteHint.text = strDeleted;
                        SaveManager.Instance.DeleteFile();
                    }
                }
                else
                {
                    timerDeleteSaveFile -= Time.unscaledDeltaTime;
                }
                timerDeleteSaveFile = Mathf.Clamp(timerDeleteSaveFile, 0, timeDeleteSaveFile);
                imageDeleteHint.fillAmount = timerDeleteSaveFile / timeDeleteSaveFile;
            }

            if (canSwitchSoloMode)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    bool ret = InputManager.Instance.SwitchAutoPilot();
                    if (ret)
                    {
                        txtSoloMode.text = strSoloModeOn;
                        txtSoloMode.color = txtColorOn;
                    }
                    else
                    {
                        txtSoloMode.text = strSoloModeOff;
                        txtSoloMode.color = txtColorOff;
                    }
                }
            }


            UpdateUI();
            //}
        }
        
    }
}
