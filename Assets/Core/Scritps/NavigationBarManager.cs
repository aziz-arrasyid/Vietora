using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationBarManager : MonoBehaviour
{
    [SerializeField] private HomeManager homeManager;
    InputAction backAction;

    private void Awake()
    {
        backAction = new(binding: "<Keyboard>/escape");
        backAction.performed += ctx => OnBackAction();
    }

    private void OnBackAction()
    {
        homeManager.OnExitBtnClicked();
    }
}
