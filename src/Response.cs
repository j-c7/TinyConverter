using Godot;
using System;

namespace TinyConversor;

public partial class Response : RefCounted
{
	public bool Success { get; set; }

	public string Message { get; set; }
}