using System;
using Godot.Collections;

namespace TinyConverter.Core;

public static partial class NodeExtends
{
    public static T GetChildByType<T>(this Node p_parent, bool p_returnFirst = true)// where T : Node
    {
        Array<Node> stack = [p_parent];
        while (stack.Count > 0)
        {
            Node current = p_returnFirst ? stack.PopFront() : stack.PopBack();
            if (current is T t)
            {
                return t;
            }

            foreach (var child in current.GetChildren())
            {
                stack.Add(child);
            }
        }
        return default;
    }

    public static T GetChildByType<T>(this Node p_parent, Func<T, bool> p_predicate, bool p_returnFirst = true)
    {
        Array<Node> stack = [p_parent];
        while (stack.Count > 0)
        {
            Node current = p_returnFirst ? stack.PopFront() : stack.PopBack();
            if (current is T t)
            {
                if (p_predicate.Invoke(t))
                {
                    return t;
                }
            }
            foreach (var child in current.GetChildren())
            {
                stack.Add(child);
            }
        }
        return default;
    }

    public static Array<Node> GetAllChildrenByType<T>(this Node p_parent)
    {
        Array<Node> stack = [p_parent];
        Array<Node> ret = [];
        while (stack.Count > 0)
        {
            Node current = stack.PopFront();
            if (current != p_parent)
            {
                ret.Add(current);
            }
            foreach (var child in current.GetChildren())
            {
                if (child is T)
                    stack.Add(child);
            }
        }
        return ret;
    }

    public static void AssertNode<T>(this T p_node, SceneTree p_Tree, string p_what = "", bool p_stop_app = true)
    {
        if (p_node is null && OS.IsDebugBuild())
        {
            GD.PrintErr(p_what);
            if (p_stop_app)
                p_Tree.Quit();
        }
    }
}
