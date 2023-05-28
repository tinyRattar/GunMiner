using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : Singleton<DroneManager>
{
    [SerializeField] GameObject pfbDrone;
    [SerializeField] GameObject droneGenPos;
    [SerializeField] GameObject droneParent;

    public void ActivateDrone(float lastTime)
    {
        GameObject go = GameObject.Instantiate(pfbDrone, droneGenPos.transform.position, Quaternion.identity, droneParent.transform);
        Drone drone = go.GetComponent<Drone>();
        drone.Init(lastTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // tmp code
        if (MainManager.Instance.InCheatMode() && Input.GetKeyDown(KeyCode.F2))
        {
            MinerManager.Instance.GetMiner().OnHeal(999);
            MinerManager.Instance.GetMiner().ChangeShield(999);
            ActivateDrone(10.0f);
        }
    }
}
