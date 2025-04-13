using System;
using System.Threading.Tasks;

namespace TinyConverter.Core;

public partial class FormatterNode : Node, IFormatterNode
{
	public virtual string FormatName { get; }

	public virtual Task<Error> ConvertImage(string p_loadPath, string p_outPath, string p_fileName) => default;
	
	public virtual void SetQualityParams(params string[] p_params) { }

	protected static async Task<Error> ConvertProcess(string p_loadPath, Func<Image, Error> p_predicate)
	{
		Image img = new();
		Error err = new();
		Task<Error> task = new(() =>
		{
			Error error = img.Load(p_loadPath);
			if (error == Error.Ok)
			{
				err = p_predicate.Invoke(img);
				if (err != Error.Ok)
					error = err;

				img.Dispose();
			}
			return error;
		});
		task.Start();
		return await task;
	}

	protected static string GetFinalOutPath(string p_outPath, string p_fileName)
	{
		return string.Concat(p_outPath, "/", p_fileName);
	}
}