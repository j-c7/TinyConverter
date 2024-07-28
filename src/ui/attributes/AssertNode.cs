namespace TinyConverter;

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class AssertNode : System.Attribute
{
	private string _errorMessage = "";

	public AssertNode(string p_error_message)
	{
		_errorMessage = p_error_message;
	}

	public string ErrorMessage => _errorMessage;
}