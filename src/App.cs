using Godot.Collections;
using System;
using System.Threading.Tasks;
using System.IO;

namespace TinyConverter;

public partial class App : Node, IApp
{
	#region Format

	public OutFormats OutFormat { get; set; } = OutFormats.Jpg;

	private float _jpgQuality = 0.9f;

	public float JpgQuality
	{
		get => _jpgQuality;
		set => _jpgQuality = Mathf.Clamp(value, 0.0f, 1.0f);
	}

	#endregion

	#region Path

	public string OutPath { get => _outPath; }

	private string _outPath;

	private Array<string> _paths = new();

	#endregion

	#region Delegates

	public Action<Response> FinalizeTask { get; set; }

	#endregion

	public void SetSourcesPath(string[] p_path)
	{
		_paths.Clear();
		for (int i = 0; i < p_path.Length; i++)
		{
			_paths.Add(p_path[i]);
		}
	}

	public void SetOutPath(string p_path)
	{
		_outPath = p_path;
	}

	public async Task Start()
	{
		Response ret = new();
		foreach (var p in _paths)
		{
			var err = await ConvertImage(p, Path.GetFileName(p).Replace(Path.GetExtension(p), ""));
			
			FinalizeTask.Invoke(
				Res(err, "Successful" + " " + p, Path.GetFileName(p))
			);
		}
		Response Res(Error p_error, string p_success_msg, string p_img_name)
		{
			if (p_error != Error.Ok)
			{
				ret.FileName = "";
				ret.Message = p_error.ToString();
				ret.Success = false;
			}
			else
			{
				ret.FileName = p_img_name;
				ret.Message = p_success_msg;
				ret.Success = true;
			}
			return ret;
		}
	}

	private async Task<Error> ConvertImage(string p_path, string p_file_name)
	{
		Image img = new();
		Error err = new();
		Task<Error> task = new(() =>
		{
			Error error = img.Load(p_path);
			if (error == Error.Ok)
			{
				switch (OutFormat)
				{
					case OutFormats.Jpg:
						err = img.SaveJpg(_outPath + "/" + p_file_name + ".jpg", _jpgQuality);
						break;

					case OutFormats.Jpeg:
						err = img.SaveJpg(_outPath + "/" + p_file_name + ".jpeg", _jpgQuality);
						break;

					case OutFormats.Png:
						err = img.SavePng(_outPath + "/" + p_file_name + ".png");
						break;
				}
				if (err != Error.Ok)
					error = err;

				img.Dispose();
			}
			return error;
		});
		task.Start();
		return await task;
	}
}
