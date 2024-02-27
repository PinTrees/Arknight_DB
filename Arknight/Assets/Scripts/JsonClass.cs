using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FireBaseClass
{
    public class Status_FB
    {
        public int comment;
    }
    [Serializable]
    public class Count
    {
        public int comment;
    }
    [Serializable]
    public class Comment
    {
        public string localId;
        public string userName;
        public string info;
        public string date;
    }
    [Serializable]
    public class StageDropData
    {
        public string code;     // Stagecode
        public string material;     //ST_1,SG_3
        public string count;    // 1,2,1
        public string date;     // Query orderBy key 
    }
    [Serializable]
    public class PointsC
    {
        public string localId;
        public string userName;
        public int count;
    }
    [Serializable]
    public class PointS
    {
        public int point;
    }
    [Serializable]
    public class Point
    {
        public int good;
        public int bad;
    }
    [Serializable]
    public class PointUser
    {
        public int type;
    }
}
