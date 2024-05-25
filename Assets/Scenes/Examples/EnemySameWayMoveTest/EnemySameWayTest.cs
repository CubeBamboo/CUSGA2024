using Shuile;
using Shuile.Core.Gameplay;
using Shuile.Gameplay;

using UnityEngine;

#if UNITY_EDITOR
public class EnemySameWayTest : MonoBehaviour
{
    private void Start()
    {
        var enemyPrefab = LevelEntityUtils.EnemyType2Prefab(EnemyType.ZakoRobot);
        var playerPos = GameplayService.Interface.Get<Player>().transform.position;

        LevelEntityFactory.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(-3, 0, 0));
        LevelEntityFactory.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(-2, 0, 0));

        LevelEntityFactory.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(2, 0, 0));
        LevelEntityFactory.Instance.SpawnEnemy(enemyPrefab, playerPos + new Vector3(3, 0, 0));
    }
}
#endif
