using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    public LayerMask interactableMask;
    public float interactDistance = 1.5f;

    [Header("UI")]
    public TMP_Text interactText;

    private Collider2D col;
    private ButtonInteractable currentButton;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        DetectButton();

        if (currentButton != null && Input.GetKeyDown(KeyCode.E))
        {
            currentButton.Interact();
        }
    }

    private void DetectButton()
    {
        float dirX = transform.localScale.x >= 0f ? 1f : -1f;

        Bounds b = col.bounds;
        Vector2 origin = new Vector2(b.center.x, b.center.y);
        Vector2 direction = new Vector2(dirX, 0f);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, interactableMask);

        ButtonInteractable newButton = null;

        if (hit.collider != null)
            newButton = hit.collider.GetComponentInParent<ButtonInteractable>();

        if (currentButton != newButton)
        {
            if (currentButton != null)
                currentButton.SetHighlighted(false);

            currentButton = newButton;

            if (currentButton != null)
            {
                currentButton.SetHighlighted(true);

                if (interactText != null)
                {
                    interactText.text = currentButton.GetPromptText();
                    interactText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (interactText != null)
                    interactText.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;
        Bounds b = c.bounds;
        Vector2 origin = new Vector2(b.center.x, b.center.y);
        Vector2 end = origin + Vector2.right * dirX * interactDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, end);
    }
}