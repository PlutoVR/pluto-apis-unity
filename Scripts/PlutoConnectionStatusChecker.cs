using Pluto.APIs.Client;
using Pluto.APIs.Model;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlutoConnectionStatusChecker : MonoBehaviour
{
    public PlutoAPIs PlutoAPIs;

    public int AfterLoggedInCheckDelay = 10000;
    public int AfterConnectedCheckDelay = 5000;
    public int AfterNotConnectedCheckDelay = 1000;
    public int AfterErrorCheckDelay = 1000;

    public UnityEvent<User> OnLoggedIn;
    public UnityEvent OnConnected;
    public UnityEvent OnNotConnected;
    public UnityEvent OnError;

    private bool _runChecks = true;

    public bool IsLoggedIn
    {
        get { return _status == PlutoConnectionStatus.LoggedIn; }
    }

    public User UserData
    {
        get;
        private set;
    }

    public enum PlutoConnectionStatus
    {
        NotConnected,
        Connected,
        LoggedIn,
        Error,
        Unknown,
    }

    private PlutoConnectionStatus _status = PlutoConnectionStatus.Unknown;
    public PlutoConnectionStatus Status
    {
        get
        {
            return _status;
        }

        set
        {
            if (_status != value)
            {
                _status = value;

                if (_status == PlutoConnectionStatus.LoggedIn)
                    OnLoggedIn.Invoke(UserData);
                else if (_status == PlutoConnectionStatus.Connected)
                    OnConnected.Invoke();
                else if (_status == PlutoConnectionStatus.NotConnected)
                    OnNotConnected.Invoke();
                else
                    OnError.Invoke();
            }
        }
    }

    void OnDestroy()
    {
        _runChecks = false;
    }

    void Start()
    {
        CheckConnectionStatus();
    }

    private async void CheckConnectionStatus()
    {
        while (_runChecks)
        {
            Status = await GetConnectionStatus();

            if (Status == PlutoConnectionStatus.LoggedIn)
                await Task.Delay(AfterLoggedInCheckDelay);
            else if (Status == PlutoConnectionStatus.Connected)
                await Task.Delay(AfterConnectedCheckDelay);
            else if (Status == PlutoConnectionStatus.NotConnected)
                await Task.Delay(AfterNotConnectedCheckDelay);
            else
                await Task.Delay(AfterErrorCheckDelay);
        }
    }

    private async Task<PlutoConnectionStatus> GetConnectionStatus()
    {
        try
        {
            UserData = await PlutoAPIs.Client.UserGetAsync();
            return PlutoConnectionStatus.LoggedIn;
        }
        catch (ApiException e)
        {
            if (e.ErrorCode == 0)
            {
                return PlutoConnectionStatus.NotConnected;
            }
            else if (e.ErrorCode == 404)
            {
                return PlutoConnectionStatus.Connected;
            }
            else
            {
                return PlutoConnectionStatus.Error;
            }
        }
    }
}
