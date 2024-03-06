using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonoCache : NetworkBehaviour //������ Update, ��������� ����� OnTick()
{
    public static List<MonoCache> allUpdate = new List<MonoCache>(10001);

    //������ OnEnable � OnDisable � PlayerCOntrol
    public Action Enabled;
    public Action Disabled;

    private void OnEnable()
    {
        //GOEnable();
        allUpdate.Add(this);
        Enabled?.Invoke();
    }
    private void OnDisable()
    {
        //GODisable();
        allUpdate.Remove(this);
        Disabled?.Invoke();
    }
    private void OnDestroy()
    {
        //GODisable();
        allUpdate.Remove(this);
        Disabled?.Invoke();
    }


    public void Tick() => OnTick();
    public virtual void OnTick() { }
}
