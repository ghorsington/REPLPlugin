using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Mono.CSharp;
using UnityEngine;
using Attribute = System.Attribute;
using Object = UnityEngine.Object;

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

public class TypeHelper
{
    public object instance;
    public Type type;

    public TypeHelper(Type type)
    {
        this.type = type;
        instance = null;
    }

    public TypeHelper(object instance)
    {
        this.instance = instance;
        type = instance.GetType();
    }

    public T val<T>(string name) where T : class
    {
        var field = type.GetField(name,
                                  BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                  | BindingFlags.NonPublic);

        if (field != null)
        {
            if (!field.IsStatic && instance == null)
                throw new ArgumentException("Field is not static, but instance is missing.");
            return field.GetValue(field.IsStatic ? null : instance) as T;
        }

        var prop = type.GetProperty(name,
                                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                                    | BindingFlags.Instance);

        if (prop == null || !prop.CanWrite)
            throw new ArgumentException($"No field or settable property of name {name} was found!");

        var getter = prop.GetSetMethod(true);

        if (!getter.IsStatic && instance == null)
            throw new ArgumentException("Property is not static, but instance is missing.");

        return getter.Invoke(getter.IsStatic ? null : instance, null) as T;
    }

    public void set(string name, object value)
    {
        var field = type.GetField(name,
                                  BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                  | BindingFlags.NonPublic);

        if (field != null)
        {
            if (!field.IsStatic && instance == null)
                throw new ArgumentException("Field is not static, but instance is missing.");
            field.SetValue(field.IsStatic ? null : instance, value);
            return;
        }

        var prop = type.GetProperty(name,
                                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                                    | BindingFlags.Instance);

        if (prop == null || !prop.CanWrite)
            throw new ArgumentException($"No field or settable property of name {name} was found!");

        var setter = prop.GetSetMethod(true);

        if (!setter.IsStatic && instance == null)
            throw new ArgumentException("Property is not static, but instance is missing.");

        setter.Invoke(setter.IsStatic ? null : instance, new[] {value});
    }

    public object invoke(string name, params object[] args)
    {
        var method = type.GetMethod(name,
                                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                    | BindingFlags.NonPublic);
        if (method == null)
            throw new ArgumentException($"No method of name {name} was found!");
        if (!method.IsStatic && instance == null)
            throw new ArgumentException("Method is not static, but instance is missing.");

        return method.Invoke(method.IsStatic ? null : instance, args);
    }

    public string info()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Info about {type.FullName}");
        sb.AppendLine("Methods");

        foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                                                   | BindingFlags.Instance))
        {
            bool putComma = false;
            sb.Append(methodInfo.IsPublic ? "public" : "private").Append(" ");
            if (methodInfo.ContainsGenericParameters)
            {
                sb.Append("<");
                foreach (var genericArgument in methodInfo.GetGenericArguments())
                {
                    if (putComma)
                        sb.Append(", ");
                    sb.Append(genericArgument.FullName);
                    putComma = true;
                }

                sb.Append(">");
            }

            sb.Append(methodInfo.Name).Append("(");

            putComma = false;
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (putComma)
                    sb.Append(", ");
                sb.Append(parameterInfo.ParameterType.FullName);
                if (parameterInfo.DefaultValue != DBNull.Value)
                    sb.Append($"= {parameterInfo.DefaultValue}");
                putComma = true;
            }

            sb.AppendLine(")");
        }

        sb.AppendLine().AppendLine("Fields");

        foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
        )
        {
            sb.Append(fieldInfo.IsPublic ? "public" : "private").Append(" ");
            sb.AppendLine(fieldInfo.Name);
        }

        return sb.ToString();
    }
}

public class REPL : InteractiveBase
{
    private static readonly GameObject go;

    static REPL()
    {
        go = new GameObject("UnityREPL");
        MB = go.AddComponent<ReplHelper>();
    }

    public new static string help
    {
        get
        {
            string original = InteractiveBase.help;

            var sb = new StringBuilder();
            sb.AppendLine("In addition, the following helper methods are provided:");
            foreach (var methodInfo in typeof(REPL).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = methodInfo.GetCustomAttributes(typeof(DocumentationAttribute), false);
                if (attr.Length == 0)
                    continue;
                sb.Append("  ");
                sb.AppendLine(((DocumentationAttribute) attr[0]).Docs);
            }

            return $"{original}\n{sb}";
        }
    }


    [Documentation("MB - A dummy MonoBehaviour for accessing Unity.")]
    public static ReplHelper MB { get; }

    [Documentation("find<T>() - find a UnityEngine.Object of type T.")]
    public static T find<T>() where T : Object
    {
        return MB.Find<T>();
    }

    [Documentation("findAll<T>() - find all UnityEngine.Object of type T.")]
    public static T[] findAll<T>() where T : Object
    {
        return MB.FindAll<T>();
    }

    [Documentation("runCoroutine(enumerator) - runs an IEnumerator as a Unity coroutine.")]
    public static Coroutine runCoroutine(IEnumerator i)
    {
        return MB.RunCoroutine(i);
    }

    [Documentation("endCoroutine(co) - ends a Unity coroutine.")]
    public static void endCoroutine(Coroutine c)
    {
        MB.EndCoroutine(c);
    }

    [Documentation("type<T>() - obtain type info about a type T. Provides some Reflection helpers.")]
    public static TypeHelper type<T>()
    {
        return new TypeHelper(typeof(T));
    }

    [Documentation("type(obj) - obtain type info about object obj. Provides some Reflection helpers.")]
    public static TypeHelper type(object instance)
    {
        return new TypeHelper(instance);
    }

    [Documentation("dir(obj) - lists all available methods and fiels of a given obj.")]
    public static string dir(object instance)
    {
        return type(instance).info();
    }

    [Documentation("dir<T>() - lists all available methods and fields of type T.")]
    public static string dir<T>()
    {
        return type<T>().info();
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    private class DocumentationAttribute : Attribute
    {
        public DocumentationAttribute(string doc)
        {
            Docs = doc;
        }

        public string Docs { get; }
    }
}