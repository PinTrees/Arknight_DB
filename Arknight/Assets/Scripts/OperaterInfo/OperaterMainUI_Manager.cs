using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UI;
public class OperaterMainUI_Manager : MonoBehaviour
{
    public CanvasScaler canvasScrean;
    public RectTransform rect;

    [Header("- UI Manager")]
    public OperaterInfo_UIManager infoUI_Mng;
    public AudioUI_Op audioUI;
    public InfraUIManager_OP InfraMenu;
    StatusUI_Op statusUI;

    public Canvas iconViewer;
    public void Initialized()
    {
        statusUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<StatusUI_Op>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScrean, rect);
    }
    public void TRStatusUIManager()
    {
        iconViewer.enabled = true;
       InfraMenu.This.enabled = false;
    }
    public void TRInfraUIManager()
    {
       InfraMenu.This.enabled = true;
        iconViewer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (statusUI.activeSelf())
                infoUI_Mng.TR_SetStatusUI(false);
            else if(audioUI.IsActive())
                infoUI_Mng.TR_AudioUI(false);
            else if (infoUI_Mng.GetStatus_CommentUI())
                infoUI_Mng.TR_SetCommentUI(false);
            else if (infoUI_Mng.profileUI.mainCanvas.enabled)
                infoUI_Mng.tr_set_profileCanvas(false);
            else if (infoUI_Mng.This.enabled)
                infoUI_Mng.TR_SetActive(false);
            else
                SceneManager.LoadScene("Main");
        }
    }
}
