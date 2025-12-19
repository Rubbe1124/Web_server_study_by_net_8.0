using System;
using UnityEngine;
using _1_Scripts;
using SharedLibrary;

public class WebManager : MonoBehaviour
{
    async void Start()
    {
        var player = await HttpClient.Get<Player>("https://localhost:44345/player/500");
    }
}
