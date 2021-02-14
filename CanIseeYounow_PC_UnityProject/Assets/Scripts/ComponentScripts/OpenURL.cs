using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void Link(string url)
    {
        Application.OpenURL(url);
    }
}
