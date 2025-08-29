// Assets/Scripts/Chests/ChestModels.cs
using System;
using System.Collections.Generic;

[Serializable]
public enum ChestSlotState { Empty, Locked, Unlocking, Ready }

[Serializable]
public class ChestSlot
{
    public int SlotIndex;           // 0..4
    public string ChestId;          // из каталога
    public ChestSlotState State;
    public long StartedAtUnix;      // серверный unix (сек)
    public long FinishAtUnix;       // серверный unix (сек)
    public string RewardSignature;  // подпись Cloud Code (анти-чит)
}
