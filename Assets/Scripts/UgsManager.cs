using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class UgsManager : MonoBehaviour
{
    public static UgsManager Instance { get; private set; }

    // Флаг, который показывает, что инициализация успешно завершена.
    public bool IsInitialized { get; private set; } = false;

    async void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Делаем менеджер доступным между сценами

        // Запускаем асинхронную инициализацию
        await InitializeUgs();
    }

    private async Task InitializeUgs()
    {
        try
        {
            // Инициализируем сервисы
            await UnityServices.InitializeAsync();

            // Проверяем, нужно ли входить в систему
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // Входим анонимно
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("UGS Initialized and Signed In Anonymously. Player ID: " + AuthenticationService.Instance.PlayerId);
            }

            // Устанавливаем флаг успешной инициализации
            IsInitialized = true;
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError($"Error initializing UGS: {e.Message}");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Error during authentication: {e.Message}");
        }
    }
}