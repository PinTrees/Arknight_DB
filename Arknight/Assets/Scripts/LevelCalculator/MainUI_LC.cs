using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UI;
public class MainUI_LC : MonoBehaviour
{
    CanvasScaler canvasScreen;
    RectTransform rect;
    public void setCanvasScale()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
    }
    public void TR_GOHome()
    {
        SceneManager.LoadScene("Main");
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
