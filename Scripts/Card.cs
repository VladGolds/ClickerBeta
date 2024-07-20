using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public string name;
    public float basePrice;
    public float baseCatsPerSecond;
    public int level;

    public TMP_Text priceText;
    public TMP_Text catsPerSecondText;
    public TMP_Text levelText;

    public float CurrentPrice
    {
        get { return basePrice * Mathf.Pow(2, level); }
    }

    public float CurrentCatsPerSecond
    {
        get { return baseCatsPerSecond * level; }
    }

    // Конструктор для инициализации карточки
    public Card(string name, float basePrice, float baseCatsPerSecond, int level, TMP_Text priceText, TMP_Text catsPerSecondText, TMP_Text levelText)
    {
        this.name = name;
        this.basePrice = basePrice;
        this.baseCatsPerSecond = baseCatsPerSecond;
        this.level = level;
        this.priceText = priceText;
        this.catsPerSecondText = catsPerSecondText;
        this.levelText = levelText;
    }

    // Пустой конструктор для сериализации
    public Card() { }

    public void UpdateTexts()
    {
        if (priceText != null)
        {
            priceText.text = "" + CurrentPrice.ToString("F2");
        }
        if (catsPerSecondText != null)
        {
            catsPerSecondText.text = "Cats/Hour: " + CurrentCatsPerSecond.ToString("F1");
        }
        if (levelText != null)
        {
            levelText.text = "LVL: " + level;
        }
    }
}