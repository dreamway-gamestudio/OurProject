// UGSBootstrap.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;

public class UGSBootstrap : MonoBehaviour
{
    [Header("UGS")]
    [SerializeField] string environmentName = "production"; // или "development"
    [SerializeField] string profile = "default";

    async void Awake()
    {
        Application.runInBackground = true;
        await InitAndSignIn();
        await TestCloudSave();
    }

    private async Task InitAndSignIn()
    {
        try
        {
            var opts = new InitializationOptions()
                .SetEnvironmentName(environmentName)
                .SetProfile(profile);

            await UnityServices.InitializeAsync(opts);
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log($"UGS OK | Env={environmentName} | Profile={profile} | PlayerId={AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException e) { Debug.LogError($"Auth error: {e.ErrorCode} | {e.Message}"); }
        catch (RequestFailedException e) { Debug.LogError($"UGS request failed: {e.ErrorCode} | {e.Message}"); }
        catch (Exception e) { Debug.LogError(e); }
    }

    private async Task TestCloudSave()
    {
        try
        {
            // write
            var payload = new Dictionary<string, object> {
            { "first_login_ts", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            { "auth_provider", "anonymous" }
        };
            await CloudSaveService.Instance.Data.ForceSaveAsync(payload);
            Debug.Log("Cloud Save: write OK");

            // read (как строки)
            var keys = new HashSet<string> { "first_login_ts", "auth_provider" };
            var data = await CloudSaveService.Instance.Data.LoadAsync(keys); // Dictionary<string,string>

            long ts = -1;
            if (data.TryGetValue("first_login_ts", out var tsStr))
                long.TryParse(tsStr, System.Globalization.NumberStyles.Integer,
                              System.Globalization.CultureInfo.InvariantCulture, out ts);

            string prov = data.TryGetValue("auth_provider", out var provStr) ? provStr : "?";

            Debug.Log($"Cloud Save: read OK | first_login_ts={ts} | auth_provider={prov}");
        }
        catch (RequestFailedException e) { Debug.LogError($"Cloud Save error: {e.ErrorCode} | {e.Message}"); }
        catch (Exception e) { Debug.LogError(e); }
    }

    [ContextMenu("UGS Re-SignIn (same user)")]
    public async void ReSignIn()
    {
        try
        {
            AuthenticationService.Instance.SignOut();                 // ← синхронный
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Re-Signed | PlayerId={AuthenticationService.Instance.PlayerId}");
        }
        catch (System.Exception e) { Debug.LogError(e); }
    }
}