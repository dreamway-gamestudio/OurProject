using TMPro;
using UnityEngine;

public class ChestDebugUI : MonoBehaviour
{
    public ChestManager manager;
    public TMP_Text slot0Label;
    public TMP_Text buttonLabel; // текст на кнопке

    void Update()
    {
        var s = manager.Slots[0];
        if (s.State == ChestSlotState.Unlocking)
        {
            long remain = s.FinishAtUnix - ServerClock.UtcNowUnix();
            if (remain < 0) remain = 0;
            slot0Label.text = $"Slot0: Unlocking {remain/3600:D2}:{(remain%3600)/60:D2}:{remain%60:D2}";
            buttonLabel.text = "Open Now"; // во время Unlocking кнопка = мгновенное открытие
        }
        else
        {
            slot0Label.text = $"Slot0: {s.State}";
            buttonLabel.text = s.State switch
            {
                ChestSlotState.Empty => "Put Tier1",
                ChestSlotState.Locked => "Unlock",
                ChestSlotState.Ready => "Claim",
                _ => "..."
            };
        }
    }

    public async void OnSlot0Button()
    {
        var s = manager.Slots[0];
        switch (s.State)
        {
            case ChestSlotState.Empty:
                manager.PutChestIntoSlot(0, "tier1"); // сюда можно подставить нужный Id сундука
                break;

            case ChestSlotState.Locked:
                await manager.TryStartUnlockAsync(0);
                break;

            case ChestSlotState.Unlocking:
                await manager.OpenNowWithGemsAsync(0, cost => { 
                    Debug.Log($"Spend {cost} gems"); 
                    return true; 
                });
                break;

            case ChestSlotState.Ready:
                await manager.ClaimAsync(0, (coins, cards) =>
                {
                    Debug.Log($"REWARD: +{coins} coins, +{cards} cards");
                });
                break;
        }
    }
}
