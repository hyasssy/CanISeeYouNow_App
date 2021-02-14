using UnityEngine;

public class UICommands : MonoBehaviour
{
    public void SwitchActive(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}