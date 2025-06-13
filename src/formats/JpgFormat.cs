using System.Globalization;
using System.Threading.Tasks;
using TinyConverter.Core;

namespace TinyConverter.Formats;

public partial class JpgFormat : FormatterNode
{
    private float _quality = 0.9f;
    private readonly string _formatName = ".jpg";
    public override string FormatName => _formatName;

    public override void SetQualityParams(params string[] p_params)
    {
        float q = float.Parse(p_params[0], CultureInfo.InvariantCulture);
        _quality = Mathf.Clamp(q, 0.0f, 1.0f);
    }

    public override async Task<Error> ConvertImage(string p_loadPath, string p_outPath, string p_fileName) =>
        await ConvertProcess(p_loadPath, img =>
            img.SaveJpg(GetFinalOutPath(p_outPath, p_fileName, _formatName), _quality));
}