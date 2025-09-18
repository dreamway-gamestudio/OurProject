// Assets/Scripts/Init/UnityServicesInit.cs
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class UnityServicesInit : MonoBehaviour
{
    async void Awake() { await InitAsync(); }

    private async Task InitAsync()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Unity Services ready");
    }
}
