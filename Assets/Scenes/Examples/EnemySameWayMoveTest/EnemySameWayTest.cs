using Shuile;
using Shuile.Gameplay;
using Shuile.Rhythm;

using UnityEngine;

#if UNITY_EDITOR
public class EnemySameWayTest : MonoBehaviour
{
    private void Start()
    {
        var enemyPrefab = NoteEventHelper.EnemyType2Prefab(EnemyType.ZakoRobot);
        var playerGridPos = LevelGrid.Instance.grid.WorldToCell(GameplayService.Interface.Get<PlayerController>().transform.position);

        Debug.Log(playerGridPos);
        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerGridPos + new Vector3Int(-3, 0, 0));
        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerGridPos + new Vector3Int(-2, 0, 0));

        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerGridPos + new Vector3Int(2, 0, 0));
        EnemyManager.Instance.SpawnEnemy(enemyPrefab, playerGridPos + new Vector3Int(3, 0, 0));

        Debug.Log("Created");
    }
}
#endif