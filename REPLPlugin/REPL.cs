using System.Collections;
using UnityEngine;

public class ReplHelper : MonoBehaviour
{
    public T Find<T>() where T : Object
    {
        return FindObjectOfType<T>();
    }

    public T[] FindAll<T>() where T : Object
    {
        return FindObjectsOfType<T>();
    }

    public Coroutine RunCoroutine(IEnumerator i)
    {
        return StartCoroutine(i);
    }

    public void EndCoroutine(Coroutine c)
    {
        StopCoroutine(c);
    }
}

public static class REPL
{
    private static readonly GameObject go;

    static REPL()
    {
        go = new GameObject("UnityREPL");
        MB = go.AddComponent<ReplHelper>();
    }

    public static ReplHelper MB { get; }

    public static T Find<T>() where T : Object
    {
        return MB.Find<T>();
    }

    public static T[] FindAll<T>() where T : Object
    {
        return MB.FindAll<T>();
    }

    public static Coroutine RunCoroutine(IEnumerator i)
    {
        return MB.RunCoroutine(i);
    }

    public static void EndCoroutine(Coroutine c)
    {
        MB.EndCoroutine(c);
    }
}