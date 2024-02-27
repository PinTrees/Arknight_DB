using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireBaseClass;
using UnityEngine.Networking;
using System.Linq;
using UI;
using UnityEngine.UI;

public class CommentList
{
    public Dictionary<string, Comment> data;
    public Dictionary<string, Point> point;
    public CommentList() { data = new Dictionary<string, Comment>();
        point = new Dictionary<string, Point>(); }
    public void AddPoint(string _key, Point data)
    {
        if(!point.ContainsKey(_key))
            point.Add(_key, data);
    }
    public Point GetPoint(string _key)
    {
        if (point.ContainsKey(_key))
            return point[_key];
        else
            return null;
    }
    public bool ContainsPoint(string _key)
    {
        return point.ContainsKey(_key);
    }
}
public class CommentUIManager : MonoBehaviour
{
    public Canvas canvas;

    public Transform viewerTr;
    public Transform bestCommentTr;
    
    public RectTransform bestCommentRect;
    public RectTransform commentRect;

    public ScrollRect viewer;
    public Text userName;
    public InputField info;
    public GameObject loginBtn;
    public GameObject pushBtn;
    public Text commentCount;

    GameObject upBtn;
    GameObject downBtn;

    List<CommentIcon> icons;
    List<CommentIcon> bestCommentIcons;

    GameObject loadUI;
    RawImage background;
    GameObject gobject;

    string curOperator;
    CommentList commentFireBase_best;
    CommentList commentFireBase;
    CommentList commentGitHub;
    CommentList curComments;
    CommentList curComments_cache;

    int unit = 10;
    int index = 0;

    int gitIndex = 0;
    int gitMaxIndex = 0;

    List<string> keyindex;
    // ui priset ===========
    float besthieght = 708;
    public void Initialized(GameObject _loadUI, RawImage _background, GameObject _object)
    {
        loadUI = _loadUI;
        background = _background;
        gobject = _object;

        keyindex = new List<string>();

        commentFireBase_best = new CommentList();
        commentFireBase = new CommentList();
        commentGitHub = new CommentList();
        curComments_cache = new CommentList();

        icons = new List<CommentIcon>();
        for (int i = 0; i < viewerTr.childCount; i++)
        {
            if (i.Equals(0))
                upBtn = viewerTr.GetChild(i).gameObject;
            else if (i.Equals(viewerTr.childCount - 1))
                downBtn = viewerTr.GetChild(i).gameObject;
            else
            {
                CommentIcon tmp = new CommentIcon();
                tmp._this = viewerTr.GetChild(i).gameObject;
                tmp.tr = viewerTr.GetChild(i).GetComponent<RectTransform>();
                tmp.userName = viewerTr.GetChild(i).GetChild(0).GetComponent<Text>();
                tmp.info = viewerTr.GetChild(i).GetChild(1).GetComponent<Text>();
                tmp.date = viewerTr.GetChild(i).GetChild(2).GetComponent<Text>();
                tmp.good = viewerTr.GetChild(i).GetChild(3).GetComponent<Text>();
                tmp.bad = viewerTr.GetChild(i).GetChild(4).GetComponent<Text>();
                tmp.data = null;
                tmp.menu = viewerTr.GetChild(i).GetChild(7).gameObject;
                tmp.goodBtn = viewerTr.GetChild(i).GetChild(7).GetChild(0).GetComponent<Image>();
                tmp.badBtn = viewerTr.GetChild(i).GetChild(7).GetChild(1).GetComponent<Image>();
                icons.Add(tmp);
            }
        }
       
        bestCommentIcons = new List<CommentIcon>();
        for (int i = 0; i < bestCommentTr.childCount; i++)
        {
            CommentIcon tmp = new CommentIcon();
            bestCommentIcons.Add(tmp.Initialize(bestCommentTr.GetChild(i)));
        }

        loginBtn.SetActive(false);
        pushBtn.SetActive(false);
        upBtn.SetActive(false);
        downBtn.SetActive(false);

        float h = Screen.height / (float)Screen.width;
        float hh = (float)Screen.height / (float)1080;
        float rect_c = 970 * (h - 1f);
        //2 = 970
        //2.221 = 1210;
        float y = (commentRect.rect.height - rect_c) / (float)2;
        commentRect.sizeDelta = new Vector2(commentRect.rect.width, rect_c);

        commentRect.anchoredPosition = new Vector2(commentRect.anchoredPosition.x, commentRect.anchoredPosition.y + y);
        canvas.enabled = false;
    }
    public void ClearCommentIcon()
    {
        for(int i = 0; i < icons.Count; i++)
            icons[i]._this.SetActive(false);
        for (int i = 0; i < bestCommentIcons.Count; i++)
            bestCommentIcons[i]._this.SetActive(false);
    }
    public IEnumerator StartUI_Main(string _operator)
    {
        loadUI.SetActive(true);
        upBtn.SetActive(false);
        downBtn.SetActive(false);
        
        keyindex.Clear();
        index = 0;
        gitMaxIndex = 0;
        commentCount.text = "0개";
        curOperator = _operator;
   
        if (FirebaseDataBase.instance.GetCurrentUser() == null)
        {
            pushBtn.SetActive(false);
            loginBtn.SetActive(true);
        }
        else
        {
            userName.text = "Dr. " + FirebaseDataBase.instance.GetCurrentUser().userName;
            pushBtn.SetActive(true);
        }

        if (StatusChecking.instance.InternetNetworkStatus().Equals(false))
        {
            loadUI.SetActive(false);
            ExiteUI_Main();

            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i]._this.SetActive(false);
                icons[i].data = null;
            }
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/status.xml");
        yield return request.SendWebRequest();
        gitMaxIndex = XML.This.GetGitMaxCount(request.downloadHandler.text, curOperator);

