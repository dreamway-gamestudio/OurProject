using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestSlotsSimpleUI : MonoBehaviour
{
    [Serializable]
    public class SlotWidgets
    {
        public Button button;
        public TMP_Text title;
        public TMP_Text state;
        public TMP_Text timer;
        public Image icon;      // ← перетащи сюда Image из слота
        public Image frame;     // ← (опц.) рамка/фон для акцента по редкости
    }

    public ChestManager manager;
    public ChestCatalog catalog;
    public SlotWidgets[] slots; // 4 шт.

    void Awake()
    {
        manager.OnSlotChanged += (i, _) => Repaint(i);
        for (int i = 0; i < slots.Length; i++)
        {
            int idx = i;
            slots[i].button.onClick.AddListener(() => OnSlotClick(idx));
        }
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++) Repaint(i);
    }

    async void OnSlotClick(int index)
    {
        var s = manager.Slots[index];
        switch (s.State)
        {
            case ChestSlotState.Locked:
                if (manager.HasActiveUnlock())
                {
                    // есть активный таймер → сразу открываем за гемы
                    var def = catalog.ById(manager.Slots[index].ChestId);
                    int cost = def.GemOpenNowCost;

                    await manager.TryOpenLockedWithGemsAsync(index, gemCost =>
                    {
                        // тут твоя логика списания гемов
                        // пример:
                        if (true)
                        {
                            
                            Debug.Log($"Spend {gemCost} gems to open instantly");
                            return true;
                        }
                        Debug.Log("Not enough gems!");
                        return false;
                    });
                }
                else
                {
                    // нет активного таймера → обычный запуск
                    await manager.TryStartUnlockAsync(index);
                }
                break;
            case ChestSlotState.Unlocking:
                await manager.OpenNowWithGemsAsync(index, cost => { Debug.Log($"Spend {cost} gems"); return true; });
                break;
            case ChestSlotState.Ready:
                await manager.ClaimAsync(index, (coins, cards) => Debug.Log($"REWARD slot {index}: +{coins} coins, +{cards} cards"));
                break;
            case ChestSlotState.Empty:
                Debug.Log("Пусто: положи J/K/L");
                break;
        }
    }

    void Repaint(int i)
    {
        if (i >= manager.Slots.Count) return;
        var s = manager.Slots[i];
        var w = slots[i];
        var def = s.State == ChestSlotState.Empty ? null : catalog.ById(s.ChestId);

        w.title.text = s.State == ChestSlotState.Empty ? "Empty" : (def?.DisplayName ?? s.ChestId);
        w.state.text = s.State.ToString();
        // Для Locked показываем полную длительность (из дефиниции)
        if (s.State == ChestSlotState.Locked && def != null)
        {
            w.timer.text = FormatDuration(def.DurationSeconds);
        }


        // --- ИКОНКА/ЦВЕТ ---
        if (def != null && def.Icon != null)
        {
            if (w.icon)  { w.icon.enabled = true;  w.icon.sprite = def.Icon; }
            if (w.frame) { w.frame.color = def.Accent; }
        }
        else
        {
            if (w.icon)  { w.icon.enabled = false; w.icon.sprite = null; }
            if (w.frame) { w.frame.color = Color.white; }
        }

        // --- ТАЙМЕР ---
        if (s.State == ChestSlotState.Unlocking)
        {
            long remain = Math.Max(0, s.FinishAtUnix - ServerClock.UtcNowUnix());
            w.timer.text = FormatDuration(remain);
        }
        else if (s.State == ChestSlotState.Locked && def != null)
        {
            // Показываем длительность сундука до начала
            w.timer.text = FormatDuration(def.DurationSeconds);
        }
        else
        {
            w.timer.text = "";
        }

    }
    static string FormatDuration(long sec)
    {
        if (sec < 60) return $"{sec}s";                   // 45s
        if (sec < 3600) return $"{sec / 60}m";            // 20m
        long h = sec / 3600;
        long m = (sec % 3600) / 60;
        if (m > 0) return $"{h}h {m}m";                   // 3h 25m
        return $"{h}h";                                   // 2h
    }

    static string ToHMS(long sec) { long h=sec/3600,m=(sec%3600)/60,s=sec%60; return $"{h:D2}:{m:D2}:{s:D2}"; }
}
