using System;
using System.Collections;
using UnityEngine;

public class MeatManager : MonoBehaviour
{
    public static MeatManager Instance { get; private set; }
    private bool isActive;
    public float totalTimeToGenerate = 1;
    public int currentMeat = 0;
    public float baseMeatGenerationRate = 1f; // Carne por segundo
    public float upgradeMultiplier = 1.2f; // Multiplicador de taxa por upgrade
    public int upgradeCost = 10; // Custo inicial do upgrade
    public int upgradeCostMultiplier = 2; // Multiplicador do custo de upgrade

    private float meatGenerationTimer = 0f;
    private int firstUnityCost = 4;
    
    public event Action<int> OnMeatChanged;
    public event Action<float> OnMeatProgress;
    public event Action<int, int> OnUpgradeCostChanged;
    private Coroutine coMeat;
    private GameManager _gm;
    private UIUnitController _uiUnitController;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            isActive = false;
        }
    }

    private void Start()
    {
        if(Instance == this)
        {
            _gm = GameManager.Instance;
            _gm.onEndGame += EndGame;
            _gm.onUpgradeMeat += UpdateTimer;
            _uiUnitController = FindFirstObjectByType<UIUnitController>();
            totalTimeToGenerate = _gm.saveUpgrade.meatSpeed.getCurrentStatsRemove();
        }
    }

    private void UpdateTimer(float value) => totalTimeToGenerate = value;
    
    private IEnumerator IE_GenerateMeat()
    {
        meatGenerationTimer = 0f;
        firstUnityCost = _uiUnitController.buttons[0].price - 3;
        currentMeat = firstUnityCost;
        OnMeatChanged?.Invoke(currentMeat);
        
        while (isActive)
        {
            float elapsed = 0f;
            while (elapsed < totalTimeToGenerate)
            {
                if (_gm.isPause)
                {
                    yield return new WaitForSeconds(0.1f); // checa a cada 0.1s enquanto pausado
                    continue;
                }

                yield return new WaitForSeconds(0.01f); // tick fixo
                elapsed += 0.01f;
                
                float progress = Mathf.Clamp01(elapsed / totalTimeToGenerate);
                OnMeatProgress?.Invoke(progress);
            }

            AddMeat(Mathf.FloorToInt(baseMeatGenerationRate));
        }
    }

    public void GenerateMeat(bool active)
    {
        isActive = active;

        if (isActive) coMeat = StartCoroutine(IE_GenerateMeat());
        else StopCoroutine(coMeat);
    }

    public void AddMeat(int amount)
    {
        currentMeat += amount;
        OnMeatChanged?.Invoke(currentMeat);
    }

    public bool TryUpgradeGenerationRate()
    {
        if (currentMeat >= upgradeCost)
        {
            currentMeat -= upgradeCost;
            baseMeatGenerationRate *= upgradeMultiplier;
            upgradeCost *= upgradeCostMultiplier;
            Debug.Log("Taxa de geração de carne atualizada para: " + baseMeatGenerationRate + "/s. Próximo custo de upgrade: " + upgradeCost);
            OnMeatChanged?.Invoke(currentMeat);
            OnUpgradeCostChanged?.Invoke(upgradeCost, Mathf.FloorToInt(baseMeatGenerationRate));
            return true;
        }
        else
        {
            Debug.Log("Carne insuficiente para upgrade. Necessário: " + upgradeCost + ", Atual: " + currentMeat);
            return false;
        }
    }

    private void EndGame()
    {
        if(coMeat != null) StopCoroutine(coMeat);
        isActive = false;
        currentMeat = 0;
        AddMeat(0);
    }
}
