using UnityEngine;
using TMPro; // Убедитесь, что вы подключили TextMeshPro

public class FirebaseStatus : MonoBehaviour
{
    public TextMeshProUGUI statusText; // Ссылка на ваш компонент TextMeshProUGUI

    void Start()
    {
        // Проверка статуса Firebase после загрузки
        Application.ExternalCall("checkFirebaseStatus");
    }

    // Метод вызывается из JavaScript
    public void OnFirebaseConnectionStatusReceived(string status)
    {
        if (statusText != null)
        {
            Debug.Log("Получен статус Firebase: " + status); // Логируем статус
            statusText.text = status; // Обновляем текст в TextMeshPro компоненте
        }
        else
        {
            Debug.LogWarning("statusText не задан в FirebaseStatus.");
        }
    }
}