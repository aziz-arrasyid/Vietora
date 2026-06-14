using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private QuizzManager quizzManager;
    [Header("UI Components Dialogue")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textLine;

    [Header("UI Components Chat Log")]
    [SerializeField] private GameObject chatLogBtn;
    [SerializeField] private GameObject chatLogContent;
    [SerializeField] private TextMeshProUGUI textVA;
    [SerializeField] private TextMeshProUGUI textContent;

    [SerializeField] private string[] currentLines;
    private Dialogue dialogue;
    private int index;
    private Coroutine typingCoroutine;
    private GameObject currentObj;

    [Header("Settings")]
    [SerializeField] private float textSpeed;

    private void OnEnable() => AreaZone.PlayerInside += OnDialogue;
    private void OnDisable() => AreaZone.PlayerInside -= OnDialogue;
    private void OnDialogue(bool status, GameObject obj)
    {
        if (status)
        {
            currentLines = obj.GetComponent<Dialogue>().lines;
            dialogueBox.SetActive(true);
            StartDialogue();
            ReadyChatLog(obj);
            currentObj = obj;
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
            chatLogBtn.SetActive(false);
        }
    }

    private void CheckReadingCompleted(GameObject obj)
    {
        GameObject parentObj = obj.transform.parent.gameObject;
        int parentIndex = spawnManager.objectSpawned.FindIndex(o => o.parentObject == parentObj);

        if (parentIndex != -1)
        {
            if (parentObj != null)
            {
                QRGrouping parent = spawnManager.objectSpawned[parentIndex];
                Dialogue[] child = parent.parentObject.GetComponentsInChildren<Dialogue>();

                bool check = child.All(c => c.isReadingCompleted);
                parent.isReadingCompletedAll = check;

                spawnManager.objectSpawned[parentIndex] = parent;

                if(check)
                {
                    quizzManager.QuizzReady(parent.QRText.ToString());
                }
            }
        }

    }

    #region Dialogue
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
            dialogue.isReadingCompleted = true;
            CheckReadingCompleted(currentObj);
        }
    }
    #endregion

    #region Chat Log
    public void OnChatLogClicked()
    {
        chatLogContent.SetActive(true);
        dialogueBox.SetActive(false);
        dialogue.isReadingCompleted = true;
        CheckReadingCompleted(currentObj);
    }

    public void OnCloseChatLogClicked()
    {
        chatLogContent.SetActive(false);
        dialogueBox.SetActive(true);
    }

    private void ReadyChatLog(GameObject obj)
    {
        chatLogBtn.SetActive(true);
        dialogue = obj.GetComponent<Dialogue>();
        textVA.text = $"VA: {dialogue.VA} ID";

        string fullText = string.Join("\n\n", currentLines);
        textContent.text = fullText;
    }
    #endregion

}
