using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using AESWithJava.Con;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using FullSerializer;
using FireBaseClass;
using UI;
public class UserCache
{
    string uid;
    string pin;
    string key;
    public string status;
    public UserCache() { }
    public UserCache(string _uid, string _pin, string _key, string _status)
    {
        uid = _uid;
        pin = _pin;
        key = _key;
        status = _status;
    }
    public void Set(string _uid, string _pin, string _key, string _status)
    {
        uid = _uid;
        pin = _pin;
        key = _key;
        status = _status;
    }
    public string GetUID() { return uid; }
    public string GetPin() { return pin; }
    public string GetKey() { return key; }
}
public class FirebaseDataBase : MonoBehaviour
{
    public static FirebaseDataBase instance;

    public string operatorName;

    string idToken;
    string localId;
    string error;

    UserCache curUserCache;
    User currUser;

    fsSerializer serializer = new fsSerializer();
    string DataBaseURL = "https://arknights-db.firebaseio.com/users";

    public delegate void GetCommentsCallback(Dictionary<string, Comment> comment);
    public delegate void GetCommentCountCallback(Count count);
    private void Awake()
    {
        instance = this;

        localId = string.Empty;
        error = string.Empty;

        curUserCache = new UserCache();

        DontDestroyOnLoad(this.gameObject);
        string key = string.Empty;
    }
    IEnumerator SignInUser(string _uid, string _pin)
    {
        bool result = false; localId = string.Empty;
        error = string.Empty;

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            error = "internet 0";
            yield break;
        }

        if (Version.Use.cacheXML.Length < 256)
        {
            UnityWebRequest cacheRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/cache.xml");
            cacheRequest.SendWebRequest();
            while (!cacheRequest.isDone)
            {
                yield return new WaitForSeconds(.01f);
            }
            Version.Use.cacheXML = cacheRequest.downloadHandler.text;
        }

        string userData = "{\"email\":\"" + _uid + "@xxx.xxx" + "\",\"password\":\"" + _pin + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" +
            Program.Decrypt(XML.This.GetCache_A("a"), Program.Decrypt(XML.This.GetCache_A("l"), XML.This.GetCache_A("k"))), userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;

