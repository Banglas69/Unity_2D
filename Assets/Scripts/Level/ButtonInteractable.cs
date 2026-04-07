using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    public SpriteRenderer targetRenderer;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    [Header("UI")]
    public string promptText = "Press E";

    [Header("Use")]
    public bool oneUse = false;

    [Header("Event")]
    public UnityEvent onInteract;

    private bool used;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        SetHighlighted(false);
    }

    public void Interact()
    {
        if (used && oneUse) return;

        onInteract?.Invoke();
        used = true;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (targetRenderer == null) return;
        targetRenderer.color = highlighted ? highlightColor : normalColor;
    }

    public string GetPromptText()
    {
        return promptText;
    }
}