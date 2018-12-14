using System.Collections;
using UnityEngine;

public class ReplHelper : MonoBehaviour
{
    public T Find<T>() where T : Object => FindObjectOfType<T>();
    public T[] FindAll<T>() where T : Object => FindObjectsOfType<T>();
    public Coroutine RunCoroutine(IEnumerator i) => StartCoroutine(i);
    public void EndCoroutine(Coroutine c) => StopCoroutine(c);
}

public static class REPL
{
    private static GameObject go;

    static REPL()
    {
        go = new GameObject("UnityREPL");
        MB = go.AddComponent<ReplHelper>();
    }

    public static T Find<T>() where T : Object => MB.Find<T>();
    public static T[] FindAll<T>() where T : Object => MB.FindAll<T>();
    public static Coroutine RunCoroutine(IEnumerator i) => MB.RunCoroutine(i);
    public static void EndCoroutine(Coroutine c) => MB.EndCoroutine(c);

    public static ReplHelper MB { get; }
}

