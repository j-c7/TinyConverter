namespace TinyConverter.Core;

public partial class Result : RefCounted
{
    public string FileName{ get; set; }

    public bool Success{ get; set; }

    public string Message{ get; set; }
}