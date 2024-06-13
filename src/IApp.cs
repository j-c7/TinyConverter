using System.Threading.Tasks;
using System;
namespace TinyConverter;

public interface IApp
{
	OutFormats OutFormat { get; set; }

	float JpgQuality { get; set; }

	string OutPath { get; }

	Action<Response> FinalizeTask { get; set; }

	void SetSourcesPath(string[] p_path);

	void SetOutPath(string p_path);

	Task Start();
}