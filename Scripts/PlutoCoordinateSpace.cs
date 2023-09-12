using Pluto.APIs.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlutoCoordinateSpace : MonoBehaviour
{
    public PlutoAPIs PlutoAPIs;
    public GameObject RotationPivot;
    public GameObject PositionPivot;

    public Transform Anchor
    {
        get { return PositionPivot.transform; }
    }

    void Start()
    {
        KeepCoordinateSpaceUpdated();
    }

    private async void KeepCoordinateSpaceUpdated()
    {
        while (true)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

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
            catch (ApiException e)
            {
                await Task.Delay(10000);
            }
        }
    }


}
