using System.Collections.Generic;

/// <summary>
/// 行为树黑板 - 节点间共享数据存储
/// </summary>
public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    /// <summary>
    /// 设置黑板数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void SetValue<T>(string key, T value)
    {
        data[key] = value;
    }

    /// <summary>
    /// 获取黑板数据（类类型）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>黑板数据值</returns>
    public T GetValue<T>(string key, T defaultValue = default) where T : class
    {
        if (data.TryGetValue(key, out var value))
        {
            return value as T;
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取黑板数据（值类型）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>黑板数据值</returns>
    public T GetStructValue<T>(string key, T defaultValue = default) where T : struct
    {
        if (data.TryGetValue(key, out var value) && value is T)
        {
            return (T)value;
        }
        return defaultValue;
    }


    /// <summary>
    /// 检查是否存在某个键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否存在</returns>
    public bool HasValue(string key)
    {
        return data.ContainsKey(key);
    }

    /// <summary>
    /// 移除键值对
    /// </summary>
    /// <param name="key">键值对的键</param>
    public void RemoveValue(string key)
    {
        data.Remove(key);
    }

    /// <summary>
    /// 清空黑板
    /// </summary>
    public void Clear()
    {
        data.Clear();
    }
}