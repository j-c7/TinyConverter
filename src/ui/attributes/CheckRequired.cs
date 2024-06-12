namespace TinyConverter;

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class CheckRequired : System.Attribute
{
	private string _errorMessage = "";

	public CheckRequired(string p_error_message)
	{
		_errorMessage = p_error_message;
	}

	public string ErrorMessage => _errorMessage;
}