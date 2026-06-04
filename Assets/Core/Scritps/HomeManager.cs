using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitBtn;

    private void Start()
    {
        startBtn.onClick.AddListener(OnStartBtnClicked);
        settingsBtn.onClick.AddListener(OnSettingsBtnClicked);
        creditsBtn.onClick.AddListener(OnCreditsBtnClicked);
        exitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnStartBtnClicked()
    {
        Debug.Log("Start button klik!");
    }

    private void OnSettingsBtnClicked()
    {
        Debug.Log("Settings button klik!");
    }

    private void OnCreditsBtnClicked()
    {
        Debug.Log("Credits button klik!");
    }

    private void OnExitBtnClicked()
    {
        Debug.Log("Exit button klik!");
    }
}
