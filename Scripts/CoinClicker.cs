using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinClicker : MonoBehaviour
{
    public TMP_Text coinText;
    public TMP_Text coinText2;
    public TMP_Text clickValueText;
    public TMP_Text leagueText;
    public TMP_Text upgradeButtonText;
    public Button upgradeButton;
    public TMP_Text energyText;
    public TMP_Text energyUpgradeButtonText;
    public Button energyUpgradeButton;
    public TMP_Text energyUpgradeLevelText;
    public TMP_Text clickUpgradeLevelText;
    public Slider taskBarSlider;
    public Button replenishEnergyButton;
    public TMP_Text replenishEnergyTimerText;

    private int coinCount;
    private int coinsPerClick = 1;
    private int upgradeCost = 5000;
    private int upgradeIncrement = 100;
    private int upgradeBonus = 2;

    private int maxEnergy = 100;
    private int currentEnergy;
    private int energyUpgradeCost = 5000;
    private int energyUpgradeIncrement = 500;
    private int energyUpgradeLevel = 0;
    private int clickUpgradeLevel = 0;
    private float energyRegenRate = 1f;
    private float energyRegenTimer;
    private bool canReplenishEnergy = true;
    private float replenishCooldown = 3600f;
    private float replenishTimer;

    private enum League { None, Bronze, Silver, Gold, Platinum, Kitty, CatKing, Barsik }
    private League currentLeague = League.None;

    private int coinsForBronze = 200;
    private int coinsForSilver = 400;
    private int coinsForGold = 600;
    private int coinsForPlatinum = 800;
    private int coinsForKitty = 1000;
    private int coinsForCatKing = 1200;
    private int coinsForBarsik = 1400;

    private bool bronzeAchieved;
    private bool silverAchieved;
    private bool goldAchieved;
    private bool platinumAchieved;
    private bool kittyAchieved;
    private bool catKingAchieved;
    private bool barsikAchieved;

    private void Start()
    {
        coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        coinsPerClick = PlayerPrefs.GetInt("CoinsPerClick", 1);
        upgradeCost = PlayerPrefs.GetInt("UpgradeCost", 100);
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        maxEnergy = PlayerPrefs.GetInt("MaxEnergy", 100);
        energyUpgradeCost = PlayerPrefs.GetInt("EnergyUpgradeCost", 1000);
        energyUpgradeLevel = PlayerPrefs.GetInt("EnergyUpgradeLevel", 0);
        clickUpgradeLevel = PlayerPrefs.GetInt("ClickUpgradeLevel", 0);
        bronzeAchieved = PlayerPrefs.GetInt("BronzeAchieved", 0) == 1;
        silverAchieved = PlayerPrefs.GetInt("SilverAchieved", 0) == 1;
        goldAchieved = PlayerPrefs.GetInt("GoldAchieved", 0) == 1;
        platinumAchieved = PlayerPrefs.GetInt("PlatinumAchieved", 0) == 1;
        kittyAchieved = PlayerPrefs.GetInt("KittyAchieved", 0) == 1;
        catKingAchieved = PlayerPrefs.GetInt("CatKingAchieved", 0) == 1;
        barsikAchieved = PlayerPrefs.GetInt("BarsikAchieved", 0) == 1;
        string savedLeague = PlayerPrefs.GetString("CurrentLeague", "None");
        currentLeague = (League)Enum.Parse(typeof(League), savedLeague);

        UpdateTaskBar();

        replenishTimer = PlayerPrefs.GetFloat("ReplenishTimer", 0);
        canReplenishEnergy = PlayerPrefs.GetInt("CanReplenishEnergy", 1) == 1;

        DateTime lastExitTime = DateTime.Parse(PlayerPrefs.GetString("LastExitTime", DateTime.Now.ToString()));
        TimeSpan offlineDuration = DateTime.Now - lastExitTime;

        if (!canReplenishEnergy)
        {
            replenishTimer -= (float)offlineDuration.TotalSeconds;
            UpdateReplenishEnergyStatus();
        }

        UpdateCoinText();
        UpdateClickValueText();
        UpdateLeagueText();
        UpdateUpgradeButtonText();
        UpdateEnergyText();
        UpdateEnergyUpgradeButtonText();
        UpdateEnergyUpgradeLevelText();
        UpdateClickUpgradeLevelText();
        UpdateReplenishEnergyStatus();
        UpdateReplenishEnergyButton();
        UpdateReplenishEnergyTimerText();

        energyRegenTimer = 0f;
        UpdateLeague();
    }

    private void Update()
    {
        energyRegenTimer += Time.deltaTime;
        if (energyRegenTimer >= energyRegenRate)
        {
            energyRegenTimer = 0f;
            RegenerateEnergy();
        }

        if (!canReplenishEnergy)
        {
            replenishTimer -= Time.deltaTime;
            UpdateReplenishEnergyStatus();
            UpdateReplenishEnergyTimerText();
        }

        if (upgradeButton != null)
            upgradeButton.interactable = coinCount >= upgradeCost;
        if (energyUpgradeButton != null)
            energyUpgradeButton.interactable = coinCount >= energyUpgradeCost;

        UpdateLeague();
    }

    void OnMouseDown()
    {
        if (currentEnergy > 0)
        {
            coinCount += coinsPerClick;
            currentEnergy--;
            UpdateCoinText();
            UpdateEnergyText();
            UpdateUpgradeButtonText();
            UpdateEnergyUpgradeButtonText();
            SavePlayerData();

            // Добавляем игрока в топ-10, если его монеты превышают порог
            LeaderboardManager.Instance.CheckAndAddToLeaderboard(PlayerPrefs.GetString("PlayerName"), coinCount);
        }
    }

    void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy++;
            UpdateEnergyText();
            UpdateUpgradeButtonText();
            UpdateEnergyUpgradeButtonText();
            SavePlayerData();
        }
    }

    public void OnUpgradeButtonClicked()
    {
        if (coinCount >= upgradeCost && upgradeButton != null && upgradeButton.interactable)
        {
            coinCount -= upgradeCost;
            coinsPerClick += upgradeBonus;
            upgradeCost += upgradeIncrement;
            clickUpgradeLevel++;
            UpdateCoinText();
            UpdateClickValueText();
            UpdateUpgradeButtonText();
            UpdateClickUpgradeLevelText();
            if (upgradeButton != null)
                upgradeButton.interactable = coinCount >= upgradeCost;
            SavePlayerData();
        }
    }

    public void OnEnergyUpgradeButtonClicked()
    {
        if (coinCount >= energyUpgradeCost && energyUpgradeButton != null && energyUpgradeButton.interactable)
        {
            coinCount -= energyUpgradeCost;
            maxEnergy += energyUpgradeIncrement;
            energyUpgradeCost += energyUpgradeIncrement;
            energyUpgradeLevel++;
            UpdateCoinText();
            UpdateEnergyText();
            UpdateEnergyUpgradeButtonText();
            UpdateEnergyUpgradeLevelText();
            if (energyUpgradeButton != null)
                energyUpgradeButton.interactable = coinCount >= energyUpgradeCost;
            SavePlayerData();
        }
    }

    public void OnReplenishEnergyButtonClicked()
    {
        if (canReplenishEnergy && replenishTimer <= 0 && replenishEnergyButton != null)
        {
            currentEnergy = maxEnergy;
            canReplenishEnergy = false;
            replenishTimer = replenishCooldown;
            UpdateEnergyText();
            UpdateReplenishEnergyButton();
            UpdateReplenishEnergyTimerText();
            SavePlayerData();
        }
    }

    void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = "" + coinCount;
        if (coinText2 != null)
            coinText2.text = "" + coinCount;
        UpdateTaskBar();
    }

    void UpdateClickValueText()
    {
        if (clickValueText != null)
            clickValueText.text = "Click Value: " + coinsPerClick;
    }

    void UpdateLeagueText()
    {
        if (leagueText != null)
            leagueText.text = " " + currentLeague.ToString();
    }

    void UpdateLeague()
    {
        if (coinCount >= coinsForBarsik)
        {
            currentLeague = League.Barsik;
            barsikAchieved = true;
            taskBarSlider.maxValue = coinsForBarsik;
        }
        else if (coinCount >= coinsForCatKing)
        {
            if (!barsikAchieved)
            {
                currentLeague = League.CatKing;
                taskBarSlider.maxValue = coinsForBarsik;
            }
            catKingAchieved = true;
        }
        else if (coinCount >= coinsForKitty)
        {
            if (!barsikAchieved && !catKingAchieved)
            {
                currentLeague = League.Kitty;
                taskBarSlider.maxValue = coinsForCatKing;
            }
            kittyAchieved = true;
        }
        else if (coinCount >= coinsForPlatinum)
        {
            if (!barsikAchieved && !catKingAchieved && !kittyAchieved)
            {
                currentLeague = League.Platinum;
                taskBarSlider.maxValue = coinsForKitty;
            }
            platinumAchieved = true;
        }
        else if (coinCount >= coinsForGold)
        {
            if (!barsikAchieved && !catKingAchieved && !kittyAchieved && !platinumAchieved)
            {
                currentLeague = League.Gold;
                taskBarSlider.maxValue = coinsForPlatinum;
            }
            goldAchieved = true;
        }
        else if (coinCount >= coinsForSilver)
        {
            if (!barsikAchieved && !catKingAchieved && !kittyAchieved && !platinumAchieved && !goldAchieved)
            {
                currentLeague = League.Silver;
                taskBarSlider.maxValue = coinsForGold;
            }
            silverAchieved = true;
        }
        else if (coinCount >= coinsForBronze)
        {
            if (!barsikAchieved && !catKingAchieved && !kittyAchieved && !platinumAchieved && !goldAchieved && !silverAchieved)
            {
                currentLeague = League.Bronze;
                taskBarSlider.maxValue = coinsForSilver;
            }
            bronzeAchieved = true;
        }
        else
        {
            if (!barsikAchieved && !catKingAchieved && !kittyAchieved && !platinumAchieved && !goldAchieved && !silverAchieved && !bronzeAchieved)
            {
                currentLeague = League.None;
                taskBarSlider.maxValue = coinsForBronze;
            }
        }

        UpdateLeagueText();
        UpdateTaskBar();
        SavePlayerData();
    }

    void UpdateUpgradeButtonText()
    {
        if (upgradeButtonText != null)
            upgradeButtonText.text = "" + upgradeCost + "";
    }

    void UpdateEnergyUpgradeButtonText()
    {
        if (energyUpgradeButtonText != null)
            energyUpgradeButtonText.text = "" + energyUpgradeCost + "";
    }

    void UpdateEnergyUpgradeLevelText()
    {
        if (energyUpgradeLevelText != null)
            energyUpgradeLevelText.text = "LVL " + energyUpgradeLevel;
    }

    void UpdateClickUpgradeLevelText()
    {
        if (clickUpgradeLevelText != null)
            clickUpgradeLevelText.text = "LVL " + clickUpgradeLevel;
    }

    void UpdateEnergyText()
    {
        if (energyText != null)
            energyText.text = " " + currentEnergy + " / " + maxEnergy;
    }

    void UpdateReplenishEnergyButton()
    {
        if (replenishEnergyButton != null)
            replenishEnergyButton.interactable = canReplenishEnergy;
    }

    void UpdateReplenishEnergyTimerText()
    {
        if (replenishEnergyTimerText != null)
        {
            if (canReplenishEnergy)
            {
                replenishEnergyTimerText.text = "";
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(replenishTimer);
                replenishEnergyTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
            }
        }
    }

    void UpdateReplenishEnergyStatus()
    {
        canReplenishEnergy = replenishTimer <= 0;
        UpdateReplenishEnergyButton();
    }

    void UpdateTaskBar()
    {
        taskBarSlider.value = coinCount; // Обновляем текущее значение слайдера
    }


    void SavePlayerData()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.SetInt("CoinsPerClick", coinsPerClick);
        PlayerPrefs.SetInt("UpgradeCost", upgradeCost);
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetInt("MaxEnergy", maxEnergy);
        PlayerPrefs.SetInt("EnergyUpgradeCost", energyUpgradeCost);
        PlayerPrefs.SetInt("EnergyUpgradeLevel", energyUpgradeLevel);
        PlayerPrefs.SetInt("ClickUpgradeLevel", clickUpgradeLevel);
        PlayerPrefs.SetInt("BronzeAchieved", bronzeAchieved ? 1 : 0);
        PlayerPrefs.SetInt("SilverAchieved", silverAchieved ? 1 : 0);
        PlayerPrefs.SetInt("GoldAchieved", goldAchieved ? 1 : 0);
        PlayerPrefs.SetInt("PlatinumAchieved", platinumAchieved ? 1 : 0);
        PlayerPrefs.SetInt("KittyAchieved", kittyAchieved ? 1 : 0);
        PlayerPrefs.SetInt("CatKingAchieved", catKingAchieved ? 1 : 0);
        PlayerPrefs.SetInt("BarsikAchieved", barsikAchieved ? 1 : 0);
        PlayerPrefs.SetString("CurrentLeague", currentLeague.ToString());
        PlayerPrefs.SetFloat("ReplenishTimer", replenishTimer);
        PlayerPrefs.SetInt("CanReplenishEnergy", canReplenishEnergy ? 1 : 0);
        PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateCoinText();
        SavePlayerData();
    }

    public bool SpendCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            UpdateCoinText();
            SavePlayerData();
            return true;
        }
        return false;
    }
}
