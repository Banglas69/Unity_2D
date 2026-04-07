using UnityEngine;

[System.Serializable]
public struct DamageRequest
{
    public int amount;
    public GameObject sender;
    public Vector2 hitPoint;
    public Vector2 direction;

    public DamageRequest(int amount, GameObject sender, Vector2 hitPoint, Vector2 direction)
    {
        this.amount = amount;
        this.sender = sender;
        this.hitPoint = hitPoint;
        this.direction = direction;
    }
}