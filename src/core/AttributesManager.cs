using System;
using System.Collections.Generic;
using System.Reflection;

namespace TinyConverter.Core;

public partial class AttributesManager : Node
{
    private SceneTree _tree;

    public override void _EnterTree()
    {
        _tree = GetTree();
        _tree.NodeAdded += OnNodeAdded;
    }

    public override void _ExitTree()
    {
        _tree.NodeAdded -= OnNodeAdded;
    }

    protected void AssertNode<T>(T p_node, string p_what = "", bool p_stop_app = true)
    {
        if (p_node is null && OS.IsDebugBuild())
        {
            GD.PrintErr(p_what);
            if (p_stop_app)
                GetTree().Quit();
        }
    }

    private void OnNodeAdded(Node p_node)
    {
        StartAttributes(p_node);
    }

    private void StartAttributes(Node p_node)
    {
        // Properties
        PropertyInfo[] properies = this.GetType().GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (PropertyInfo pInfo in properies)
        {
            IEnumerable<Attribute> attributes = pInfo.GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is AssertNode)
                {
                    AssertNode prop = attr as AssertNode;
                    AssertNode(pInfo.GetValue(this) as Node,
                        $"The node '{pInfo.Name}' is required in '{this.Name}'"
                    );
                }
            }
        }

        // Fiels
        FieldInfo[] fields = this.GetType().GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (FieldInfo fInfo in fields)
        {
            IEnumerable<Attribute> attributes = fInfo.GetCustomAttributes();
            foreach (Attribute attr in attributes)
            {
                if (attr is AssertNode)
                {
                    AssertNode prop = attr as AssertNode;
                    AssertNode(
                        fInfo.GetValue(this) as Node,
                        $"The node '{fInfo.Name}' is required in '{this.Name}'"
                    );
                }
            }
        }

    }
}
