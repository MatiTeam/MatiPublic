using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethod
{
    public static string[] SplitString(this string str, string value)
    {
        string[] list = str.Split(new string[] { value }, StringSplitOptions.None);
        return list;
    }

    public static string ToCapitalized(this string str)
    {
        if(string.IsNullOrEmpty(str)) return str;
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    public static string ToString(this string[] param)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string str in param) { sb.Append(str); }
        return sb.ToString();
    }

    public static T GetChildFromName<T>(this Transform target, string name) where T : Component
    {
        T[] transforms = target.gameObject.GetComponentsInChildren<T>();

        foreach (T t in transforms)
        {

            if (t.name == name)
                return t;
        }

        return null;
    }

    public static T[] GetChildsFromName<T>(this Transform target, string name) where T : Component
    {
        T[] transforms = target.gameObject.GetComponentsInChildren<T>();
        List<T> arr = new List<T>();

        foreach (T t in transforms)
        {

            if (t.name == name)
                arr.Add(t);
        }

        return arr.ToArray();
    }

    public static void SetPositionX(this Transform transform, float x)
    {
        var newPos = new Vector3(x, transform.position.y, transform.position.z);
        transform.position = newPos;
    }

    public static void SetPositionY(this Transform transform, float y)
    {
        var newPos = new Vector3(transform.position.x, y, transform.position.z);
        transform.position = newPos;
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        var newPos = new Vector3(transform.position.x, transform.position.y, z);
        transform.position = newPos;
    }

    public static void ChangeAllChieldLayers(this GameObject target, int layer)
    {
        Transform[] transforms = target.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform t in transforms)
            t.gameObject.layer = layer;

    }

    public static void EnableRenderer(this GameObject go, bool enable)
    {
        Renderer[] arrR = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in arrR)
            r.enabled = enable;


        SpriteRenderer[] arrS = go.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer r in arrS)
            r.enabled = enable;


        Image[] arrI = go.GetComponentsInChildren<Image>();
        foreach (Image i in arrI)
            i.enabled = enable;

        Text[] arrT = go.GetComponentsInChildren<Text>();
        foreach (Text t in arrT)
            t.enabled = enable;

    }

    public static T Instance<T>(this T o, string name) where T : MonoBehaviour
    {
        GameObject obj = new GameObject();
        obj.name = name;
        return obj.AddComponent<T>();
    }

    public static T GetInstance<T>(this T obj, string name) where T : Component
    {
        GameObject go = GameObject.Find(name);

        if (go == null)
        {
            return null;
        }

        return go.GetComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component c) where T : Component
    {
        return c.gameObject.GetOrAddComponent<T>();
    }

    public static bool IsBetween(this float value, float min, float max)
    {
        return value > min && value < max;
    }
}
