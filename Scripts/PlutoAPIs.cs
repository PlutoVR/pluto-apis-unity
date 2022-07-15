using Pluto.APIs.Api;
using Pluto.APIs.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlutoAPIs : MonoBehaviour
{
    public string Url = "http://localhost:12000/v2";

    public DefaultApi Client
    {
        get;
        private set;
    }
        
    public void Awake()
    {
        Client = new DefaultApi(Url);
    }
}