            Debug.Log("[auth] email login success");
            result = true;
        })
        .Catch(error =>
        {
            Debug.Log(error);
            result = true;
        });
        yield return new WaitUntil(() => result);
        result = false;

        currUser = null;
        if (localId.Equals(string.Empty))
            yield break;

        RestClient.Get<User>(DataBaseURL + "/" + localId + ".json").Then(response =>
        {
            currUser = response;
            result = true;

            if (response == null)
                Debug.Log("none user data");
        });
        yield return new WaitUntil(() => result);
        yield return null;
    }
    IEnumerator getIdToken(string _uid, string _pin)
    {
        bool result = false; localId = string.Empty;
        error = string.Empty;

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            error = "internet 0";
            yield break;
        }

        if (Version.Use.cacheXML.Length < 256)
        {
            UnityWebRequest cacheRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/cache.xml");
            cacheRequest.SendWebRequest();
            while (!cacheRequest.isDone)
            {
                yield return new WaitForSeconds(.01f);
            }
            Version.Use.cacheXML = cacheRequest.downloadHandler.text;
        }

        string userData = "{\"email\":\"" + _uid + "@xxx.xxx" + "\",\"password\":\"" + _pin + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" +
            Program.Decrypt(XML.This.GetCache_A("a"), Program.Decrypt(XML.This.GetCache_A("l"), XML.This.GetCache_A("k"))), userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            result = true;
        })
        .Catch(error =>
        {
            Debug.Log(error);
            result = true;
        });

        yield return new WaitUntil(() => result);
        result = false;
    }
    // SinIn Start ============================================================
    public IEnumerator SignInWithInput(string _uid, string _pin)
    {
        yield return SignInUser(_uid, _pin);
        yield return null;
    }
    public IEnumerator SignInWithSaveFile()
    {
        string key = string.Empty;
        error = string.Empty;

        if (Version.Use.cacheXML.Length < 256)
        {
            if (!StatusChecking.instance.InternetNetworkStatus())
            {
                error = "internet 0";
                yield break;
            }
            else
            {
                UnityWebRequest cacheRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/cache.xml");
                cacheRequest.SendWebRequest();
                while (!cacheRequest.isDone)
                {
                    yield return new WaitForSeconds(.01f);
                }
                Version.Use.cacheXML = cacheRequest.downloadHandler.text;
            }
        }

        if (curUserCache.status.Equals("0"))
        { key = XML.This.GetCache_B("j"); }
        else if (curUserCache.status.Equals("1"))
        { key = XML.This.GetCache_B("k"); }

        yield return StartCoroutine(SignInUser(Program.Decrypt(curUserCache.GetUID(), Program.Decrypt(curUserCache.GetKey(), key)),
            Program.Decrypt(curUserCache.GetPin(), Program.Decrypt(curUserCache.GetKey(), key))));

        if (currUser == null)
        {
            //File.Delete(Files.Use.DocumentsPath("cache/user.xml"));
        }
    }
    public IEnumerator SignUpAuth(string _ucode, string _pcode, string _nick, GameObject _erorr)
    {
        bool result = false; localId = string.Empty;
        error = string.Empty;

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            error = "internet 0";
            yield break;
        }

        if (Version.Use.cacheXML.Length < 256)
        {
            UnityWebRequest cacheRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/cache.xml");
            cacheRequest.SendWebRequest();
            while (!cacheRequest.isDone)
            {
                yield return new WaitForSeconds(.01f);
            }
            Version.Use.cacheXML = cacheRequest.downloadHandler.text;
        }

        string userData = "{\"email\":\"" + _ucode + "@xxx.xxx" + "\",\"password\":\"" + _pcode + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" +
             Program.Decrypt(XML.This.GetCache_A("a"), Program.Decrypt(XML.This.GetCache_A("l"), XML.This.GetCache_A("k"))), userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;

            User user = new User(_nick, localId);
            RestClient.Put(DataBaseURL + "/" + localId + ".json?auth=" + idToken, user);
            currUser = user;
            result = true;
        })
        .Catch(error =>
        {
            Debug.Log(error.Message);
            _erorr.SetActive(true);
            result = true;
        });
        yield return new WaitUntil(() => result);
        yield return null;
    }
    public string get_userCache_uid()
    {
        string key = string.Empty;
        error = string.Empty;

        if (Version.Use.cacheXML.Length < 256)
            return "cache error";

        key = XML.This.GetCache_B("j");

        return Program.Decrypt(curUserCache.GetUID(), Program.Decrypt(curUserCache.GetKey(), key));
    }
    public string get_userCache_pin()
    {
        string key = string.Empty;
        error = string.Empty;

        if (Version.Use.cacheXML.Length < 256)
            return "cache error";

        key = XML.This.GetCache_B("j");

        return Program.Decrypt(curUserCache.GetPin(), Program.Decrypt(curUserCache.GetKey(), key));
    }
    // Push ======================================================================
    public IEnumerator PushComment(Comment _set)
    {
        bool result = false;

        _set.localId = localId;
        _set.userName = currUser.userName;
        _set.date = System.DateTime.Now.ToString("yyyy-MM-dd");

        yield return StartCoroutine(SignInWithSaveFile());
        yield return RestClient.Post("https://arknights-db.firebaseio.com/operator/" + operatorName + "/comments.json?auth=" + idToken, _set).Then(response =>
        {
            result = true;
        });
        yield return new WaitUntil(() => result);
        yield return null;
    }
    IEnumerator PatchComment(Comment _set)
    {
        string json = JsonUtility.ToJson(_set);

        UnityWebRequest www = UnityWebRequest.Put("https://arknights-db.firebaseio.com/operator/Bagpipe/comments/" + "hMb7r9sTHtbQcT6OZhcUwTt3kVx2" + ".json?auth=" + idToken, json);
        www.method = "PATCH";
        yield return www.SendWebRequest();
    }
    public IEnumerator PatchCommentCount(int _count)
    {
        Count count = new Count();
        count.comment = _count;

        string json = JsonUtility.ToJson(count);

        UnityWebRequest www = UnityWebRequest.Put("https://arknights-db.firebaseio.com/operator/" + operatorName + "/count.json?auth=" + idToken, json);
        www.method = "PATCH";
        yield return www.SendWebRequest();
    }
    public IEnumerator PatchCommentPoint(Point _set, string _operator, string _key)
    {
        string json = JsonUtility.ToJson(_set);

        UnityWebRequest www = UnityWebRequest.Put(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-rating/{1}.json?auth={2}", _operator, _key, idToken), json);
        www.method = "PATCH";
        yield return www.SendWebRequest();
    }
    public IEnumerator PatchCommentUser(PointUser _set, string _operator, string _key)
    {
        string json = JsonUtility.ToJson(_set);

        UnityWebRequest www = UnityWebRequest.Put(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-users/{1}/{3}.json?auth={2}", _operator, _key, idToken, localId), json);
        www.method = "PATCH";
        yield return www.SendWebRequest();
    }
    // Push ======================================================================
    // Get =======================================================================
    public IEnumerator GetComments(CommentList _data, string operName, string limit="Z")
    {
        string url = string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments.json?orderBy=\"$key\"&endAt=\"{1}\"&limitToLast=10&print=pretty", operName, limit);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        Debug.Log(url);
        if (www.isNetworkError && www.isHttpError)  {    }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Comment>), ref deserialized);

            if (_data.data == null)
                _data.data = new Dictionary<string, Comment>();

            _data.data.Clear();
            _data.data = deserialized as Dictionary<string, Comment>;
        }
    }
    public IEnumerator GetCommentsOrderByBest3(CommentList _data)
    {
        _data.data.Clear();
        _data.point.Clear();

        UnityWebRequest www = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-rating.json?orderBy=\"good\"&limitToLast=3&print=pretty", operatorName));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError)  { }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Point>), ref deserialized);
            var bestlist = deserialized as Dictionary<string, Point>;
            if (bestlist == null)
                yield break;

            List<string> keys = bestlist.Keys.ToList();
            var orderDictianary = new Dictionary<string, int>();
            for (int i = 0; i < bestlist.Count; i++)
                orderDictianary.Add(keys[i], bestlist[keys[i]].good);

            var orderValue = orderDictianary.OrderByDescending(x => x.Value);

            for (int i = 0; i < orderValue.Count(); i++)
            {
                KeyValuePair<string, int> curValue = orderValue.ElementAt(i);
                UnityWebRequest wwwC = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments/{1}.json", operatorName, curValue.Key));
                yield return wwwC.SendWebRequest();
                if (wwwC.isNetworkError && wwwC.isHttpError) { }
                else
                {
                    var curData = fsJsonParser.Parse(wwwC.downloadHandler.text);
                    object curDeserialized = null;
                    serializer.TryDeserialize(curData, typeof(Comment), ref curDeserialized);
                    var curComments = curDeserialized as Comment;
                    _data.data.Add(curValue.Key, curComments);
                }
            }
        }
    }
    public IEnumerator GetCommentCount(Count _set)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://arknights-db.firebaseio.com/operator/" + operatorName + "/count.json");
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError)
        {
            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            _set.comment = -1;
        }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Count), ref deserialized);
            var curData = deserialized as Count;
            if (curData != null)
                _set.comment = curData.comment;
            else
                _set.comment = 0;
        }
    }
    public IEnumerator GetCommentPoint(Dictionary<string, Point> _data, string _operator, string _id, CommentIcon _ui = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-rating/{1}.json", _operator, _id));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError)
        {

        }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Point), ref deserialized);

            _data.Clear();
            _data.Add(_id, deserialized as Point);
            if (_ui != null)
                _ui.RefreshPoint(_data[_id]);
        }
    }
    public IEnumerator get_commentPoint_from_firebase(Dictionary<string, Point> _data, string _operator, string _id, CommentIcon _ui = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-rating/{1}.json", _operator, _id));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError) { }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Point), ref deserialized);

            if (_data.ContainsKey(_id))
                _data.Remove(_id);
            _data.Add(_id, deserialized as Point);
            if (_ui != null)
                _ui.RefreshPoint(_data[_id]);
        }
    }
    public IEnumerator GetCommentUser(CommentIcon _data, string _operator, string _id)
    {
        UnityWebRequest www = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/operator/{0}/comments-users/{1}/{2}.json", _operator, _id, localId));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError)
        {
        }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(PointUser), ref deserialized);
            _data.SetActiveMenu(deserialized as PointUser, true);
        }
    }
    // Get =======================================================================
    // ranking data =======================================================================
    public IEnumerator getStagePointRankingLimit20(List<int> _data, List<string> _user)
    {
        _data.Clear();
        _user.Clear();

        UnityWebRequest www = UnityWebRequest.Get("https://arknights-db.firebaseio.com/stage-drop/point.json?orderBy=\"point\"&limitToLast=20&print=pretty");
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError) { }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, PointS>), ref deserialized);
            var valueDictionary = deserialized as Dictionary<string, PointS>;

            if (valueDictionary == null)
                yield break;

            List<string> keys = valueDictionary.Keys.ToList();
            var orderDictianary = new Dictionary<string, int>();
            for (int i = 0; i < valueDictionary.Count; i++)
                orderDictianary.Add(keys[i], valueDictionary[keys[i]].point);
            // orderby class PointS > point [3, 2, 1]
            var orderValue = orderDictianary.OrderByDescending(x => x.Value);

            for (int i = 0; i < orderValue.Count(); i++)
            {
                // get cur Dictionary data > try Key
                KeyValuePair<string, int> curValue = orderValue.ElementAt(i);
                UnityWebRequest wwwC = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/users/{0}.json", curValue.Key));
                yield return wwwC.SendWebRequest();
                if (wwwC.isNetworkError && wwwC.isHttpError) { }
                else
                {
                    var curData = fsJsonParser.Parse(wwwC.downloadHandler.text);
                    object curDeserialized = null;
                    serializer.TryDeserialize(curData, typeof(User), ref curDeserialized);
                    var curComments = curDeserialized as User;

                    if (curComments != null)
                    {
                        _user.Add(curComments.userName);
                        _data.Add(curValue.Value);
                    }
                }
            }
        }
    }
    // StageDropData ======================================================================
    public IEnumerator PushDropData(StageDropData _set)
    {
        bool result = false;

        _set.date = System.DateTime.Now.ToString("yyyyMMddhhmmss");
        if (System.DateTime.Now.ToString("tt").Equals("PM"))
        {
            long dateCount = long.Parse(_set.date);
            dateCount += 120000;
            _set.date = dateCount.ToString();
        }
        string json = JsonUtility.ToJson(_set);

        yield return StartCoroutine(SignInWithSaveFile());
        yield return RestClient.Post("https://arknights-db.firebaseio.com/stage-drop/public/" + localId + ".json?auth=" + idToken, json).Then(response =>
        {
            result = true;
        });
        yield return new WaitUntil(() => result);
        yield return null;
    }
    public IEnumerator set_DELETE_dropdata_firebase(string pushId)
    {
        UnityWebRequest www = UnityWebRequest.Delete(string.Format("https://arknights-db.firebaseio.com/stage-drop/public/{0}/{1}.json?auth={2}", localId, pushId, idToken));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError){ }
        else
        {
            StartCoroutine(LogU.Use.SetLog("delete key" + pushId));
        }
    }
    public IEnumerator GetUserDropData(List<StageDropData> dropData, List<string> pushId)
    {
        if (localId == string.Empty)
            yield return StartCoroutine(SignInWithSaveFile());

        UnityWebRequest www = UnityWebRequest.Get(string.Format("https://arknights-db.firebaseio.com/stage-drop/public/{0}.json?orderBy=\"date\"&limitToLast=10&print=pretty", localId));
        yield return www.SendWebRequest();
        if (www.isNetworkError && www.isHttpError)
        {
        }
        else
        {
            var data = fsJsonParser.Parse(www.downloadHandler.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, StageDropData>), ref deserialized);
            var tmp = deserialized as Dictionary<string, StageDropData>;
            if (tmp != null)
            {
                var res = tmp.Values.ToList();
                var res_key = tmp.Keys.ToList();

                res.Reverse();
                res_key.Reverse();

                for (int i = 0; i < res.Count; i++)
                {
                    dropData.Add(res[i]);
                    pushId.Add(res_key[i]);
                }
            }
            else
                dropData.Clear();
        }
    }
    // Push ======================================================================
    public UserCache GetUserCache()
    {
        return curUserCache;
    }
    public User GetCurrentUser()
    {
        return currUser;
    }
    public string GetUID()
    {
        return localId;
    }
    public string GetError()
    {
        return error;
    }
    public void TR_PutPassword(InputField set)
    {
        Debug.Log(set.text);
    }
}
 