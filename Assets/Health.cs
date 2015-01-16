using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public const int MaxHitPoints = 100;

    public int CurrentHitPoints { get { return currentHitPoints; } }

    [RPC]
    public void TakeDamage(int amount) {
        currentHitPoints -= amount;

        if (currentHitPoints <= 0) {
            Die();
        }
    }

    void Start() {
        currentHitPoints = MaxHitPoints;
    }

    void Update() {

    }

    void Die() {
        Debug.Log("DETH");
    }

    int currentHitPoints;
}
