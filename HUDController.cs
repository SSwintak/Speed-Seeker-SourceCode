using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Dictionary<string, Canvas> GameCanvases { get; private set; } = new Dictionary<string, Canvas>();

    void Awake()
    {
        var canvases = GameObject.FindObjectsOfType<Canvas>(true);
        foreach (Canvas c in canvases)
        {
            if (GameCanvases.ContainsKey(c.name)) continue;
            GameCanvases.Add(c.name, c);
        }        
    }

    public void ShowMenu(string name)
    {
        if (!GameCanvases.ContainsKey(name)) return;

        GameCanvases[name].gameObject.SetActive(true);
    }

    public void HideMenu(string name)
    {
        if (!GameCanvases.ContainsKey(name)) return;

        GameCanvases[name].gameObject.SetActive(false);
    }
}
