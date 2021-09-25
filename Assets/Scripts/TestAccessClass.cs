using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAccessClass : MonoBehaviour
{
    private string _guid;
    public string GUID { get => _guid; set => _guid = value; }
    public string g_guid;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        this.SetGUID();
    }

    public void SetGUID()
    {
        this._guid = Guid.NewGuid().ToString();
        this.g_guid = this._guid;
    }
}
