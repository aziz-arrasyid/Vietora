using DG.Tweening;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private RectTransform BTNContainer;

    private void Start()
    {
        BTNContainer.localScale = Vector3.zero;
        BTNContainer.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
}
