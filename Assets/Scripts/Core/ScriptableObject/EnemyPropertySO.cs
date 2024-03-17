using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyProperty", menuName = "Config/Enemy Property")]
public class EnemyPropertySO : ScriptableObject
{
    [Header("Base property")]
    public int healthPoint;
    public int attackPoint;

    [Header("Behaviour")]
    public int viewRange;
    public int attackRange;
    [Tooltip("0则代表每一拍都会试图移动")] public int moveInterval = 0;
}
