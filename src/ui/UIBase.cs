using System;
using System.Reflection;
using System.Collections.Generic;

namespace TinyConverter;

public partial class UIBase : Control
{

    public override void _EnterTree()
    {
        Initialize();
    }

    private void Initialize()
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
				if (attr is CheckRequired)
				{
					CheckRequired prop = attr as CheckRequired;
					CheckRequiredNode(pInfo.GetValue(this) as Node,
						$"The node '{pInfo.Name}' is required in '{this.Name}' {prop.ErrorMessage}"	
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
				if (attr is CheckRequired)
				{
					CheckRequired prop = attr as CheckRequired;
					CheckRequiredNode(
						fInfo.GetValue(this) as Node,
						$"The node '{fInfo.Name}' is required in '{this.Name}' {prop.ErrorMessage}"
					);
				}
			}
		}
	}

	protected void CheckRequiredNode<T>(T p_node, string p_what = "", bool p_stop_app = true)
	{
		if(p_node is null)
		{
			GD.PrintErr(p_what);
			if(p_stop_app)
				GetTree().Quit();
		}
	}
}
