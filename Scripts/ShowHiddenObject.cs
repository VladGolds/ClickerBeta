using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHiddenObject : MonoBehaviour
{
    public GameObject objectToShow; // Объект, который нужно показать/скрыть
    public Button showButton; // Кнопка, при нажатии на которую будет выполняться действие
    public Button resetButton; // Кнопка, при нажатии на которую всё вернется в исходное состояние
    public SpriteRenderer spriteRendererToHide; // SpriteRenderer, в котором находится спрайт, который нужно скрыть

    private Sprite originalSprite; // Оригинальный спрайт, чтобы вернуть его обратно

    void Start()
    {
        // Устанавливаем начальное состояние объекта как скрытое
        objectToShow.SetActive(false);

        // Добавляем слушатель нажатия на кнопки
        showButton.onClick.AddListener(ToggleObject);
        resetButton.onClick.AddListener(ResetObject);

        // Сохраняем оригинальный спрайт
        if (spriteRendererToHide != null)
        {
            originalSprite = spriteRendererToHide.sprite;
        }
    }

    void ToggleObject()
    {
        // Показываем объект
        objectToShow.SetActive(true);

        // Скрываем спрайт
        if (spriteRendererToHide != null)
        {
            spriteRendererToHide.sprite = null;
        }
    }

    void ResetObject()
    {
        // Скрываем объект
        objectToShow.SetActive(false);

        // Восстанавливаем оригинальный спрайт
        if (spriteRendererToHide != null)
        {
            spriteRendererToHide.sprite = originalSprite;
        }
    }
}