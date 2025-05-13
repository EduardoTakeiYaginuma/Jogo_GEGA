using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private readonly Dictionary<GameObject, Stack<GameObject>> pools = new();

    public GameController gameController;
    void Awake() => Instance = this;

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(prefab, out var stack) || stack.Count == 0)
            return Instantiate(prefab, pos, rot);

        GameObject obj = stack.Pop();
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        if (!pools.ContainsKey(prefab)) pools[prefab] = new Stack<GameObject>();
        pools[prefab].Push(obj);
    }
}
