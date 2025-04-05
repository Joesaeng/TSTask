using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Quaternion GetTargetRotation(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        angle = -angle;

        if (angle < -180)
        {
            angle += 360f;
        }

        return Quaternion.Euler(new Vector3(0, 0, angle));
    }

    [System.Serializable]
    public class WeightedItem<T>
    {
        public T item;
        public float weight;
    }

    public class WeightedRandomUtility
    {
        public static T GetWeightedRandom<T>(List<WeightedItem<T>> weightedItems)
        {
            // 만약 항목이 없으면 기본값 반환
            if (weightedItems.Count == 0)
                return default(T);

            // 모든 가중치를 더하여 총합을 구함
            float totalWeight = 0f;
            foreach (var weightedItem in weightedItems)
            {
                totalWeight += weightedItem.weight;
            }

            // 0부터 총합 사이의 랜덤 값을 생성
            float randomValue = UnityEngine.Random.value * totalWeight;

            // 랜덤 값이 어느 범위에 속하는지 확인하여 항목 선택
            foreach (var weightedItem in weightedItems)
            {
                randomValue -= weightedItem.weight;
                if (randomValue <= 0)
                    return weightedItem.item;
            }

            // 여기까지 왔다면 뭔가 잘못된 것이 아니므로 마지막 항목 반환
            return weightedItems[weightedItems.Count - 1].item;
        }
    }
}