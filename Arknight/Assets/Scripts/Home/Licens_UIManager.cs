using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Licens_UIManager : MonoBehaviour
{
    public GameObject UI;
    public void Initialized()
    {
        UI.SetActive(false);
    }
    public void Trigger_SetActive(bool set)
    {
        UI.SetActive(set);
    }
    public void Trigger_Link(string url)
    {
        Application.OpenURL(url);
    }
}
