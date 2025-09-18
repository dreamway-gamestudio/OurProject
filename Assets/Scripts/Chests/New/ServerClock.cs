// Assets/Scripts/Chests/ServerClock.cs
using System;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using UnityEngine;
using System.Collections.Generic; // добавь в using

public static class ServerClock
{
    private static long _offsetSec; // serverNow - deviceUtcNow

    public static async Task InitializeAsync()
    {
        try
        {
            var res = await CloudCodeService.Instance.CallEndpointAsync<ServerNowDto>(
                "serverNow",
                null // или: new Dictionary<string, object>()
            );
            var deviceNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _offsetSec = res.nowUnix - deviceNow;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ServerClock init failed: {e.Message}");
            // если офлайн — держим _offsetSec как есть (последний известный)
        }
    }

    public static long UtcNowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _offsetSec;

    [Serializable] public struct ServerNowDto { public long nowUnix; }
}
