using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
using UnityEngine.UI;
using UI;
using TMPro;
public class Ability_Item
{
    public RectTransform rect;
    public GameObject This;
    public TextMeshProUGUI name;
    public TextMeshProUGUI info;

    public void Clear()
    {
        name.text = string.Empty;
        info.text = string.Empty;
    }
    public int Refresh(string _name, string _info)
    {
        int size = 0;

        name.text = _name;
        info.text = _info;

        if (info.preferredHeight < 60)
            size = 110;
        else if (info.preferredHeight < 110)
            size = 160;
        else if (info.preferredHeight < 160)
            size = 210;
        else if (info.preferredHeight < 210)
            size = 260;
        else if (info.preferredHeight < 260)
            size = 310;

        rect.sizeDelta = new Vector2(rect.rect.width, size);
        return size;
    }

    public void Display(bool set)
    {
        if (!This.activeSelf.Equals(set))
            This.SetActive(set);
    }
    public void Disable()
    {
        This.SetActive(false);
    }
}
public class OpAbility_UIClass : MonoBehaviour
{
    [Header("- UI Transform")]
    public RectTransform abilityViewer;
    public RectTransform abilityPanel;

    [Header("- UI Components")]
    public TextMeshProUGUI specipy;
    public TextMeshProUGUI poten;

    private List<Ability_Item> ablitys;

    public void Initialized()
    {
        ablitys = new List<Ability_Item>();
        for(int i = 0; i < abilityViewer.childCount; i++)
        {
            Ability_Item _new = new Ability_Item();
            _new.This = abilityViewer.GetChild(i).gameObject;
            _new.rect = abilityViewer.GetChild(i).GetComponent<RectTransform>();
            _new.name = abilityViewer.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _new.info = abilityViewer.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            ablitys.Add(_new);
        }
    }
    // UI Corutine Start ========================================
    public IEnumerator UIStart_Main(OperaterClass set)
    {
        specipy.text = set.specificity;

        string poten_tmp = string.Empty;
        for (int i = 0; i < set.potential.Length; i++)
        {
            poten_tmp = string.Format("{0}{1}{2}", poten_tmp, set.potential[i], System.Environment.NewLine);
        }
        poten.text = poten_tmp;

        List<int> size = new List<int>();
        for (int i = 0; i < ablitys.Count; i++)
        {
            if (i >= set.ability.Count)
            {
                ablitys[i].Disable();
            }
            else
            {
                int _size = ablitys[i].Refresh(set.ability[i].name, set.ability[i].info);
                size.Add(_size);
                ablitys[i].Display(true);
            }
        }
        int maxSize = 0;
        for (int i = 0; i < size.Count; i++)
            maxSize += size[i];
        abilityViewer.sizeDelta = new Vector2(abilityViewer.rect.width, maxSize);
        abilityPanel.sizeDelta = new Vector2(abilityPanel.rect.width, maxSize + 100);
        yield return new WaitForEndOfFrame();
    }
    public void UIExit_Main()
    {
        for (int i = 0; i < ablitys.Count; i++)
            ablitys[i].Clear();
    }
    // UI Corutine Exit ========================================
}
