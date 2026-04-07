using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerUI2D : MonoBehaviour
{
    [Header("Player")]
    public Health playerHealth;

    [Header("Enemy Raycast")]
    public LayerMask enemyMask;
    public float lookDistance = 2f;

    [Header("UI")]
    public TMP_Text playerHealthText;
    public TMP_Text enemyNameText;
    public TMP_Text enemyHealthText;
    public TMP_Text youDiedText;
    public GameObject enemyPanel;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        if (playerHealth == null)
            playerHealth = GetComponent<Health>();

        if (enemyPanel != null)
            enemyPanel.SetActive(false);

        if (youDiedText != null)
            youDiedText.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdatePlayerHealthUI();
        UpdateEnemyUI();
        UpdateDeathUI();
    }

    private void UpdatePlayerHealthUI()
    {
        if (playerHealthText == null || playerHealth == null) return;

        playerHealthText.text = "HP: " + playerHealth.CurrentHealth + " / " + playerHealth.MaxHealth;
    }

    private void UpdateEnemyUI()
    {
        if (enemyPanel == null || enemyNameText == null || enemyHealthText == null)
            return;

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;

        Bounds b = col.bounds;
        Vector2 origin = new Vector2(b.center.x, b.center.y);
        Vector2 direction = new Vector2(dirX, 0f);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, lookDistance, enemyMask);

        if (hit.collider == null)
        {
            enemyPanel.SetActive(false);
            return;
        }

        Health enemyHealth = hit.collider.GetComponentInParent<Health>();

        if (enemyHealth == null || enemyHealth.transform.root == transform.root || enemyHealth.IsDead)
        {
            enemyPanel.SetActive(false);
            return;
        }

        enemyPanel.SetActive(true);
        enemyNameText.text = enemyHealth.displayName;
        enemyHealthText.text = enemyHealth.CurrentHealth + " / " + enemyHealth.MaxHealth;
    }

    private void UpdateDeathUI()
    {
        if (youDiedText == null || playerHealth == null) return;

        youDiedText.gameObject.SetActive(playerHealth.IsDead);
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;
        Bounds b = c.bounds;

        Vector2 origin = new Vector2(b.center.x, b.center.y);
        Vector2 end = origin + Vector2.right * dirX * lookDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, end);
    }
}