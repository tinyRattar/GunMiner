using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] Miner miner;
    [SerializeField] bool freezeInput;

    [Header("×Ô¶¯Ãé×¼")]
    [SerializeField] bool canAutoPilot;
    bool autoPilot = false;
    [SerializeField] float detectDistance;
    [SerializeField] float timeAutoPilotPause;
    float timerAutoPilotPause;
    [SerializeField] GameObject goAutoPilotHint;
    //[SerializeField] Image imgAutoPilotPause;
    //[SerializeField] Text txtAutoPilot;
    //[SerializeField] Color txtColorOn;
    //[SerializeField] Color txtColorOff;
    [SerializeField] UIAutoPilotToolKit uiAutoPilot;
    [SerializeField] GameObject goManualToolkit;

    //public bool SetAutoPilot(bool flag)
    //{
    //    if (canAutoPilot)
    //        autoPilot = true;
    //    return autoPilot;
    //}

    public bool SwitchAutoPilot()
    {
        if (canAutoPilot)
            autoPilot = !autoPilot;
        if (autoPilot)
        {
            if (goAutoPilotHint)
            {
                goAutoPilotHint.SetActive(true);
                goManualToolkit.SetActive(false);
            }
        }
        else
        {
            if (goAutoPilotHint)
            {
                goAutoPilotHint.SetActive(false);
                goManualToolkit.SetActive(true);
            }
        }
        return autoPilot;
    }

    public void SetFreezeInput(bool flag)
    {
        freezeInput = flag;
    }

    private void AutoPilot()
    {
        List<Mob> listMobs = MobManager.Instance.GetListMobs();
        Vector3 srcPos = WeaponManager.Instance.GetCurrentWeapon().transform.position;
        float minDistance = 65535.0f;
        Vector3 shootDirect = Vector3.zero;
        foreach (Mob mob in listMobs)
        {
            Vector3 distance = mob.transform.position - srcPos;
            if (detectDistance < distance.magnitude)
            {
                continue;
            }
            if (distance.magnitude < minDistance)
            {
                minDistance = distance.magnitude;
                shootDirect = distance.normalized;
            }
        }
        float tarDegree = Quaternion.FromToRotation(Vector3.up, shootDirect).eulerAngles[2];
        if (tarDegree > 180)
            tarDegree = tarDegree - 360;
        int ret = WeaponManager.Instance.GetCurrentWeapon().SwingToTargetDegree(tarDegree);
        uiAutoPilot.SetState(ret);
    }

    // Start is called before the first frame update
    void Start()
    {
        miner = MinerManager.Instance.GetMiner();
    }

    private void FixedUpdate()
    {
        if (!freezeInput)
        {
            miner.MoveByInput(Input.GetAxis("MinerX"));
            //WeaponManager.Instance.GetCurrentWeapon().SwingByInput(Input.GetAxis("GunnerX"));
            if (canAutoPilot && autoPilot)
            {
                if (Input.GetAxis("GunnerX") != 0)
                {
                    timerAutoPilotPause = timeAutoPilotPause;
                }
                if (timerAutoPilotPause > 0.0f)
                {
                    //txtAutoPilot.color = txtColorOff;
                    timerAutoPilotPause -= Time.fixedDeltaTime;
                    uiAutoPilot.SetIdle();
                    //if (imgAutoPilotPause)
                    //    imgAutoPilotPause.fillAmount = Mathf.Clamp01(1 - timerAutoPilotPause / timeAutoPilotPause);
                    WeaponManager.Instance.GetCurrentWeapon().SwingByInput(Input.GetAxis("GunnerX"));
                }
                else
                {
                    //txtAutoPilot.color = txtColorOn;
                    AutoPilot();
                }
            }
            else
            {
                WeaponManager.Instance.GetCurrentWeapon().SwingByInput(Input.GetAxis("GunnerX"));
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!freezeInput)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                WeaponManager.Instance.GetCurrentWeapon().SteamByInput();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (ItemManager.Instance)
                    ItemManager.Instance.UseCardByInput(0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.I))
            {
                if (ItemManager.Instance)
                    ItemManager.Instance.UseCardByInput(1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.K))
            {
                ClawManager.Instance.GetCurrentClaw().StartExtendByInput();
            }
        }
    }
}
