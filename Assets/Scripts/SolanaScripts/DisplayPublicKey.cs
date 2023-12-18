using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPublicKey : MonoBehaviour
{
    TextMeshProUGUI publicKey;
    void Start()
    {
        publicKey = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        Web3.OnLogin += OnLogin;
    }
    private void OnDisable()
    {
        Web3.OnLogin -= OnLogin;
    }
    void OnLogin(Account account)
    {
        publicKey.text = account.PublicKey;
    }
}
