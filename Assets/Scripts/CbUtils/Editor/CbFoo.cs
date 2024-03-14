using Shuile;
using UnityEngine;
using UnityEngine.InputSystem;


public class CbFoo : MonoBehaviour
{
    public GameObject player;

    public void Attack()
    {
        player.GetComponent<IAttackable>().OnAttack(1);
    }
}
