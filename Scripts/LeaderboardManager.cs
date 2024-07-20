using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    public List<PlayerData> leaderboard = new List<PlayerData>();
    public TMP_Text[] nameTexts;
    public TMP_Text[] coinTexts;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadLeaderboard();
        UpdateLeaderboardUI();
    }

    public void CheckAndAddToLeaderboard(string playerName, int playerCoins)
    {
        PlayerData newEntry = new PlayerData(playerName, playerCoins);
        leaderboard.Add(newEntry);
        leaderboard.Sort((x, y) => y.coins.CompareTo(x.coins));  // Сортировка по убыванию монет
        if (leaderboard.Count > 10)
        {
            leaderboard.RemoveAt(10);  // Оставляем только 10 лучших игроков
        }
        SaveLeaderboard();
        UpdateLeaderboardUI();
    }

    public void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerPrefs.SetString($"PlayerName{i}", leaderboard[i].name);
            PlayerPrefs.SetInt($"PlayerCoins{i}", leaderboard[i].coins);
        }
        PlayerPrefs.SetInt("LeaderboardCount", leaderboard.Count);
        PlayerPrefs.Save();
    }

    public void LoadLeaderboard()
    {
        int count = PlayerPrefs.GetInt("LeaderboardCount", 0);
        leaderboard.Clear();
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"PlayerName{i}");
            int coins = PlayerPrefs.GetInt($"PlayerCoins{i}");
            leaderboard.Add(new PlayerData(name, coins));
        }
    }

    public void UpdateLeaderboardUI()
    {
        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (i < leaderboard.Count)
            {
                nameTexts[i].text = leaderboard[i].name;
                coinTexts[i].text = leaderboard[i].coins.ToString();
            }
            else
            {
                nameTexts[i].text = "";
                coinTexts[i].text = "";
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public int coins;

    public PlayerData(string playerName, int playerCoins)
    {
        name = playerName;
        coins = playerCoins;
    }
}
