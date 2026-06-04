using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textLine;

    [SerializeField] private string[] currentLines;
    private int index;
    private Coroutine typingCoroutine;

    [Header("Settings")]
    [SerializeField] private float textSpeed;

    private void OnEnable() => AreaZone.PlayerInside += OnDialogue;
    private void OnDisable() => AreaZone.PlayerInside -= OnDialogue;

    private void OnDialogue(bool status, GameObject gameObject)
    {
        if (status)
        {
            currentLines = gameObject.GetComponent<Dialogue>().lines;
            dialogueBox.SetActive(true);
            StartDialogue();
        }
        else
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            currentLines = null;
            dialogueBox.SetActive(false);
        }
    }

    public void OnNextButtonClicked()
    {
        if (textLine.text == currentLines[index])
        {
            NextLine();
        }
        else
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            textLine.text = currentLines[index];
        }
    }

    private void StartDialogue()
    {
        index = 0;
        textLine.text = string.Empty;

        TypingCoroutine();
    }

    private void TypingCoroutine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in currentLines[index].ToCharArray())
        {
            textLine.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            textLine.text = string.Empty;
            TypingCoroutine();
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }
}
