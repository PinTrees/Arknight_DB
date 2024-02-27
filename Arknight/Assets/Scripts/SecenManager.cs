using UnityEngine;

using UnityEngine.SceneManagement;

public class SecenManager : MonoBehaviour
{
    public void Trigger_LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
