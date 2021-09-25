using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SpeedTestController : MonoBehaviour
{
    // 100万回
    public const int COUNT = 1000000;
    
    private ObjectMemoryPool<Component> _objectMemory1;
    private ObjectMemoryPool<TestAccessClass> _objectMemory2;
    private static string _message;
    private void Awake()
    {
        this._objectMemory1 = new ObjectMemoryPool<Component>(5, () => new GameObject("Test", typeof(TestAccessClass)).GetComponent<TestAccessClass>());
        this._objectMemory2 = new ObjectMemoryPool<TestAccessClass>(5, () => new GameObject("Test", typeof(TestAccessClass)).GetComponent<TestAccessClass>());
    }

    private void Start()
    {
        var btton = this.gameObject.GetComponent<Button>();
        btton.onClick.AddListener(this.SpeedTest);
    }

    private void SpeedTest()
    {
        UnityEngine.Debug.Log("Start");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < COUNT; i++) {
            var item = this._objectMemory1.Alloc();
            item.gameObject.SetActive(true);
            _message = item.GetType().GetField("_guid", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(item).ToString();
            item.gameObject.SetActive(false);
            this._objectMemory1.Free(item);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"毎回GetType、GetFieldしてGetValue:{stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        var typeObject = this._objectMemory1.Alloc();
        var fieldInfo = typeObject.GetType().GetField("_guid", BindingFlags.NonPublic | BindingFlags.Instance);
        this._objectMemory1.Free(typeObject);
        stopwatch.Start();
        for (int i = 0; i < COUNT; i++) {
            var item = this._objectMemory1.Alloc();
            item.gameObject.SetActive(true);
            _message = fieldInfo.GetValue(item).ToString();
            item.gameObject.SetActive(false);
            this._objectMemory1.Free(item);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"FieldInfo固定でGetValue:{stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        stopwatch.Start();
        for (int i = 0; i < COUNT; i++) {
            var item = this._objectMemory2.Alloc();
            item.gameObject.SetActive(true);
            _message = item.GUID;
            item.gameObject.SetActive(false);
            this._objectMemory2.Free(item);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"プロパティの素アクセス:{stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        stopwatch.Start();
        for (int i = 0; i < COUNT; i++) {
            var item = this._objectMemory2.Alloc();
            item.gameObject.SetActive(true);
            _message = item.g_guid;
            item.gameObject.SetActive(false);
            this._objectMemory2.Free(item);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"メンバ変数への素アクセス:{stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        UnityEngine.Debug.Log($"Finish");
    }
}
