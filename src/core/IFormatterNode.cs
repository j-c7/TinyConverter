using System;
using System.Threading.Tasks;

namespace TinyConverter.Core;

public interface IFormatterNode
{
    string FormatName { get; }

    void SetQualityParams(params string[] p_params);
    
    Task<Error> ConvertImage(string p_loadPath, string p_outPath, string p_fileName);
}