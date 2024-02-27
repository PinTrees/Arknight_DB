using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class User
{
    public string userID;
    public string userName;
    public User(string _username, string _userID)
    {
        userName = _username;
        userID = _userID;
    }
}
