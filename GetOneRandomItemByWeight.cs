//int-->T
public static int GetOneRandomItemByWeight(int[] items)
    {
        float maxPower = 0;
        foreach (var item in items)
        {
            maxPower += item;
        }
        float randomResult = UnityEngine.Random.Range(0, maxPower);
        float tempValue = 0;
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            tempValue += item;
            if (randomResult <= tempValue)
            {
                return i;
            }
        }
        return 0;
    }
