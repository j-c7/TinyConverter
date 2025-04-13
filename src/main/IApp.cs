using System;
using TinyConverter.Core;
using System.Threading.Tasks;

namespace TinyConverter.Main;

public interface IApp
{
    string OutFormat { get; set; }

    string OutPath { get; set; }

	Action<Result> FinalizeTask { get; set; }

    void SetQualityParams(params string[] p_params);

	void SetSourcesPath(string[] p_path);

	Task Start();
}