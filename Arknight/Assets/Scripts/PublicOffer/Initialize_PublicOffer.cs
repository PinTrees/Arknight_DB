using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_PublicOffer : MonoBehaviour
{
    public SecenManager SceneMng;
    public MultiTagIcon_UIManager multiIconUI;
    public MultiTagBtn_UIManager MenuUI;
    private void Start()
    {
        multiIconUI.setCanvasScale();
        multiIconUI.Initialized();
        MenuUI.Initialized();
    }
}