        if (!XML.This.GetStatus(request.downloadHandler.text, "ver").Equals(1))
        {
            loadUI.SetActive(false);
            StartCoroutine(LogU.Use.SetLog("해당 버전에서 이용이 제한되었습니다."));
            ExiteUI_Main();
            yield break;
        }
        if (!XML.This.GetStatus(request.downloadHandler.text, "check").Equals(0))
        {
            loadUI.SetActive(false);
            StartCoroutine(LogU.Use.SetLog("데이터베이스 점검중 입니다."));
            ExiteUI_Main();
            yield break;
        }
        if (!XML.This.GetStatus(request.downloadHandler.text, "connect").Equals(1))
        {
            loadUI.SetActive(false);
            StartCoroutine(LogU.Use.SetLog("트래픽 할당량 보호를 위해 일시적으로 기능이 사용중지 됩니다."));
            ExiteUI_Main();
            yield break;
        }

        FirebaseDataBase.instance.operatorName = _operator;
        yield return StartCoroutine(FirebaseDataBase.instance.GetComments(curComments_cache, _operator));
        yield return StartCoroutine(FirebaseDataBase.instance.GetCommentsOrderByBest3(commentFireBase_best));
        //commentFireBase = curComments_cache;
        keyindex.Add("Z");

        Count count = new Count();
        yield return StartCoroutine(FirebaseDataBase.instance.GetCommentCount(count));
        if (count.comment != -1)
            commentCount.text = count.comment.ToString() + "개";

        loadUI.SetActive(false);

        if (curComments_cache.data != null)
            yield return StartCoroutine(RefreshUI_CommentList(0, curComments_cache));
        else
        {
            ClearCommentIcon();
            StartCoroutine(LogU.Use.SetLog("평가글이 없습니다."));
        }

        RefreshUI_Size();

