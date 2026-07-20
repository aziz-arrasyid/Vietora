using System.Collections;
using UnityEngine;
using PrimeTween;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    [Header("UI Components")]
    [SerializeField] private CanvasGroup panel;

    [Header("Settings")]
    [SerializeField] private float duration;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        StartCoroutine(CompleteTransition());
    }

    public void ChangeScene(string sceneName)
    {
        if (panel.blocksRaycasts) return;

        StartCoroutine(BeginTransition(sceneName));
    }

    private IEnumerator CompleteTransition()
    {
        panel.alpha = 1f;

        yield return Tween.Alpha(panel, endValue: 0f, duration: duration, ease: Ease.InSine).ToYieldInstruction();

        panel.blocksRaycasts = false;
    }

    private IEnumerator BeginTransition(string sceneName)
    {
        panel.blocksRaycasts = true;

        ARSession session = FindFirstObjectByType<ARSession>();


        yield return Tween.Alpha(panel, endValue: 1f, duration: duration, ease: Ease.OutSine).ToYieldInstruction();

        if (session != null)
        {
            session.Reset();
            session.enabled = false;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        StartCoroutine(CompleteTransition());
    }
}
