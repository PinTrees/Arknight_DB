using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AESWithJava.Con;
public class SignIn_UIManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject loginUI;
    public GameObject selectMenu;
    public GameObject nicknameMenu;
    public GameObject pincodeMenu;
    public GameObject connectMenu;
    public GameObject resultMenu;
    public GameObject errorMenu1;
    public GameObject errorMenu2;
    public GameObject errorMenu3;   // pin & usercode lenth error
    public GameObject errorMenu4;   // signin failed error
    public GameObject errorMenu5;   // internet error;
    public GameObject mypageBtn;

    [Header("usercache panel components")]
    public GameObject usercache_panel;
    public Text user_uid;
    public Text user_pin;

    [Header("NickName Menu")]
    public InputField nickNameI;

    [Header("Pin Menu Components")]
    public Text userCode;
    public InputField pinCodeI;

    [Header("Result Menu Components")]
    public Text usercodeR;
    public Text pincodeR;

    [Header("Pin Menu Components")]
    public InputField connect_usercodeI;
    public InputField connect_pincodeI;

    public Text nicknameTxt;

    private void Start()
    {
        ClearMenu();

        if (FirebaseDataBase.instance != null)
            if (FirebaseDataBase.instance.GetCurrentUser() != null)
            {
                nicknameTxt.text = "Dr." + FirebaseDataBase.instance.GetCurrentUser().userName;
                mypageBtn.SetActive(true);
            }
        loginUI.SetActive(false);
    }

    void ClearMenu()
    {
        selectMenu.SetActive(false);
        nicknameMenu.SetActive(false);
        pincodeMenu.SetActive(false);
        connectMenu.SetActive(false);
        resultMenu.SetActive(false);
        errorMenu1.SetActive(false);
        errorMenu2.SetActive(false);
        errorMenu3.SetActive(false);
        errorMenu4.SetActive(false);
        errorMenu5.SetActive(false);

        usercache_panel.SetActive(false);

        mypageBtn.SetActive(false);
    }
    // UI Start ===============================================================
    public void StartUI_Login()
    {
        ClearMenu();
        // sign up
        if (XML.This.GetUserData(FirebaseDataBase.instance.GetUserCache()).Equals(false))
        {
            loginUI.SetActive(true);
            selectMenu.SetActive(true);
        }
        // sign in
        else
        {
            if (!FirebaseDataBase.instance.GetUID().Equals(string.Empty))
            {
                Debug.Log("[auth] already logged in");
                mypageBtn.SetActive(true);
                return;
            }
            else
            {
                // user save key Decrypt
                StartCoroutine(SignIn());
            }
        }
    }
    public void trCopyUserCache()
    {
        UniClipboard.SetText("uid:" + user_uid.text + "      pin:" + user_pin.text); //클립보드에 텍스트 복사

        Log_Manager.instance.Add_Log("복사되었습니다.");
    }
    public void trUserCacheUI()
    {
        user_uid.text = FirebaseDataBase.instance.get_userCache_uid();
        user_pin.text = FirebaseDataBase.instance.get_userCache_pin();
        usercache_panel.SetActive(true);
    }
    public void TR_NextStep_Connect(int index)
    {
        if (index.Equals(0))
        {
            ClearMenu();
            connectMenu.SetActive(true);
            connect_usercodeI.text = string.Empty;
            connect_pincodeI.text = string.Empty;
        }
        else if (index.Equals(1))
        {
            if (connect_usercodeI.text.Length != 12)
            {
                errorMenu3.SetActive(true);
                return;
            }
            else if (connect_pincodeI.text.Length != 6)
            {
                errorMenu3.SetActive(true);
                return;
            }
            StartCoroutine(SignInConnect(connect_usercodeI.text, connect_pincodeI.text));
        }
    }
    public void TRNextStep(int index)
    {
        if (index.Equals(0))
        {
            ClearMenu();
            nicknameMenu.SetActive(true);
            pinCodeI.text = string.Empty;
        }
        else if (index.Equals(1)) //  nickName ok
        {
            if (nickNameI.text.Length > 10)
                return;

            ClearMenu();
            pincodeMenu.SetActive(true);
            userCode.text = CreateUserCode(12);
        }
        else if (index.Equals(2))    //  pincode ok
        {
            if (pinCodeI.text.Length < 6)
                errorMenu1.SetActive(true);
            else
                StartCoroutine(SignUpAndNext());
        }
        else if (index.Equals(3))    // user create ok
        {
            ClearMenu();
            resultMenu.SetActive(true);
            usercodeR.text = userCode.text;
            pincodeR.text = pinCodeI.text;
            userCode.text = string.Empty;
            pinCodeI.text = string.Empty;
        }
        else if (index.Equals(4))    // user code view ok
        {
            // save user data
            string key1, key2, status;
            key1 = XML.This.GetCache_B("new");
            key2 = XML.This.GetCache_B("k");
            status = "1";
            if (key1.Equals(string.Empty))
            {
                status = "0";
                key1 = XML.This.GetCache_B("now");
                key2 = XML.This.GetCache_B("j");
            }
            string uid = Program.Encrypt(usercodeR.text, Program.Decrypt(key1, key2));
            string pin = Program.Encrypt(pincodeR.text, Program.Decrypt(key1, key2));
            XML.This.SaveUserCache(Files.Use.DocumentsPath("cache"), "user.xml", uid, pin, key1, status);
            usercodeR.text = string.Empty;
            pincodeR.text = string.Empty;

            FirebaseDataBase.instance.GetUserCache().Set(uid, pin, key1, status);
            ClearMenu();
            loginUI.SetActive(false);
        }
    }
    string CreateUserCode(int lenght)
    {
        string input = "abcdefghijklmnopqrstuvwxyz0123456789";
        string code = string.Empty;
        for (int i = 0; i < lenght; i++)
        {
            code += input[Random.Range(0, input.Length)];
        }
        return code;
    }
    IEnumerator SignUpAndNext()
    {
        yield return StartCoroutine(FirebaseDataBase.instance.SignUpAuth(userCode.text, pinCodeI.text, nickNameI.text, errorMenu1));

        if(FirebaseDataBase.instance.GetError().Equals("internet 0"))
        {
            errorMenu5.SetActive(true);
            yield break;
        }

        if (errorMenu1.activeSelf.Equals(false))
        {
            if (FirebaseDataBase.instance.GetCurrentUser() != null)
                nicknameTxt.text = "Dr." + FirebaseDataBase.instance.GetCurrentUser().userName;
            TRNextStep(3);
        }
    }
    IEnumerator SignIn()
    {
        yield return StartCoroutine(FirebaseDataBase.instance.SignInWithSaveFile());

        if (FirebaseDataBase.instance.GetError().Equals("internet 0"))
        {
            errorMenu5.SetActive(true);
            yield break;
        }

        if (FirebaseDataBase.instance.GetCurrentUser() != null)
        {
            nicknameTxt.text = "Dr." + FirebaseDataBase.instance.GetCurrentUser().userName;
            mypageBtn.SetActive(true);
        }
    }
    IEnumerator SignInConnect(string _uid, string _pin)
    {
        yield return StartCoroutine(FirebaseDataBase.instance.SignInWithInput(_uid, _pin));

        if (FirebaseDataBase.instance.GetError().Equals("internet 0"))
        {
            errorMenu5.SetActive(true);
            yield break;
        }

        if (!FirebaseDataBase.instance.GetUID().Equals(string.Empty))
        {
            if (FirebaseDataBase.instance.GetCurrentUser() != null)
                nicknameTxt.text = "Dr." + FirebaseDataBase.instance.GetCurrentUser().userName;

            string key1, key2, status;
            key1 = XML.This.GetCache_B("new");
            key2 = XML.This.GetCache_B("k");
            status = "1";
            if (key1.Equals(string.Empty))
            {
                status = "0";
                key1 = XML.This.GetCache_B("now");
                key2 = XML.This.GetCache_B("j");
            }
            string uid = Program.Encrypt(_uid, Program.Decrypt(key1, key2));
            string pin = Program.Encrypt(_pin, Program.Decrypt(key1, key2));
            XML.This.SaveUserCache(Files.Use.DocumentsPath("cache"), "user.xml", uid, pin, key1, status);

            ClearMenu();
            loginUI.SetActive(false);
        }
        else
        {
            errorMenu4.SetActive(true);
        }
    }
    // =========================================================================
}
