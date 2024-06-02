using Godot.Collections;
using Godot;
using System.Threading.Tasks;
using System;
namespace TinyConversor;

public interface IApp
{
	OutFormats OutFormat { get; set; }

	float JpgQuality { get; set; }

	string OutPath { get; }

	Action<string> FinalizeTask { get; set; }

	void SetSourcesPath(string[] p_path);

	void SetOutPath(string p_path);

	Task<Response> Start();
}