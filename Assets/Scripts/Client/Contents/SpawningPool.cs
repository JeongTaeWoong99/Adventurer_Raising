using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    int monsterCount  = 0;
    int _reserveCount = 0;

    [SerializeField] int keepMonsterCount = 0;

    [SerializeField] Vector3 spawnPos;
    [SerializeField] float   spawnRadius = 15.0f;
    [SerializeField] float   spawnTime   = 5.0f;

    public void AddMonsterCount(int value) { monsterCount += value; }
    public void SetKeepMonsterCount(int count) { keepMonsterCount = count; }

    private void Start()
    {
        ClientManager.Game.OnSpawnEvent -= AddMonsterCount;  // 초기화
        ClientManager.Game.OnSpawnEvent += AddMonsterCount;  // 등록
    }

    private void Update()
    {
        while (_reserveCount + monsterCount < keepMonsterCount)
        {
            StartCoroutine(nameof(ReserveSpawn));
        }
    }

    private IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, spawnTime));
        GameObject   obj = ClientManager.Game.Spawn(Define.WorldObject.Monster, "Knight");
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
        
        Vector3 randPos;
        while (true)
        {
            // 랜덤 크기의 원 안에서, Y(위아래)값을 제외한 랜덤 위치 생성
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, spawnRadius);
			randDir.y = 0;
			randPos = spawnPos + randDir;

            // 건물과 겹치지 않으면, 탈출
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
		}

        obj.transform.position = randPos;
        _reserveCount--;
    }
}
