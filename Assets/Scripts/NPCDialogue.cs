using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Attach this to your NPC GameObject
public class NPCDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker; // "NPC" or "Player"
        [TextArea(3, 10)]
        public string text;
        public int nextLineIndex = -1; // -1 means auto-increment, otherwise jump to specific line
    }
    
    [Header("Dialogue Settings")]
    public List<DialogueLine> dialogueSequence = new List<DialogueLine>();
    public string npcName = "NPC";
    public string playerName = "Player";
    
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI npcNameText;
    public GameObject continueButton;
    public GameObject interactPrompt;
    
    [Header("Animation Settings")]
    public float animationDuration = 0.3f;
    public AnimationType openAnimation = AnimationType.ScaleUp;
    public AnimationType closeAnimation = AnimationType.ScaleDown;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRange = 2f;
    
    private Transform player;
    private bool playerInRange = false;
    private bool isDialogueActive = false;
    private int currentLineIndex = 0;
    private bool isAnimating = false;
    
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private CanvasGroup panelCanvasGroup;
    private RectTransform panelRectTransform;
    
    public enum AnimationType
    {
        ScaleUp,
        SlideFromTop,
        SlideFromBottom,
        Fade,
        ScaleDown
    }
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerAttack = playerObj.GetComponent<PlayerAttack>();
        }
        
        if (dialoguePanel != null)
        {
            panelCanvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
            }
            panelRectTransform = dialoguePanel.GetComponent<RectTransform>();
            dialoguePanel.SetActive(false);
        }
            
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            playerInRange = distance <= interactionRange;
            
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(playerInRange && !isDialogueActive);
            }
        }
        
        if (playerInRange && Input.GetKeyDown(interactKey) && !isDialogueActive && !isAnimating)
        {
            StartDialogue();
        }
        
        if (isDialogueActive && !isAnimating && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(interactKey)))
        {
            NextLine();
        }
        
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            StartCoroutine(CloseAllDialogueWithAnimation());
        }
    }
    
    public void StartDialogue()
    {
        if (dialogueSequence.Count == 0) return;
        
        isDialogueActive = true;
        currentLineIndex = 0;
        
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerAttack != null)
            playerAttack.enabled = false;
            
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
        
        ShowLine(currentLineIndex);
    }
    
    void ShowLine(int index)
    {
        if (index < 0 || index >= dialogueSequence.Count)
        {
            EndDialogue();
            return;
        }
        
        DialogueLine line = dialogueSequence[index];
        
        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue panel is null");
            return;
        }
        
        // UPDATE TEXT FIRST before activating panel and animating
        UpdateDialogueText(line);
        
        dialoguePanel.SetActive(true);
        StartCoroutine(OpenDialogueWithAnimation());
    }
    
    void UpdateDialogueText(DialogueLine line)
    {
        if (dialogueText != null)
            dialogueText.text = line.text;
        if (npcNameText != null)
            npcNameText.text = line.speaker.ToLower() == "player" ? playerName : npcName;
    }
    
    IEnumerator OpenDialogueWithAnimation()
    {
        isAnimating = true;
        
        switch (openAnimation)
        {
            case AnimationType.ScaleUp:
                yield return StartCoroutine(ScaleAnimation(panelRectTransform, Vector3.zero, Vector3.one));
                break;
            case AnimationType.SlideFromTop:
                yield return StartCoroutine(SlideAnimation(panelRectTransform, new Vector2(0, 200), Vector2.zero));
                break;
            case AnimationType.SlideFromBottom:
                yield return StartCoroutine(SlideAnimation(panelRectTransform, new Vector2(0, -200), Vector2.zero));
                break;
            case AnimationType.Fade:
                yield return StartCoroutine(FadeAnimation(panelCanvasGroup, panelRectTransform, 0f, 1f));
                break;
        }
        
        isAnimating = false;
    }
    
    IEnumerator CloseDialogueWithAnimation()
    {
        isAnimating = true;
        
        switch (closeAnimation)
        {
            case AnimationType.ScaleDown:
                yield return StartCoroutine(ScaleAnimation(panelRectTransform, Vector3.one, Vector3.zero));
                break;
            case AnimationType.SlideFromTop:
                yield return StartCoroutine(SlideAnimation(panelRectTransform, Vector2.zero, new Vector2(0, 200)));
                break;
            case AnimationType.SlideFromBottom:
                yield return StartCoroutine(SlideAnimation(panelRectTransform, Vector2.zero, new Vector2(0, -200)));
                break;
            case AnimationType.Fade:
                yield return StartCoroutine(FadeAnimation(panelCanvasGroup, panelRectTransform, 1f, 0f));
                break;
        }
        
        dialoguePanel.SetActive(false);
        isAnimating = false;
    }
    
    IEnumerator CloseAllDialogueWithAnimation()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            yield return StartCoroutine(CloseDialogueWithAnimation());
        }
        
        EndDialogue();
    }
    
    IEnumerator ScaleAnimation(RectTransform rectTransform, Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        rectTransform.localScale = from;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            rectTransform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        
        rectTransform.localScale = to;
    }
    
    IEnumerator SlideAnimation(RectTransform rectTransform, Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPos + from;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos + from, startPos + to, t);
            yield return null;
        }
        
        rectTransform.anchoredPosition = startPos + to;
    }
    
    IEnumerator FadeAnimation(CanvasGroup canvasGroup, RectTransform rectTransform, float from, float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        rectTransform.localScale = Vector3.one;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        
        canvasGroup.alpha = to;
    }
    
    public void NextLine()
    {
        if (isAnimating) return;
        
        StartCoroutine(AdvanceDialogue());
    }
    
    IEnumerator AdvanceDialogue()
    {
        DialogueLine currentLine = dialogueSequence[currentLineIndex];
        
        // Close current bubble
        yield return StartCoroutine(CloseDialogueWithAnimation());
        
        // Determine next line index
        int nextIndex;
        if (currentLine.nextLineIndex >= 0)
        {
            // Use custom link
            nextIndex = currentLine.nextLineIndex;
        }
        else
        {
            // Auto-increment
            nextIndex = currentLineIndex + 1;
        }
        
        // Check if conversation should end
        if (nextIndex >= dialogueSequence.Count || nextIndex < 0)
        {
            EndDialogue();
            yield break;
        }
        
        currentLineIndex = nextIndex;
        ShowLine(currentLineIndex);
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
            
        currentLineIndex = 0;
        
        if (panelRectTransform != null)
        {
            panelRectTransform.localScale = Vector3.one;
            panelCanvasGroup.alpha = 1f;
        }
        
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerAttack != null)
            playerAttack.enabled = true;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}