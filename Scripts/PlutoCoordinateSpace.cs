using Pluto.APIs.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlutoCoordinateSpace : MonoBehaviour
{
    public PlutoAPIs PlutoAPIs;
    public GameObject RotationPivot;
    public GameObject PositionPivot;
    private bool isQuitting = false;

    public Transform Anchor
    {
        get { return PositionPivot.transform; }
    }

    void Start()
    {
        KeepCoordinateSpaceUpdated();
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private async void KeepCoordinateSpaceUpdated()
    {
        while (true)
        {
#if UNITY_2022_1_OR_NEWER
            // This is a preferred method for handling async cancelation, but this
            // token is not provided by the system until Untiy 2022
            if (destroyCancellationToken.IsCancellationRequested)
                return;
#else
            if (isQuitting) return;
#endif

            try
            {
                var conversationData = await PlutoAPIs.Client.ConversationGetAsync();

                var quaternion = conversationData.Anchor.Quaternion;
                var position = conversationData.Anchor.Position;
                RotationPivot.transform.localRotation = new Quaternion(Convert.ToSingle(quaternion.X), Convert.ToSingle(quaternion.Y), Convert.ToSingle(-quaternion.Z), Convert.ToSingle(quaternion.W));
                PositionPivot.transform.localPosition = new Vector3(Convert.ToSingle(-position.X), Convert.ToSingle(-position.Y), Convert.ToSingle(position.Z));

                // Note: to use the following log, add the Json package through the package manager
                // You can add by name: com.unity.nuget.newtonsoft-json
                //
                // Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(conversationData));

                await Task.Delay(1000);
            }
            // Can add catch parameter with type ApiException here
            catch
            {
                await Task.Delay(10000);
            }
        }
    }


}
