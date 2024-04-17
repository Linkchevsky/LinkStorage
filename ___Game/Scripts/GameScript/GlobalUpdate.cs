using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpdate : NetworkBehaviour //вызов всех Update
{
    private void Update()
    {
        for (int i = 0; i < MonoCache.allUpdate.Count; i++)
            MonoCache.allUpdate[i].Tick();
    }

    public static GlobalUpdate Instance;
    private void Awake() //объ€вл€ю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static Action s_energyTick;
    private void Start()
    {
        if (isServer)
            StartCoroutine(ExecuteAfterTime());
    }
    private IEnumerator ExecuteAfterTime()
    {
        while (true)
        {
            CmdExecuteAfterTime();
            yield return new WaitForSeconds(5);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdExecuteAfterTime() => RpcExecuteAfterTime();
    [ClientRpc]
    private void RpcExecuteAfterTime() => s_energyTick?.Invoke();
}
