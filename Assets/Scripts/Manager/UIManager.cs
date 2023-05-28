using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] SpriteRenderer srLeftButton;
    [SerializeField] List<Sprite> spLeftButton;
    [SerializeField] SpriteRenderer srLeftJoystick;
    int leftJoystickDirect;
    int leftJoystickButton;
    [SerializeField] List<Sprite> spLeftJoystick;
    [SerializeField] Image imLeftBattery;
    [SerializeField] GameObject vfx_batteryShine;
    [SerializeField] float battery_fillpad;

    [SerializeField] SpriteRenderer srRightButton;
    int rightButtonUp;
    int rightButtonDown;
    [SerializeField] List<Sprite> spRightButton;

    [SerializeField] SpriteRenderer srRightJoystick;
    [SerializeField] List<Sprite> spRightJoystick;
    [SerializeField] GameObject goHealthPointer;
    [SerializeField] float maxRotation;
    [SerializeField] float minRotation;

    [SerializeField] Image imageShield;
    [SerializeField] GameObject goCompassLight;

    Miner miner;


    /// <summary>
    /// state 0 = idle; 1 = pressed
    /// </summary>
    /// <param name="state"></param>
    public void ChangeLeftButtonState(int state)
    {
        srLeftButton.sprite = spLeftButton[state];
    }

    private void UpdateLeftJoystickSprite()
    {
        srLeftJoystick.sprite = spLeftJoystick[leftJoystickDirect * 2 + leftJoystickButton];
    }

    /// <summary>
    /// state 0 = middle; 1 = left;  2 = right
    /// </summary>
    /// <param name="state"></param>
    public void ChangeLeftJoystickState(int state)
    {
        leftJoystickDirect = state;
        UpdateLeftJoystickSprite();
    }
    /// <summary>
    /// state 0 = idle; 1 = pressed
    /// </summary>
    /// <param name="state"></param>
    public void ChangeLeftJoystickButtonState(int state)
    {
        leftJoystickButton = state;
        UpdateLeftJoystickSprite();
    }



    public void SetLeftBatteryValue(float value)
    {
        value = value * (1 - 2 * battery_fillpad) + battery_fillpad;
        imLeftBattery.fillAmount = value;
    }

    public void PlayLeftBatteryShine()
    {
        GameObject.Instantiate(vfx_batteryShine, imLeftBattery.transform.position, Quaternion.identity, imLeftBattery.transform);
    }

    private void UpdateRightButtonSprite()
    {
        srRightButton.sprite = spRightButton[rightButtonUp * 2 + rightButtonDown];
    }

    /// <summary>
    /// state 0 = idle; 1 = pressed
    /// </summary>
    /// <param name="state"></param>
    public void ChangeRightButtonUpState(int state)
    {
        rightButtonUp = state;
        UpdateRightButtonSprite();
    }

    /// <summary>
    /// state 0 = idle; 1 = pressed
    /// </summary>
    /// <param name="state"></param>
    public void ChangeRightButtonDownState(int state)
    {
        rightButtonDown = state;
        UpdateRightButtonSprite();
    }

    /// <summary>
    /// state 0 = middle; 1 = left;  2 = right
    /// </summary>
    /// <param name="state"></param>
    public void ChangeRightJoystickState(int state)
    {
        srRightJoystick.sprite = spRightJoystick[state];
    }

    public void SetHealthPointerValue(float value)
    {
        goHealthPointer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, minRotation + value * (maxRotation - minRotation)));
    }

    public void SetShieldValue(float value)
    {
        imageShield.fillAmount = value;
    }

    public void SetCompassLight(bool work)
    {
        goCompassLight.SetActive(work);
    }

    private void Start()
    {
        miner = MinerManager.Instance.GetMiner();
    }

    private void Update()
    {
        // tmp code
        if (Input.GetAxis("GunnerX") > 0)
            ChangeLeftJoystickState(2);
        else if (Input.GetAxis("GunnerX") < 0)
            ChangeLeftJoystickState(1);
        else
            ChangeLeftJoystickState(0);

        if (Input.GetAxis("MinerX") > 0)
            ChangeRightJoystickState(2);
        else if (Input.GetAxis("MinerX") < 0)
            ChangeRightJoystickState(1);
        else
            ChangeRightJoystickState(0);

        if (Input.GetKeyDown(KeyCode.S))
            ChangeLeftJoystickButtonState(1);
        else if (Input.GetKeyUp(KeyCode.S))
            ChangeLeftJoystickButtonState(0);

        if (Input.GetKeyDown(KeyCode.W))
            ChangeLeftButtonState(1);
        else if(Input.GetKeyUp(KeyCode.W))
            ChangeLeftButtonState(0);

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.K))
            ChangeRightButtonDownState(1);
        else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.K))
            ChangeRightButtonDownState(0);

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.I))
            ChangeRightButtonUpState(1);
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.I))
            ChangeRightButtonUpState(0);

        float scale = miner.GetCurHealthScale();
        SetHealthPointerValue(scale);
    }
}
