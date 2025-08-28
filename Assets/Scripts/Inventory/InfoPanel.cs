using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoPanel : MonoBehaviour
{
    public GameObject Info_Panel;

    Inventory Inventory;
    LockDice LockDice;
    Class Class;
    Cards Cards;

    GameObject DiceField;
    string diceName;
    int diamondPrice;

    void Start()
    {
        Info_Panel = Info_Panel ?? gameObject; // на всякий
        if (Info_Panel != null) Info_Panel.SetActive(false);

        Inventory = FindObjectOfType<Inventory>();
        Cards     = FindObjectOfType<Cards>();
        LockDice  = FindObjectOfType<LockDice>();
        Class     = FindObjectOfType<Class>();
    }

    void Update()
    {
        if (Class != null && Class._isUpgarded)
        {
            UpdateCardBarInfo();
            Class._isUpgarded = false;
        }
    }

    // ---------- SAFE HELPERS ----------
    GameObject FindChild(Transform root, string name)
    {
        if (root == null) return null;
        for (int i = 0; i < root.childCount; i++)
        {
            var ch = root.GetChild(i);
            if (ch.name == name) return ch.gameObject;
        }
        return null;
    }

    // И немного усилим поиск внутри самого InfoPanel (если хочешь):
public GameObject GetFromInfoPanel(string findingName)
{
    var root = transform.GetChild(0).GetChild(0); // обычно Content
    var t = FindDescendantByName(root, findingName);
    if (t == null)
        Debug.LogError($"[InfoPanel] '{findingName}' not found under '{root.name}'. Children: {DumpChildren(root, 2)}");
    return t?.gameObject;
}
// Рекурсивный поиск потомка по имени (ищет на любую глубину)
Transform FindDescendantByName(Transform root, string name)
{
    if (root == null) return null;
    if (root.name == name) return root;
    for (int i = 0; i < root.childCount; i++)
    {
        var res = FindDescendantByName(root.GetChild(i), name);
        if (res != null) return res;
    }
    return null;
}

// Поиск ближайшего предка, имя которого начинается с префикса
Transform FindAncestorWithPrefix(Transform t, string prefix)
{
    while (t != null && !t.name.StartsWith(prefix)) t = t.parent;
    return t;
}

// Для отладки: выводит имена детей до depth уровней
string DumpChildren(Transform root, int depth)
{
    if (root == null) return "<null>";
    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    void Recur(Transform x, int d)
    {
        if (x == null || d < 0) return;
        sb.Append(x.name);
        if (d >= 0) sb.Append(" { ");
        for (int i = 0; i < x.childCount; i++)
        {
            Recur(x.GetChild(i), d - 1);
            if (i < x.childCount - 1) sb.Append(", ");
        }
        if (d >= 0) sb.Append(" }");
    }
    Recur(root, depth);
    return sb.ToString();
}
    GameObject ResolveDiceField()
{
    var opened  = DataSave.GetString("InfoPanelOpened", "");
    var current = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;

    if (opened == "DragSlot")
    {
        var go = GameObject.Find(DragSlot.ParentName);
        Debug.Log($"[InfoPanel] Resolve DiceField by DragSlot: {go?.name}");
        return go;
    }

    // Надёжно: поднимаемся к предку "DiceField_*"
    var holder = FindAncestorWithPrefix(current?.transform, "DiceField_");
    if (holder == null)
    {
        Debug.LogWarning($"[InfoPanel] DiceField ancestor not found from '{current?.name ?? "null"}'. Fallback to parent.");
        holder = current?.transform?.parent;
    }
    Debug.Log($"[InfoPanel] Resolved DiceField: {holder?.name ?? "null"}");
    return holder?.gameObject;
}

    public GameObject GetFromDiceField(string findingName)
{
    if (DiceField == null) DiceField = ResolveDiceField();
    var t = FindDescendantByName(DiceField?.transform, findingName);
    if (t == null)
    {
        Debug.LogError($"[InfoPanel] '{findingName}' not found under '{DiceField?.name ?? "null"}'. Children: {DumpChildren(DiceField?.transform, 2)}");
        return null;
    }
    return t.gameObject;
}

public GameObject GetFromCurrentDiceField(string findingName)
{
    var t = FindDescendantByName(DiceField?.transform, findingName);
    if (t == null)
        Debug.LogError($"[InfoPanel] '{findingName}' not found under current '{DiceField?.name ?? "null"}'. Children: {DumpChildren(DiceField?.transform, 2)}");
    return t?.gameObject;
}

    GameObject GetSlotChild(string slotName) // вернуть дочерний "Count" внутри слота
    {
        var slot = GetFromInfoPanel(slotName);
        if (slot == null) return null;
        var count = FindChild(slot.transform, "Count");
        if (count == null) Debug.LogError($"[InfoPanel] 'Count' not found in slot '{slotName}'");
        return count;
    }

    void ResizeCardBar(GameObject CardBar) // popoxel clone exac cardbari chapery
    {
        if (CardBar == null) return;
        var rt = CardBar.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.localScale = Vector3.one; // cardbari scale
        rt.sizeDelta = new Vector2(230, 50); // cardbari chapery x,y
        rt.anchoredPosition = new Vector2(-265, 145); // cardbari position
        var txt = CardBar.transform.GetChild(1)?.GetComponent<Text>();
        if (txt) txt.fontSize = 37; // CardsText i fonti chaper
        var arr = CardBar.transform.GetChild(2)?.GetComponent<RectTransform>();
        if (arr) { arr.sizeDelta = new Vector2(60, 70); arr.anchoredPosition = new Vector2(-100, 3); } // UpgradeArrow
    }

    public void UpdateCardBarInfo()
    {
        var prev = GameObject.Find("CardBar(Clone)");
        if (prev) Destroy(prev);

        var powerInfo = GetFromCurrentDiceField("PowerInfo");
        var anchor    = GetFromInfoPanel("DiceClass_Text");
        if (powerInfo == null || anchor == null) return;

        Transform prefab = (powerInfo.transform.childCount > 1) ? powerInfo.transform.GetChild(1) : null;
        if (prefab == null) { Debug.LogError("[InfoPanel] PowerInfo child[1] not found"); return; }

        var cardBar = Instantiate(prefab, anchor.transform.position, Quaternion.identity);
        cardBar.SetParent(transform.GetChild(0).GetChild(0), worldPositionStays: false);
        ResizeCardBar(cardBar.gameObject);
    }

    public void UpdateDiceInfoPanelTextes()
    {
        var t = GetSlotChild("Attack_Slot");     if (t) t.GetComponent<Text>().text = "" + DicePlayerPrefs.GetAttack(diceName);
        t = GetSlotChild("ReloadTime_Slot");     if (t) t.GetComponent<Text>().text = "" + DicePlayerPrefs.GetReloadTime(diceName);
        t = GetSlotChild("ShootSpeed_Slot");     if (t) t.GetComponent<Text>().text = "" + DicePlayerPrefs.GetShootSpeed(diceName);
    }

    public void OpenPanel()
    {
        // 1) diceName
        DiceField = ResolveDiceField();
        var nameGO = GetFromDiceField("DiceNameText");
        if (nameGO == null) return;
        var nameText = nameGO.GetComponent<Text>();
        if (nameText == null) { Debug.LogError("[InfoPanel] DiceNameText has no Text"); return; }
        diceName = nameText.text;

        // 2) класс
        int cls = DataSave.GetInt(diceName + "Class", 1);
        var clsGO = GetFromInfoPanel("DiceClass_Text");
        if (clsGO != null)
        {
            var clsText = clsGO.GetComponent<Text>();
            if (clsText) clsText.text = (cls > 14) ? "MAX" : ("Class " + cls);
        }

        // 3) CardBar/статус
        UpdateCardBarInfo();

        // 4) базовые UI-поля
        diamondPrice = DicePlayerPrefs.GetDiamondPrice(diceName);

        var nameLbl = GetFromInfoPanel("Name_Text");
        var typeLbl = GetFromInfoPanel("Type_Text");
        var imgGO   = GetFromInfoPanel("Dice_Image");

        var srcImg  = GetFromDiceField("DiceButton")?.GetComponent<Image>()?.sprite;

        if (nameLbl) nameLbl.GetComponent<Text>().text = diceName;

        // Берём тип/таргет из DataSave, а не из отсутствующего TypeNameText
        var targetStr = DicePlayerPrefs.GetTarget(diceName);
        if (typeLbl) typeLbl.GetComponent<Text>().text = targetStr;
        Debug.Log($"[InfoPanel] Type for '{diceName}' = '{targetStr}' (from DataSave)");

        if (imgGO && srcImg) imgGO.GetComponent<Image>().sprite = srcImg;


        var price = GetSlotChild("DiamondButton");
        if (price) price.GetComponent<Text>().text = "" + diamondPrice;

        // 5) характеристики
        var c = GetSlotChild("Rarity_Slot");     if (c) c.GetComponent<Text>().text = "" + DicePlayerPrefs.GetRarity(diceName);
        c = GetSlotChild("Attack_Slot");         if (c) c.GetComponent<Text>().text = "" + DicePlayerPrefs.GetAttack(diceName);
        c = GetSlotChild("Target_Slot");         if (c) c.GetComponent<Text>().text = "" + DicePlayerPrefs.GetTarget(diceName);
        c = GetSlotChild("ReloadTime_Slot");     if (c) c.GetComponent<Text>().text = "" + DicePlayerPrefs.GetReloadTime(diceName);
        c = GetSlotChild("ShootSpeed_Slot");     if (c) c.GetComponent<Text>().text = "" + DicePlayerPrefs.GetShootSpeed(diceName);

        // 6) Кнопки (логика как у тебя)
        var buttonsRoot = GetFromDiceField("Buttons")?.transform;
        bool hasInInventory = false;
        if (buttonsRoot && buttonsRoot.childCount > 1)
        {
            var btn = buttonsRoot.GetChild(1).GetComponent<Button>();
            if (btn) hasInInventory = (btn.interactable == false);
        }

        bool isUnlocked = LockDice != null && LockDice.DiceIsUnlocked(diceName);
        string rarity   = DicePlayerPrefs.GetRarity(diceName);

        if (hasInInventory)
        {
            IP_ButtonsReposition(false, 0, true, false, false, false); // upgrade
            UpgradeButtonCheck();
        }
        else if (!hasInInventory && isUnlocked)
        {
            IP_ButtonsReposition(true, -170f, true, false, false, false); // use + upgrade
            UpgradeButtonCheck();
        }
        else if (!isUnlocked)
        {
            if (rarity == "Standard")
                IP_ButtonsReposition(false, 0f, false, true,  false, false); // continue
            else if (rarity == "Exclusive")
                IP_ButtonsReposition(false, 0f, false, false, true,  false); // coin
            else if (rarity == "Legendary")
                IP_ButtonsReposition(false, 0f, false, false, false, true);  // diamond
        }

        if (Info_Panel) Info_Panel.SetActive(true);
        if (Inventory)  Inventory.HideButtons_DIP();
    }

    void IP_ButtonsReposition(bool isActive_UseBtn, float x_UpgradeBtn, bool isActive_UpgradeBtn, bool isActive_ContinueBtn, bool isActive_CoinBtn, bool isActive_DiamondBtn)
    {
        var root = Info_Panel.transform.GetChild(0);
        for (int i = 0; i < root.childCount; i++)
        {
            var ch = root.GetChild(i);
            switch (ch.name)
            {
                case "UseButton_Panel":
                    ch.gameObject.SetActive(isActive_UseBtn);
                    break;
                case "UpgradeButton":
                    var rt = ch.GetComponent<RectTransform>();
                    if (rt) rt.anchoredPosition = new Vector2(x_UpgradeBtn, rt.anchoredPosition.y);
                    ch.gameObject.SetActive(isActive_UpgradeBtn);
                    break;
                case "ContinueButton":
                    ch.gameObject.SetActive(isActive_ContinueBtn);
                    break;
                case "CoinButton":
                    ch.gameObject.SetActive(isActive_CoinBtn);
                    break;
                case "DiamondButton":
                    ch.gameObject.SetActive(isActive_DiamondBtn);
                    break;
            }
        }
    }

    public void UpgradeButtonCheck()
    {
        string rarity = DicePlayerPrefs.GetRarity(diceName);
        int cls       = DataSave.GetInt(diceName + "Class", 1);
        int cards     = DataSave.GetInt(diceName + "TotalCards", 0);

        bool interact = false;
        if (cls < 15)
        {
            if (rarity == "Standard")  interact = cards >= Cards.standard [cls - 1];
            if (rarity == "Exclusive") interact = cards >= Cards.exclusive[cls - 3];
            if (rarity == "Legendary") interact = cards >= Cards.legendary[cls - 5];
        }

        var upg = GetFromInfoPanel("UpgradeButton")?.GetComponent<Button>();
        if (upg) upg.interactable = interact;
    }

    public void UseDiceFromPanel()
    {
        var holder = DataSave.GetString($"Dice_{diceName}_pos", "");
        if (string.IsNullOrEmpty(holder))
        {
            Debug.LogError($"[InfoPanel] Dice_{diceName}_pos not set");
            return;
        }
        var thisDiceField = GameObject.Find(holder);
        if (thisDiceField == null) { Debug.LogError($"[InfoPanel] Holder '{holder}' not found"); return; }

        var dip = thisDiceField.GetComponent<DiceInfoPanel>();
        if (dip == null) { Debug.LogError("[InfoPanel] DiceInfoPanel not found on holder"); return; }

        dip.UseDice();
        ClosePanel();
    }

    public void BuyDiceWithCoin()
    {
        if (Coin.Coins >= 500)
        {
            Coin.Coins -= 500;
            DataSave.SetIntCritical($"Dice_{diceName}_isUnlocked", 1);
            ClosePanel();
            if (LockDice) LockDice.CheckDiceBuyed();
        }
        else
        {
            Debug.Log("Coin недостаточно");
        }
    }

    public void BuyDiceWithDiamond()
    {
        if (Diamond.Diamonds >= diamondPrice)
        {
            Diamond.Diamonds -= diamondPrice;
            DataSave.SetIntCritical($"Dice_{diceName}_isUnlocked", 1);
            ClosePanel();
            if (LockDice) LockDice.CheckDiceBuyed();
        }
        else
        {
            Debug.Log($"Нужно ещё {diamondPrice - Diamond.Diamonds} алмазов");
        }
    }

    public void ClosePanel()
    {
        var cardBar = GameObject.Find("CardBar(Clone)");
        if (cardBar) Destroy(cardBar);
        if (Info_Panel) Info_Panel.SetActive(false);
        DataSave.SetString("InfoPanelOpened", "null");
    }
}
