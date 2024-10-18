using UnityEngine;
using UnityEngine.UI;

public class GameManagerController : MonoBehaviour
{
    public void SetAdventureMode(bool value)
    {
        GameManager.Instance.SetAdventureMode(value);
    }

    public void SetAdventureMode(Toggle toggle)
    {
        GameManager.Instance.SetAdventureMode(toggle.isOn);
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    public void SetCurrentLevelName(string name)
    {
        GameManager.Instance.SetCurrentLevelName(name);
    }

    public void GoToLink(string link)
    {
        Application.OpenURL(link);
    }
}
