using UnityEngine;
using TMPro;
using System;

public class PassiveIncomeManager : MonoBehaviour
{
    public CoinClicker coinClicker; // Ссылка на CoinClicker
    public Card[] cards;
    public TMP_Text totalCatsText;
    public TMP_Text catsPerSecondText;

    private float totalCats;
    private float catsPerSecond;

    private DateTime lastVisitTime;
    private float passiveIncomeLimitHours = 3.0f;

    private void Start()
    {
        LoadData();
        LoadLastVisitTime();

        // Проверка времени пассивного дохода при запуске игры
        CheckPassiveIncome();

        UpdateCatsPerSecond();
        UpdateCardTexts(); // Обновление текстов карточек при запуске игры
        InvokeRepeating(nameof(AddPassiveIncome), 3f, 3f);
    }

    private void Update()
    {
        totalCatsText.text = "Total Cats: " + totalCats.ToString("F0");
        catsPerSecondText.text = "Cats per Second: " + catsPerSecond.ToString("F1");
    }

    public void UpgradeCard(int index)
    {
        if (index < 0 || index >= cards.Length)
        {
            Debug.LogError("Invalid card index: " + index);
            return;
        }

        if (coinClicker.SpendCoins((int)cards[index].CurrentPrice))
        {
            cards[index].level++;
            UpdateCatsPerSecond();
            cards[index].UpdateTexts(); // Обновление текстов карточки после апгрейда
            SaveData();
            Debug.Log("Card upgraded! New level: " + cards[index].level);
        }
        else
        {
            Debug.Log("Not enough coins to upgrade card.");
        }
    }

    private void AddPassiveIncome()
    {
        totalCats += catsPerSecond;
        SaveData();
    }

    private void UpdateCatsPerSecond()
    {
        catsPerSecond = 0;
        foreach (Card card in cards)
        {
            catsPerSecond += card.CurrentCatsPerSecond;
        }
    }

    private void UpdateCardTexts()
    {
        // Обновление текстов всех карточек
        foreach (Card card in cards)
        {
            card.UpdateTexts();
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat("TotalCats", totalCats);
        for (int i = 0; i < cards.Length; i++)
        {
            PlayerPrefs.SetInt("CardLevel" + i, cards[i].level);
        }

        PlayerPrefs.SetString("LastVisitTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        totalCats = PlayerPrefs.GetFloat("TotalCats", 0);
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].level = PlayerPrefs.GetInt("CardLevel" + i, 0);
        }
    }

    private void LoadLastVisitTime()
    {
        string lastVisitTimeString = PlayerPrefs.GetString("LastVisitTime", DateTime.Now.ToString());
        if (!DateTime.TryParse(lastVisitTimeString, out lastVisitTime))
        {
            lastVisitTime = DateTime.Now;
            Debug.LogWarning("Failed to parse LastVisitTime, using current time.");
        }
    }

    private void CheckPassiveIncome()
    {
        TimeSpan offlineTime = DateTime.Now - lastVisitTime;

        if (offlineTime.TotalHours <= passiveIncomeLimitHours)
        {
            double effectiveOfflineHours = offlineTime.TotalHours;
            float catsEarnedOffline = (float)(effectiveOfflineHours * 3600) * catsPerSecond;
            totalCats += catsEarnedOffline;
            Debug.Log($"Earned {catsEarnedOffline} cats while offline for {offlineTime.TotalHours:F2} hours. Total cats now: {totalCats}");
        }
        else
        {
            Debug.Log("Offline income limit exceeded. No cats earned.");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
        else
        {
            LoadData();
            LoadLastVisitTime();
            CheckPassiveIncome();
            UpdateCatsPerSecond();
            UpdateCardTexts(); // Обновление текстов карточек после возвращения из паузы
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}