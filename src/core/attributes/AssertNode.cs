namespace TinyConverter.Core;

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class AssertNode : System.Attribute
{
	public AssertNode(){}
}