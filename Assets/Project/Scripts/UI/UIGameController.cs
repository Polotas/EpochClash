using UnityEngine;

public class UIGameController : MonoBehaviour
{
    private UIHome _uiHome;
    private UIUpdate _uiUpdate;
    private UISettings _uiSettings;
    
    private void Awake()
    {
        _uiHome = FindFirstObjectByType<UIHome>();
        _uiUpdate = FindFirstObjectByType<UIUpdate>();
        _uiSettings = FindFirstObjectByType<UISettings>();
    }

    public void EndGame() => _uiHome.EndGame();

    public void OpenUpdate()
    {
        _uiUpdate.Open();
        _uiHome.ShowOrHideBottomMenu(false);
    }
    
    public void CloseUpdate()
    {
        _uiUpdate.Close();
        _uiHome.ShowOrHideBottomMenu(true);
    }
    
    public void OpenSettings()
    {
        _uiSettings.Open();
        _uiHome.ShowOrHideBottomMenu(false);
    }
    
    public void CloseSetting()
    {
        _uiSettings.Close();
        _uiHome.ShowOrHideBottomMenu(true);
    }

    public void InitUpdateSettings() => _uiSettings.InitUpdate();
}
