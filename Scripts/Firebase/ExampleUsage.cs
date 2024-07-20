using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    public FirebaseManager firebaseManager;

    void Start()
    {
        // Пример вызова метода
        firebaseManager.SendDataToFirebase("Vlad", 500);
    }
}