        canvas.enabled = true;
    }
    public void RefreshUI_Size()
    {
        float hieght = besthieght - bestCommentRect.rect.height;
        commentRect.sizeDelta = new Vector2(commentRect.rect.width, commentRect.rect.height + hieght);
        commentRect.anchoredPosition = new Vector2(commentRect.anchoredPosition.x, commentRect.anchoredPosition.y + (hieght * 0.5f));

        besthieght = bestCommentRect.rect.height;
    }
    public IEnumerator RefreshUI_CommentList(int _index, CommentList _setComment, string _type = "")
    {
        // best comment ========================================
        List<string> keyValueB = commentFireBase_best.data.Keys.ToList();
        for (int i = 0; i < bestCommentIcons.Count; i++)
        {
            if (i >= keyValueB.Count)
            {
                bestCommentIcons[i]._this.SetActive(false);
                continue;
            }
            bestCommentIcons[i].Refresh(commentFireBase_best.data[keyValueB[i]], keyValueB[i]);

            if (commentFireBase_best.ContainsPoint(keyValueB[i]))
                bestCommentIcons[i].RefreshPoint(commentFireBase_best.GetPoint(keyValueB[i]));
            else
            {
                StartCoroutine(FirebaseDataBase.instance.get_commentPoint_from_firebase(commentFireBase_best.point, curOperator, keyValueB[i], bestCommentIcons[i]));
            }
        }
        // best comment ========================================
        List<string> key = _setComment.data.Keys.ToList();
        key.Reverse();
        for (int i = 0; i < icons.Count; i++)
        {
            if (i >= _setComment.data.Count)
            {
                icons[i]._this.SetActive(false);
                continue;
            }
            icons[i].Refresh(_setComment.data[key[i]], key[i]);

            if (_setComment.ContainsPoint(key[i]))
                icons[i].RefreshPoint(_setComment.GetPoint(key[i]));
            else
                StartCoroutine(FirebaseDataBase.instance.GetCommentPoint(_setComment.point, curOperator, key[i], icons[i]));
        }

        if (_setComment.data.Count >= 10)
            downBtn.SetActive(true);

        viewer.content.anchoredPosition = new Vector2(viewer.content.position.x, -500);
        yield return null;
    }
   IEnumerator refresh_ui_Comment(string limit)
    {
        loadUI.SetActive(true);
        yield return StartCoroutine(FirebaseDataBase.instance.GetComments(curComments_cache, curOperator, limit));
        loadUI.SetActive(false);

        List<string> key = curComments_cache.data.Keys.ToList();
        key.Reverse();
        for (int i = 0; i < icons.Count; i++)
        {
            if (i >= curComments_cache.data.Count)
            {
                icons[i]._this.SetActive(false);
                continue;
            }
            icons[i].Refresh(curComments_cache.data[key[i]], key[i]);

            if (curComments_cache.ContainsPoint(key[i]))
                icons[i].RefreshPoint(curComments_cache.GetPoint(key[i]));
            else
                StartCoroutine(FirebaseDataBase.instance.GetCommentPoint(curComments_cache.point, curOperator, key[i], icons[i]));
        }
        
        upBtn.SetActive(true);
        if(curComments_cache.data.Count >= 10)
            downBtn.SetActive(true);
        else downBtn.SetActive(false);

        viewer.content.anchoredPosition = new Vector2(viewer.content.position.x, -500);
    }
    public void ExiteUI_Main()
    {
        background.enabled = false;
        gobject.SetActive(true);
        canvas.enabled = false;
    }
    public void TR_Up()
    {
        if (index <= 0)
        {
            keyindex.Clear();
            StartCoroutine(LogU.Use.SetLog("최신 평가글입니다."));
            return;
        }
        index--;
        StartCoroutine(refresh_ui_Comment(keyindex[index]));
    }
    public void TR_Down()
    {
        if (curComments_cache.data.Count >= 10)
        {
            index++;

            List<string> keys = curComments_cache.data.Keys.ToList();
            StartCoroutine(refresh_ui_Comment(keys[0]));
            int a = keyindex.IndexOf(keys[0]);
            if (a < 0) keyindex.Add(keys[0]);
        }
        else 
        {
            StartCoroutine(LogU.Use.SetLog("마지막 평가글 입니다."));
        }
    }
    public void TR_SetPointMenu(Transform set)
    {
        int index = set.GetSiblingIndex() - 1;
        if (FirebaseDataBase.instance.GetCurrentUser() == null)
        {
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스 입니다."));
            return;
        }
        StartCoroutine(GetCommentUser(icons, index));
    }
    public void trui_pointMenu_best(Transform set)
    {
        int index = set.GetSiblingIndex();
        if (FirebaseDataBase.instance.GetCurrentUser() == null)
        {
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스 입니다."));
            return;
        }
        StartCoroutine(GetCommentUser(bestCommentIcons, index));
    }
    public void trui_setPoint_firebase(Transform set)
    {
        int index = set.parent.parent.GetSiblingIndex();
        int type = set.GetSiblingIndex();

        if (bestCommentIcons[index].pointUser != null)
        {
            if (bestCommentIcons[index].pointUser.type != 0)
            {
                StartCoroutine(LogU.Use.SetLog("이미 투표한 글입니다. 투표 결과는 변경할 수 없습니다."));
                return;
            }
        }

        PointUser data = new PointUser();
        if (type.Equals(0))
            data.type = 1;
        else if (type.Equals(1))
            data.type = 2;

        StartCoroutine(SetCommentPoint(bestCommentIcons, index, data));
    }
    public void Web_PushComment()
    {
        StartCoroutine(PushComment());
    }
    public void Web_SignInUser()
    {
        StartCoroutine(SignIn());
    }
    public void Web_SetPoint(Transform set)
    {
        int index = set.parent.parent.GetSiblingIndex() - 1;
        int type = set.GetSiblingIndex();

        if(icons[index].pointUser != null)
        {
            if (icons[index].pointUser.type != 0)
            {
                StartCoroutine(LogU.Use.SetLog("이미 투표한 글입니다. 투표 결과는 변경할 수 없습니다."));
                return;
            }
        }

        PointUser data = new PointUser();
        if (type.Equals(0))
            data.type = 1;
        else if (type.Equals(1))
            data.type = 2;
        
        StartCoroutine(SetCommentPoint(icons, index, data));
    }
    IEnumerator SignIn()
    {
        loginBtn.SetActive(false);
        // user cache data file info
        if (XML.This.GetUserData(FirebaseDataBase.instance.GetUserCache()).Equals(false))
        {
            StartCoroutine(LogU.Use.SetLog("계정을 찾을 수 없습니다. 메인화면에서 계정을 생성해 주세요."));
            loginBtn.SetActive(true);
            yield break;
        }
        // sign in user with save file
        yield return StartCoroutine(FirebaseDataBase.instance.SignInWithSaveFile());
        // internet status 0
        if (FirebaseDataBase.instance.GetError().Equals("internet 0"))
        {
            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            loginBtn.SetActive(true);
            yield break;
        }
        // get login user data
        if (FirebaseDataBase.instance.GetCurrentUser() != null)
        {
            userName.text = "Dr. " + FirebaseDataBase.instance.GetCurrentUser().userName;
            loginBtn.SetActive(false);
            pushBtn.SetActive(true);
        }
    }
    IEnumerator PushComment()
    {
        pushBtn.SetActive(false);
        if (FirebaseDataBase.instance.GetCurrentUser() == null)
        {
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스 입니다. 계정을 생성해 주세요."));
            pushBtn.SetActive(true);
            yield break;
        }
        else if (info.text.Length <= 4)
        {
            StartCoroutine(LogU.Use.SetLog("4글자 이상 작성해 주세요."));
            pushBtn.SetActive(true);
            yield break;
        }

        Comment comment = new Comment();
        comment.info = info.text;
        info.text = null;

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/status.xml");
        yield return request.SendWebRequest();
        if (!XML.This.GetStatus(request.downloadHandler.text, "push").Equals(0))
        {
            StartCoroutine(LogU.Use.SetLog("데이터베이스 쓰기가 제한되었습니다."));
            yield break;
        }

        Count count = new Count();
        yield  return StartCoroutine(FirebaseDataBase.instance.GetCommentCount(count));
        if (count.comment != -1)
        {
            StartCoroutine(FirebaseDataBase.instance.PatchCommentCount(count.comment + 1));
            yield return StartCoroutine(FirebaseDataBase.instance.PushComment(comment));
            yield return StartCoroutine(StartUI_Main(curOperator));
        }
        else
        {
            StartCoroutine(LogU.Use.SetLog("데이터베이스 추가에 실패했습니다."));
        }
        pushBtn.SetActive(true);
    }
    IEnumerator GetCommentUser(List<CommentIcon> icons, int _index)
    {
        loadUI.SetActive(true);
        yield return StartCoroutine(FirebaseDataBase.instance.GetCommentUser(icons[_index], curOperator, icons[_index].key));
        loadUI.SetActive(false);
    }
    IEnumerator SetCommentPoint(List<CommentIcon> icons, int _index, PointUser _data)
    {
        icons[_index].SetActiveMenu(null, false);
        loadUI.SetActive(true);
        Point point;
        Dictionary<string, Point> set = new Dictionary<string, Point>();
        string key = icons[_index].key;

        if(!StatusChecking.instance.InternetNetworkStatus())
        {
            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/status.xml");
        yield return request.SendWebRequest();
        if (!XML.This.GetStatus(request.downloadHandler.text, "push").Equals(0))
        {
            StartCoroutine(LogU.Use.SetLog("데이터베이스 쓰기가 제한되었습니다."));
            yield break;
        }

        StartCoroutine(FirebaseDataBase.instance.PatchCommentUser(_data, curOperator, icons[_index].key));
        yield return StartCoroutine(FirebaseDataBase.instance.GetCommentPoint(set, curOperator, icons[_index].key));
        if (set[key] == null) point = new Point();
        else point = set[key];
        if (_data.type.Equals(1))
            point.good++;
        else if (_data.type.Equals(2))
            point.bad++;
        yield return StartCoroutine(FirebaseDataBase.instance.PatchCommentPoint(point, curOperator, icons[_index].key));
        icons[_index].RefreshPoint(point);
        loadUI.SetActive(false);
    }
}
