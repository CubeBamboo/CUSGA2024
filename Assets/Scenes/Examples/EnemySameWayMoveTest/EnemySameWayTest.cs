using Shuile;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

using UnityEngine;

#if UNITY_EDITOR
public class EnemySameWayTest : MonoBehaviour
{
    private void Start()
    {
        var enemyPrefab = NoteEventUtils.EnemyType2Prefab(EnemyType.ZakoRobot);
        var playerPos = GameplayService.Interface.Get<Player>().transform.position;

        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(-3, 0, 0));
        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(-2, 0, 0));

        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(2, 0, 0));
        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(3, 0, 0));
    }
}
#endif
