using UnityEngine;

public class EnemyCollision2D : ACharacterCollision2D
{
    [SerializeField]
    protected int maxHealth = 5, currentHealth = 4;

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.PlayerAbility;
    }

    protected override void OnHit() {
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        currentHealth--;
        if (currentHealth < 0) {
            Destroy(gameObject);
        }
    }
}
