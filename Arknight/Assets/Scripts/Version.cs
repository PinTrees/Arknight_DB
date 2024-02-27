using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Version : MonoBehaviour
{
    public static Version Use;
    public string versionXML;
    public string cacheXML;
    private void Start()
    {
        Use = this;
    }
}
