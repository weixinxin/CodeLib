using System;
using System.Reflection;


public static class OperatePrivate
{
    #region 字段

    /// <summary>
    /// 获得私有字段的值
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="instance">扩展对象类型</param>
    /// <param name="fieldname">私有字段名称 string</param>
    /// <returns>私有字段值 T</returns>
    public static T GetPrivateField<T>(this object instance, string fieldname)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
        Type type = instance.GetType();
        FieldInfo field = type.GetField(fieldname, flag);
        return (T)field.GetValue(instance);
    }

    /// <summary>
    /// 设置私有字段的值
    /// </summary>
    /// <param name="instance">扩展对象类型</param>
    /// <param name="fieldname">私有字段名称 string</param>
    /// <param name="value">私有字段新值 object</param>
    public static void SetPrivateField(this object instance, string fieldname, object value)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
        Type type = instance.GetType();
        FieldInfo field = type.GetField(fieldname, flag);
        field.SetValue(instance, value);
    }

    #endregion

    #region 属性

    /// <summary>
    /// 获取私有属性的值
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="instance">扩展对象类型</param>
    /// <param name="propertyname">私有属性名称 string</param>
    /// <returns>私有字段值 T</returns>
    public static T GetPrivateProperty<T>(this object instance, string propertyname)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
        Type type = instance.GetType();
        PropertyInfo field = type.GetProperty(propertyname, flag);
        return (T)field.GetValue(instance, null);
    }

    /// <summary>
    /// 设置私有属性的值
    /// </summary>
    /// <param name="instance">扩展对象类型</param>
    /// <param name="propertyname">私有属性名称</param>
    /// <param name="value">私有属性新值</param>
    public static void SetPrivateProperty(this object instance, string propertyname, object value)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
        Type type = instance.GetType();
        PropertyInfo field = type.GetProperty(propertyname, flag);
        field.SetValue(instance, value, null);
    }

    #endregion


    #region 方法

    /// <summary>
    /// 调用私有方法
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="instance">扩展对象类型</param>
    /// <param name="name">私有属性名称 string</param>
    /// <param name="param">参数列表 object</param>
    /// <returns>调用方法返回值</returns>
    public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
    {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
        Type type = instance.GetType();
        MethodInfo method = type.GetMethod(name, flag);
        return (T)method.Invoke(instance, param);
    }

    #endregion
}
