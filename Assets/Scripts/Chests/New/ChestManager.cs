// Assets/Scripts/Chests/ChestManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudCode;
using UnityEngine;
using System.Collections.Generic; // убедись, что есть
using Unity.Services.CloudSave.Models; // ↑ добавь в using (для Item)

public class ChestManager : MonoBehaviour
{
    [SerializeField] private ChestCatalog catalog;
    [SerializeField] private int slotsCount = 4; // 4 или 5

    public List<ChestSlot> Slots = new();

    const string SaveKey = "chest_slots_v1";

    public event Action<int, ChestSlot> OnSlotChanged; // UI подписывается

    async void Awake()
    {
        // 1) Ждём готовности CloudDataManager (или, если его нет, хотя бы логина UGS)
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.WaitUntilReadyAsync();
        }
        else
        {
            // Фолбэк: ждём, пока сервисы и аутентификация готовы
            while (Unity.Services.Core.UnityServices.State != Unity.Services.Core.ServicesInitializationState.Initialized
                || !Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
                await System.Threading.Tasks.Task.Yield();
        }

        // 2) Теперь можно безопасно работать с Cloud Code/Cloud Save
        await ServerClock.InitializeAsync();   // берём смещение времени (анти-чит) :contentReference[oaicite:6]{index=6}
        await LoadAsync();                     // читаем слоты из Cloud Save
        EnsureSlots();
        TickApply();
    }

    /// <summary>
    /// Если уже есть активный Unlocking, а этот слот Locked — открываем его сразу за гемы.
    /// </summary>
    public async System.Threading.Tasks.Task<bool> TryOpenLockedWithGemsAsync(
        int slotIndex, System.Func<int, bool> trySpendGems)
    {
        var slot = Slots[slotIndex];
        if (slot.State != ChestSlotState.Locked) return false;
        if (!HasActiveUnlock()) return false; // если нет активного таймера → обычный Unlock

        // Используем уже готовую логику мгновенного открытия
        await OpenNowWithGemsAsync(slotIndex, trySpendGems);
        return true;
    }
    
    void Update()
    {
        // локальный тик раз в кадр — UI шевелится без постоянных запросов
        TickApply();
    }

    public bool HasActiveUnlock() => Slots.Any(s => s.State == ChestSlotState.Unlocking);

    public async Task TryStartUnlockAsync(int slotIndex)
    {
        var slot = Slots[slotIndex];
        if (HasActiveUnlock()) { Debug.Log("Another chest is unlocking"); return; }
        if (slot.State != ChestSlotState.Locked) return;

        var def = catalog.ById(slot.ChestId);
        try
        {
            var res = await CloudCodeService.Instance.CallEndpointAsync<StartUnlockDto>(
                "startUnlock",
                new Dictionary<string, object>
                {
                    ["chestId"] = def.Id,
                    ["durationSeconds"] = def.DurationSeconds,
                    ["slotIndex"] = slotIndex
                }
            );

            slot.State = ChestSlotState.Unlocking;
            slot.StartedAtUnix = res.nowUnix;
            slot.FinishAtUnix = res.finishAtUnix;
            slot.RewardSignature = res.signature;

            SaveLocal(slotIndex, slot);
            OnSlotChanged?.Invoke(slotIndex, slot);
        }
        catch (Exception e)
        {
            Debug.LogError($"StartUnlock failed: {e.Message}");
        }
    }

    public void PutChestIntoSlot(int slotIndex, string chestId)
    {
        var slot = Slots[slotIndex];
        if (slot.State != ChestSlotState.Empty) return;
        slot.ChestId = chestId;
        slot.State = ChestSlotState.Locked;
        SaveLocal(slotIndex, slot);
        OnSlotChanged?.Invoke(slotIndex, slot);
    }

    public async Task OpenNowWithGemsAsync(int slotIndex, Func<int,bool> trySpendGems)
    {
        var slot = Slots[slotIndex];
        if (slot.State != ChestSlotState.Unlocking && slot.State != ChestSlotState.Locked) return;

        var def = catalog.ById(slot.ChestId);
        if (!trySpendGems(def.GemOpenNowCost)) return; // списали гемы?
        // симулируем мгновенное завершение (finish = now)
        slot.FinishAtUnix = ServerClock.UtcNowUnix();
        slot.State = ChestSlotState.Ready;
        SaveLocal(slotIndex, slot);
        OnSlotChanged?.Invoke(slotIndex, slot);
    }

