using System;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;

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

	protected static string GetFinalOutPath(string p_outPath, string p_fileName, string p_formatName)
	{
		string path = string.Concat(p_outPath, "/", p_fileName, p_formatName);
		if (File.Exists(path))
		{
			// Añadimos un un string aleatorio al nombre del fichero.
			path = string.Concat(p_outPath, "/", string.Concat(p_fileName, GenerateRandomString(3)), p_formatName);
		}
		return path;
	}

	static string GenerateRandomString(int length)
	{
		var random = new Random();
		char[] buffer = new char[length];
		const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

		for (int i = 0; i < length; i++)
		{
			buffer[i] = chars[random.Next(chars.Length)];
		}
		return new string(buffer);
	}
}