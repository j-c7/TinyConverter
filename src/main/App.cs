using System;
using System.Threading.Tasks;
using TinyConverter.Core;
using Godot.Collections;
using System.Linq;
using System.IO;

namespace TinyConverter.Main;

public partial class App : Node, IApp
{
    private string _outFormat;
    public string OutFormat
    {
        get => _outFormat;
        set
        {
            _outFormat = value;
            _formatterNode = this.GetChildByType<IFormatterNode>((n) => n.FormatName == _outFormat);
        }
    }

    public string OutPath { get; set; }

    private Array<string> _paths = [];

    public Action<Result> FinalizeTask { get; set; }

    private IFormatterNode _formatterNode;

    public void SetQualityParams(params string[] p_params)
    {
        if (_formatterNode is null)
            return;

        _formatterNode.SetQualityParams(p_params);
    }

    public void SetSourcesPath(string[] p_path)
    {
        _paths.Clear();
        for (int i = 0; i < p_path.Length; i++)
        {
            _paths.Add(p_path[i]);
        }
    }

    public async Task Start()
    {
        if (_formatterNode is null)
        {
            Result ret = new()
            {
                FileName = "",
                Message = "Formatter Node not Found",
                Success = false
            };
            FinalizeTask.Invoke(ret);
            return;
        }

        foreach (var p in _paths)
        {
            var err = await _formatterNode.ConvertImage(p, OutPath, Path.GetFileName(p).Replace(Path.GetExtension(p), ""));
            FinalizeTask.Invoke(
               Res(err, "Successful" + " " + p, Path.GetFileName(p))
            );
        }

        Result Res(Error p_error, string p_successMsg, string p_imgName)
        {
            Result ret = new();
            if (p_error != Error.Ok)
            {
                ret.FileName = "";
                ret.Message = p_error.ToString();
                ret.Success = false;
            }
            else
            {
                ret.FileName = p_imgName;
                ret.Message = p_successMsg;
                ret.Success = true;
            }
            return ret;
        }
    }
}