// ObjectPool.cs
// One pooling API for the whole project (SOP section 14), keyed by prefab reference so a rename cannot break it.
// Safe fallback: releasing an instance the pool does not own destroys it instead of corrupting the pool.
// Kill switch: turn pooling off in the inspector to A/B against plain instantiation.
using System.Collections.Generic;
using UnityEngine;

namespace __NAMESPACE__
{
    public sealed class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [SerializeField] bool poolingEnabled = true;

        readonly Dictionary<GameObject, Stack<GameObject>> free = new Dictionary<GameObject, Stack<GameObject>>();
        readonly Dictionary<GameObject, GameObject> prefabOf = new Dictionary<GameObject, GameObject>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject instance = null;
            if (poolingEnabled && free.TryGetValue(prefab, out var stack))
                while (stack.Count > 0 && instance == null) instance = stack.Pop();
            if (instance == null)
            {
                instance = Instantiate(prefab, position, rotation, parent);
                prefabOf[instance] = prefab;
            }
            else
            {
                instance.transform.SetParent(parent, false);
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.SetActive(true);
            }
            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null) return;
            if (!poolingEnabled || !prefabOf.TryGetValue(instance, out var prefab)) { Destroy(instance); return; }
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            if (!free.TryGetValue(prefab, out var stack)) free[prefab] = stack = new Stack<GameObject>();
            stack.Push(instance);
        }
    }
}
