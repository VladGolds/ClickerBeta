using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public void SendDataToFirebase(string name, int score)
    {
        Application.ExternalCall("sendDataToFirebase", name, score);
    }
}