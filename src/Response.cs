namespace TinyConverter;

public partial class Response : RefCounted
{
	public string FileName { get; set; }

	public bool Success { get; set; }

	public string Message { get; set; }
}