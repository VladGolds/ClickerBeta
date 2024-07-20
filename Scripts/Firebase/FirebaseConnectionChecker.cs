using UnityEngine;

public class FirebaseConnectionChecker : MonoBehaviour
{
    void Start()
    {
        // Вызов функции JavaScript из Unity
        Application.ExternalCall("getFirebaseConnectionStatus");
    }

    // Вызывается из JavaScript
    public void OnFirebaseConnectionStatusReceived(string status)
    {
        Debug.Log("Статус подключения Firebase: " + status);
    }
}