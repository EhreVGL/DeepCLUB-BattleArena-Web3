using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DisplayBalance : MonoBehaviour
{
    TextMeshProUGUI balance;
    void Start()
    {
        balance = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        Web3.OnBalanceChange += OnBalanceChange;
    }
    private void OnDisable()
    {
        Web3.OnBalanceChange -= OnBalanceChange;
    }
    void OnBalanceChange(double amount)
    {
        balance.text = amount.ToString(CultureInfo.InvariantCulture);
    }
}
