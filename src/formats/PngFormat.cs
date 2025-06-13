using System.Threading.Tasks;
using TinyConverter.Core;

namespace TinyConverter.Formats;

public partial class PngFormat : FormatterNode
{
    private readonly string _formatName = ".png";
    public override string FormatName => _formatName;

    public override async Task<Error> ConvertImage(string p_loadPath, string p_outPath, string p_fileName) => 
        await ConvertProcess(p_loadPath, img => 
            img.SavePng(GetFinalOutPath(p_outPath, p_fileName, _formatName)));
}
