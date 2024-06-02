using Godot;
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

    public Action<string> FinalizeTask { get; set; }
    
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

    public async Task<Response> Start()
    {
        Response ret = new();
        foreach (var p in _paths)
        {
            var err = await ConvertImage(p, Path.GetFileName(p).Replace(Path.GetExtension(p), ""));
            Res(err, "Successful" + " " + p, Path.GetFileName(p));
            FinalizeTask.Invoke(Path.GetFileName(p));
        }
        void Res(Error p_error, string p_success_msg, string p_img_name)
        {
            if (p_error != Error.Ok)
            {
                ret.Message = p_error.ToString();
                ret.Success = false;
            }
            else
            {
                ret.Message = p_success_msg;
                ret.Success = true;
            }
        }
        return ret;
    }

    private async Task<Error> ConvertImage(string p_path, string p_file_name)
    {
        Task<Error> task = new(() =>
        {
            Image img = new();
            Error error = img.Load(p_path);
            if (error == Error.Ok)
            {
                Error err = new();
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
            }
            return error;
        });
        task.Start();
        return await task;
    }
}
