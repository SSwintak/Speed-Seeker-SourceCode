using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : SerializedMonoBehaviour
{
    [SerializeField] Dictionary<string, GameObject> Menus = new Dictionary<string, GameObject>();

    [SerializeField] List<TMP_Text> menuBalanceTexts = new List<TMP_Text>();

    [SerializeField] Button ContinueButton;

    string previousMenu;
    string currentMenu;

    void Awake()
    {
        ShipMenuData.OnHasSelectedShip += HandleOnSelectedShip;
    }

    void Start()
    {
        SetAllmenuBalanceTexts();
        GameManager.cashChanged += UpdateMenuBalanceTexts;
    }

    void OnDestroy() 
    {
        GameManager.cashChanged -= UpdateMenuBalanceTexts;
        ShipMenuData.OnHasSelectedShip -= HandleOnSelectedShip;
    }

    void UpdateMenuBalanceTexts()
    {
        SetAllmenuBalanceTexts();
    }

    public void ViewPreviousMenu()
    {
        ViewMenu(previousMenu);
    }

    public void HandleOnSelectedShip(bool value)
    {
        ContinueButton.gameObject.SetActive(value);
    }

    public void ViewMenu(string menuName)
    {
        GameObject menu = GetMenu(menuName);
        if (menu == null) return;
        menu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = menuName;

        // disable all other menus
        foreach (var menus in Menus.Keys)
        {
            if (menus == menuName) continue;
            Menus[menus].SetActive(false);
        }
        
    }    

    public void ViewMenu(string menuName, bool disableOthers = true)
    {
        GameObject menu = GetMenu(menuName);
        if (menu == null) return;
        menu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = menuName;

        if (disableOthers)
        {
            foreach (var menus in Menus.Keys)
            {
                if (menus == menuName) continue;
                Menus[menus].SetActive(false);
            }
        }
    }
    GameObject GetMenu(string name)
    {
        if (Menus.ContainsKey(name)) return Menus[name];
        return null;
    }

    void SetAllmenuBalanceTexts()
    {
        foreach (var text in menuBalanceTexts)
        {
            text.text = GameManager.Instance.GetCash().ToString();
        }
    }

    public void LoadLevel(string levelName) // levels will use the same scene but have different backgrounds and have different difficulty multipliers
    {
        string tempName = levelName.Replace(" ", "");
        SceneManager.LoadSceneAsync(tempName);
    }

    /// <summary>
    /// The name cannot have spaces, if it does the space will be removed
    /// </summary>
    public void LoadLevel(TMP_Text levelName)
    {
        string tempName = levelName.text.Replace(" ", "");
        SceneManager.LoadSceneAsync(tempName);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
