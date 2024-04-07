using Shuile.Gameplay;

using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyProperty", menuName = "Config/Enemy Property")]
public class EnemyPropertySO : ScriptableObject
{
    [Header("Base property")]
    public int healthPoint;
    public int attackPoint;

    [Header("Behaviour")]
    public int attackRange;
    [Tooltip("攻击前摇拍数")] public int preAttackDuration = 1;
    [Tooltip("攻击后摇拍数")] public int postAttackDuration = 0;

    [Header("Move Behaviour")]
    [Tooltip("0则代表每一拍都会试图移动")] public int moveInterval = 0;
    public MoveAbility moveAbility = MoveAbility.Jumpable;
    public float acceleration = 0.2f;
    public float deceleration = 0.1f;
    public float maxMoveSpeed = 5f;
    public float jumpForce = 5f;
}
