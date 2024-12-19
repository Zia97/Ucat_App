using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using System;

public class PlayGameManager : MonoBehaviour
{
    public TMP_Text nameText;
    public string playerName = "TestName123";

    void Start()
    {
        SignIn();
    }

    public void SignIn()
    {

        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void SignInManually()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            nameText.text = PlayGamesPlatform.Instance.GetUserDisplayName();
            playerName = PlayGamesPlatform.Instance.GetUserDisplayName();
            Console.WriteLine("Logged in "+playerName);
        }
        else
        {
            nameText.text = "Failed!";
            playerName = "Player";
            Console.WriteLine("Failed login in ");

        }
    }
}
