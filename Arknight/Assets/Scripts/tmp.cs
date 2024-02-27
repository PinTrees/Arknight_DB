using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Initialize());
    }
    IEnumerator Initialize()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
        ApplicationChrome.navigationBarState = ApplicationChrome.States.TranslucentOverContent;
        yield return new WaitForSeconds(0.1f);
        ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
    }
    IEnumerator test()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
        yield return new WaitForSeconds(0.1f);
        ApplicationChrome.navigationBarState = ApplicationChrome.States.TranslucentOverContent;
        yield return new WaitForSeconds(0.1f);
        ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
    }
    public void TRAplicationCromTest(int index)
    {
        switch(index)
        {
            case 1:
                ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
                break;
            case 2:
                ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
                break;
            case 3:
                ApplicationChrome.navigationBarState = ApplicationChrome.States.VisibleOverContent;
                break;
            case 4:
                ApplicationChrome.navigationBarState = ApplicationChrome.States.TranslucentOverContent;
                break;
        }

    }
    public void TRAplicationCromColorTest(int index)
    {
        switch(index)
        {
            case 1:
                ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xff444444;
                break;
            case 2:
                ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xffffffff;
                break;
            case 3:
                ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xff000000;
                break;
        }

    }
    public void tr(int index)
    {
        StartCoroutine(test());
    }
}