    public void ApplyRewardedSkip(int slotIndex, int secondsSkip)
    {
        var slot = Slots[slotIndex];
        if (slot.State != ChestSlotState.Unlocking) return;
        slot.FinishAtUnix = Math.Max(ServerClock.UtcNowUnix(), slot.FinishAtUnix - secondsSkip);
        SaveLocal(slotIndex, slot);
        OnSlotChanged?.Invoke(slotIndex, slot);
    }

    public async Task ClaimAsync(int slotIndex, Action<int,int> grant)
    {
        var slot = Slots[slotIndex];
        if (slot.State != ChestSlotState.Ready && slot.State != ChestSlotState.Unlocking) return;

        // защита: проверяем серверное время
        if (ServerClock.UtcNowUnix() < slot.FinishAtUnix)
        {
            Debug.Log("Too early (anti-time cheat)");
            return;
        }
        var def = catalog.ById(slot.ChestId);

        try
        {
            // Передаем диапазоны на сервер, чтобы сервер решил награду
            var res = await CloudCodeService.Instance.CallEndpointAsync<ClaimDto>(
                "claimChest",
                new Dictionary<string, object>
                {
                    ["slotIndex"] = slot.SlotIndex,
                    ["chestId"] = def.Id,
                    ["finishAtUnix"] = slot.FinishAtUnix,
                    ["signature"] = slot.RewardSignature,
                    ["rewardRanges"] = new Dictionary<string, object>
                    {
                        ["coins"] = new int[] { def.CoinsRange.x, def.CoinsRange.y },
                        ["cards"] = new int[] { def.CardsRange.x, def.CardsRange.y }
                    }
                }
            );


            // Выдаём награду
            grant(res.coins, res.cards);

            // Очищаем слот
            Slots[slotIndex] = new ChestSlot { SlotIndex = slotIndex, State = ChestSlotState.Empty };
            SaveLocal(slotIndex, Slots[slotIndex]);
            OnSlotChanged?.Invoke(slotIndex, Slots[slotIndex]);
        }
        catch (Exception e)
        {
            Debug.LogError($"Claim failed: {e.Message}");
        }
    }

    private void TickApply()
    {
        bool changed = false;
        var now = ServerClock.UtcNowUnix();

        foreach (var slot in Slots)
        {
            if (slot.State == ChestSlotState.Unlocking && now >= slot.FinishAtUnix)
            {
                slot.State = ChestSlotState.Ready;
                changed = true;
                OnSlotChanged?.Invoke(slot.SlotIndex, slot);
            }
        }
        if (changed) _ = SaveAllAsync();
    }

    private void EnsureSlots()
    {
        for (int i = 0; i < slotsCount; i++)
            if (Slots.All(s => s.SlotIndex != i))
                Slots.Add(new ChestSlot { SlotIndex = i, State = ChestSlotState.Empty });
    }

    private async Task LoadAsync()
    {
        try
        {
            

            var dict = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { SaveKey }
            );

            if (dict.TryGetValue(SaveKey, out var item))
            {
                var json = item.Value.GetAs<string>(); // <— тянем строку
                Slots = string.IsNullOrEmpty(json) ? new List<ChestSlot>()
                                                : JsonUtility.FromJson<Wrapper>(json).List;
            }
            else Slots = new List<ChestSlot>();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"CloudSave Load failed: {e.Message}");
            Slots = new List<ChestSlot>();
        }
    }

    private async Task SaveAllAsync()
    {
        try
        {
            var json = JsonUtility.ToJson(new Wrapper { List = Slots });
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { [SaveKey] = json });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"CloudSave SaveAll failed: {e.Message}");
        }
    }

    private void SaveLocal(int slotIndex, ChestSlot slot)
    {
        // локально меняем в списке, а полную запись шлём пачкой
        var idx = Slots.FindIndex(s => s.SlotIndex == slotIndex);
        if (idx >= 0) Slots[idx] = slot;
        _ = SaveAllAsync();
    }

    // Кладёт сундук в первый пустой слот. Вернёт индекс или -1.
    public int TryPutIntoFirstEmptySlot(string chestId)
    {
        for (int i = 0; i < Slots.Count; i++)
            if (Slots[i].State == ChestSlotState.Empty)
            {
                PutChestIntoSlot(i, chestId); // сохранит Cloud Save и вызовет OnSlotChanged
                return i;
            }
        Debug.Log("Нет свободных слотов");
        return -1;
    }



    [Serializable] private class Wrapper { public List<ChestSlot> List; }

    [Serializable] public struct StartUnlockDto { public long nowUnix; public long finishAtUnix; public string signature; }
    [Serializable] public struct ClaimDto { public int coins; public int cards; }
}
