using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StdBattleManager : MonoBehaviour
{
    // 기수 규칙: 장르적 독립 매니저는 고유 싱글톤 Inst 사용
   // public static DaniTechBattleManager Inst { get; private set; }

    private bool _isBattleActive = false;

    // 모의 테스트용 유닛 데이터 (실제 데이터는 인스턴스화된 모델 사용)
    private float _playerHp = 50f;
    private float _enemyHp = 35f;

    // 플레이어와 적이 장착한 모의 아이템 데이터 ID 목록
    private List<string> _playerEquippedItems = new List<string>() { "IT_SWORD_01", "IT_SHIELD_01" };
    private List<string> _enemyEquippedItems = new List<string>() { "IT_DAGGER_POISON" };

    private void Awake()
    {
//        Inst = this;
    }

    /// <summary>
    /// 메인 UI의 준비 완료 버튼 등을 통해 호출되면 전투 루프를 가동한다.
    /// </summary>
    public void StartMockBattle()
    {
        if (_isBattleActive) return;

        _playerHp = 50f;
        _enemyHp = 35f;
        _isBattleActive = true;

        Debug.Log("[Battle] 모의 전투가 시작되었습니다!");

        // 각 아이템들의 쿨타임 루프 돌리기
        foreach (var itemId in _playerEquippedItems)
        {
            StartCoroutine(Co_ItemCooldownRoutine(itemId, isPlayer: true));
        }

        foreach (var itemId in _enemyEquippedItems)
        {
            StartCoroutine(Co_ItemCooldownRoutine(itemId, isPlayer: false));
        }
    }

    /// <summary>
    /// 아이템별 고유 쿨타임에 맞춰 독립적으로 작동하는 코루틴
    /// </summary>
    private IEnumerator Co_ItemCooldownRoutine(string itemDataId, bool isPlayer)
    {
        // 1. JSON 도감에서 아이템 정적 데이터 로드 (기존 구현된 DaniTechGameDataManager 가정)
        // var itemData = DaniTechGameDataManager.Instance.GetDNItemData(itemDataId);

        // 임시 하드코딩 데이터 (테스트용)
        float cooldown = itemDataId == "IT_SWORD_01" ? 3.0f : (itemDataId == "IT_SHIELD_01" ? 4.0f : 2.5f);

        while (_isBattleActive)
        {
            yield return new WaitForSeconds(cooldown);

            if (_isBattleActive == false) yield break;

            // 2. 쿨타임 충전 완료 시 아이템 트리거 처리 호출
            TriggerItemEffect(itemDataId, isPlayer);

            // 3. 승패 조건 체크
            CheckBattleEndCondition();
        }
    }

    private void TriggerItemEffect(string itemDataId, bool isPlayer)
    {
        // [기획 4번 룰 반영]: 여기서는 즉시 발동이지만, 차후 InteractionPriority에 따라 큐(Queue) 정렬 연산 필요
        if (isPlayer)
        {
            if (itemDataId == "IT_SWORD_01")
            {
                _enemyHp -= 4f;
                Debug.Log($"[플레이어 턴] 초보자의 검 시전! -> 적 체력: {_enemyHp}");
            }
            else if (itemDataId == "IT_SHIELD_01")
            {
                Debug.Log($"[플레이어 턴] 나무 방패 시전! -> 방어 5 획득");
            }
        }
        else
        {
            if (itemDataId == "IT_DAGGER_POISON")
            {
                _playerHp -= 2f;
                Debug.Log($"[적 턴] 암살자의 단검 시전! -> 플레이어 체력: {_playerHp} (중독 1 부여)");
            }
        }
    }

    private void CheckBattleEndCondition()
    {
        if (_enemyHp <= 0)
        {
            _isBattleActive = false;
            Debug.Log("[Battle] 플레이어 승리! 전투 종료.");
        }
        else if (_playerHp <= 0)
        {
            _isBattleActive = false;
            Debug.Log("[Battle] 플레이어 패배! 게임 오버.");
        }
    }
}

