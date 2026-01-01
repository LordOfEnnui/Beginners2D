using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Object/Player State")]
public class PlayerState : ScriptableObject {
    [Header("Status")]
    [SerializeField]
    public float maxHealth = 5, currentHealth = 4;
    public float maxOil = 100, currentOil = 0;
    public float requiredOil = 80;

    [Header("Invincibility")]
    [SerializeField]
    public float damageIframes = 60, sprintIframes = 30, flashesPerSecond = 2;

    [Header("HitStop")]
    [SerializeField]
    public float damageHitStopDuration = 0.1f, damageHitStopTimeScale = 0.0f;

    [Header("Events")]
    [SerializeField]
    public UnityEvent onDamage, onDeath;

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        onDamage.Invoke();
        if (currentHealth < 0) {
            onDeath.Invoke();
        }
    }
}
