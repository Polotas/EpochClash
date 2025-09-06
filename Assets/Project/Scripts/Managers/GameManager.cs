using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int currentGold;
    public int currentGoldEarned;
    public int currentEra;
    public SaveSettings saveSettings;
    public SaveUpgrade saveUpgrade;

    public bool isPause;
    public event Action<int> onCurrencyChanged;
    public event Action<int> onCurrencyEarnedChanged;
    public event Action<bool> onPause;
    public event Action<float> onChangedAudioBG;
    public event Action<float> onChangedAudioFX;
    public event Action<int> onUpgradeBase;
    public event Action<float> onUpgradeMeat;
    public event Action onInitGame;
    public event Action onEndGame;
    
    private EnemySpawner _enemySpawner;
    private PlayerSpawner _playerSpawner;
    private UIGameController _uiGameController;
    private UIPopupWelcome _uiPopupWelcome;
    private UIPause _uiPause;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            _enemySpawner = FindFirstObjectByType<EnemySpawner>();
            _playerSpawner = FindFirstObjectByType<PlayerSpawner>();
            _uiGameController = FindFirstObjectByType<UIGameController>();
            _uiPopupWelcome = FindFirstObjectByType<UIPopupWelcome>();
            _uiPause = FindFirstObjectByType<UIPause>();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() => LoadGame();

    #region Components

    private void SaveGame()
    {
        SaveData data = new SaveData
        {
            gold = currentGold,
            currentEra = currentEra,
            saveSettings = saveSettings,
            saveUpgrade = saveUpgrade
        };

        SaveManager.Save(data);
    }
    
    private void LoadGame()
    {
        SaveData data = SaveManager.Load();
        if (data != null)
        {
            currentGold = data.gold;
            currentEra = data.currentEra;
            saveSettings = data.saveSettings;
            saveUpgrade = data.saveUpgrade;
            
            // UPDATE UI EVENTS
            onCurrencyChanged?.Invoke(currentGold);
            _uiGameController.InitUpdateSettings();
            saveUpgrade.meatSpeed.factorPriceMultiply = 1.4f;

        }
        else
        {
            _uiPopupWelcome.Open();
            
            // BASE LIFE UPGRADE
            saveUpgrade.baseLive.startStats = 10;
            saveUpgrade.baseLive.startPrice = 10;
            saveUpgrade.baseLive.currentLevel = 0;
            saveUpgrade.baseLive.factorPriceMultiply = 1.9f;

            // MEAT UPGRADE
            
            saveUpgrade.meatSpeed.startStats = 4;
            saveUpgrade.meatSpeed.startPrice = 5;
            saveUpgrade.meatSpeed.maxLevel = 15;
            saveUpgrade.meatSpeed.currentLevel = 0;
            saveUpgrade.meatSpeed.factorPriceMultiply = 1.4f;
            currentGold = 50;
            onCurrencyChanged?.Invoke(currentGold);
            SaveGame();
        }
    }
    
    public void AddGold(int valor)
    {
        currentGold += valor;
        onCurrencyChanged?.Invoke(currentGold);
        SaveGame();
    }

    public void AddGoldEarned(int valor)
    {
        currentGoldEarned += valor;
        onCurrencyEarnedChanged?.Invoke(currentGoldEarned);
    }
    
    public void Pause(bool active)
    {
        isPause = active;
        onPause?.Invoke(active);
    }

    #endregion
    
    #region Game

    public void InitGame() =>  onInitGame?.Invoke();

    public void CollectEarned()
    {
        currentGoldEarned = 0;
        onCurrencyEarnedChanged?.Invoke(0);
    }

    public void RemoveUnits()
    {
        _enemySpawner.EndGame();
        _playerSpawner.EndGame();
    }
    
    public void EndGame(bool quit = false,bool win = true)
    {
        if(quit)
        {
            _uiGameController.EndGame();
        }
        else
        {
            _uiPause.EndGame(win);
        }
        
        RemoveUnits();
        onEndGame?.Invoke();
    }   

    #endregion
    
    #region Settings

    public void ChangeAudioBG(float valor)
    {
        saveSettings.bgAudio = valor;
        AudioManager.SetVolume(AudioManager.AudioType.BG,valor);
        SaveGame();
    }
    
    public void ChangeAudioFX(float valor)
    {
        saveSettings.fxAudio = valor;
        AudioManager.SetVolume(AudioManager.AudioType.FX,valor);
        SaveGame();
    }

    #endregion

    #region Upgrade

    public void UpgradeLiveBase()
    {
        AddGold(-saveUpgrade.baseLive.getPrice());
        saveUpgrade.baseLive.currentLevel++;
    }
    
    public void UpgradeMeat()
    {
        AddGold(-saveUpgrade.meatSpeed.getPrice());
        saveUpgrade.meatSpeed.currentLevel++;
        onUpgradeMeat?.Invoke(saveUpgrade.meatSpeed.getCurrentStatsRemove());
    }

    public bool CheckButtonUnlock(float value)
    {
        return value <= currentGold;
    }

    public void UnlockUnit2()
    {
        saveUpgrade.unlockUnit2 = true;
        SaveGame();
    }
    
    public void UnlockUnit3()
    {
        saveUpgrade.unlockUnit3 = true;
        SaveGame();
    }

    #endregion

}

[System.Serializable]
public class SaveData
{
    public int gold;
    public int currentEra;
    public SaveSettings saveSettings;
    public SaveUpgrade saveUpgrade;
}

[System.Serializable]
public class SaveSettings
{
    public float bgAudio = 0.5f;
    public float fxAudio = 0.5f;
}

[System.Serializable]
public class SaveUpgrade
{
    public int unit1Level;
    public int unitLevel2;
    public int unitLevel3;
    public UpgradeInfo baseLive;
    public UpgradeInfo meatSpeed;

    public bool unlockUnit2 = false;
    public bool unlockUnit3 = false;
}

[System.Serializable]
public class UpgradeInfo
{
    public float startStats;
    public int startPrice;
    public int maxLevel;
    public int currentLevel;
    public float removeStats = .2f;
    public float factorPriceMultiply = 1.2f;
    
    public int getPrice() => Mathf.RoundToInt(startPrice * Mathf.Pow(factorPriceMultiply, currentLevel));

    public int getCurrentStatsMultiplier()
    {
        float stats = startStats;

        for (int i = 0; i < currentLevel; i++)
            stats += stats * factorPriceMultiply;
        
        return (int)stats;
    }
    
    public float getCurrentStatsRemove()
    {
        float stats = startStats;

        for (int i = 0; i < currentLevel; i++)
            stats -= removeStats;
        
        return stats;
    }
    
    public int getNextAddMultiplier()
    {
        float stats = getCurrentStatsMultiplier() * factorPriceMultiply;;

        return (int)stats;
    }

    public bool isMaxLevel() => currentLevel >= maxLevel;
}