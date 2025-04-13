using System.Globalization;
using System.IO;
using Godot.Collections;
using TinyConverter.Core;
using TinyConverter.Main;

namespace TinyConverter.UI;

public partial class UIManager : Node
{
    #region App
    [Export]
    public NodePath AppPath { get; set; }

    private IApp _app;
    #endregion

    #region Formats
    [ExportGroup("Modes")]

    [Export]
    [AssertNode()]
    public OptionButton FormatModeOptionButton = null;

    readonly Array<StringName> OutFormatList =
    [
        ".jpg",
        ".png",
        ".webp"
    ];

    public string _outFormat;

    [Export]
    [AssertNode()]
    public LineEdit Quality = null;

    [Export]
    [AssertNode()]
    public CheckBox Lossy = null;

    Dictionary<string, string> _qualityParams = new(){
        {"quality", "0.9"},
        {"lossy", "false"}
    };

    #endregion

    [ExportGroup("Nodes")]

    #region ImportPath

    [Export]
    [AssertNode()]
    public Button ImportButton = null;

    [Export]
    [AssertNode()]
    public Label ImportLabel = null;

    [Export]
    [AssertNode()]
    public FileDialog ImportFileDialog = null;

    #endregion

    #region ExportPath

    [Export]
    [AssertNode()]
    public Button ExportButton = null;

    [Export]
    [AssertNode()]
    public Label ExportLabel = null;

    [Export]
    [AssertNode()]
    public FileDialog ExportFileDialog = null;

    #endregion

    #region StartPath

    [Export]
    [AssertNode()]
    public Button StartButton = null;

    private bool _lockStartButton = false;

    #endregion

    #region Error

    [Export]
    [AssertNode()]
    public PanelContainer ErrorPanel = null;

    [Export]
    [AssertNode()]
    public Label ErrorMessage = null;

    [Export]
    [AssertNode()]
    public Button CloseErrorButton = null;

    #endregion

    #region Success

    [Export]
    [AssertNode()]
    public ItemList SuccessItemList = null;

    [Export]
    [AssertNode()]
    public TextureRect LoadingTex = null;

    [Export]
    [AssertNode()]
    public TextureRect SuccessTex = null;

    #endregion

    public override void _EnterTree()
    {
        _app = GetNodeOrNull(AppPath) as IApp;
        this.AssertNode(GetTree(), "App node not found");
        InitializeFormatModes();
        FormatModeOptionButton.ItemSelected += OnFormatModeSelected;
        ImportButton.ButtonDown += OnPressImportButton;
        ImportFileDialog.FilesSelected += OnImportFileSelected;
        ExportButton.ButtonDown += OnPressExportButton;
        ExportFileDialog.DirSelected += OnExportDirSelected;
        StartButton.ButtonDown += OnStartButton;
        Quality.TextChanged += OnQualityChanged;
        Lossy.Pressed += OnLossyPressed;
        CloseErrorButton.ButtonDown += OnCloseErrorButton;

        _app.FinalizeTask += OnFinalizeTask;

        // Initlialize Quality
        OnQualityChanged(Quality.Text);
        OnLossyPressed();

        OnFormatModeSelected(FormatModeOptionButton.Selected);
    }

    public override void _ExitTree()
    {
        FormatModeOptionButton.ItemSelected -= OnFormatModeSelected;
        ImportButton.ButtonDown -= OnPressImportButton;
        ImportFileDialog.FilesSelected -= OnImportFileSelected;
        ExportButton.ButtonDown -= OnPressExportButton;
        ExportFileDialog.DirSelected -= OnExportDirSelected;
        StartButton.ButtonDown -= OnStartButton;
        Quality.TextChanged -= OnQualityChanged;
        Lossy.Pressed -= OnLossyPressed;
        CloseErrorButton.ButtonDown -= OnCloseErrorButton;

        _app.FinalizeTask -= OnFinalizeTask;
    }

    private void InitializeFormatModes()
    {
        OutFormatList.ForEach(item => FormatModeOptionButton.AddItem(item));
    }

    private void OnFormatModeSelected(long p_index)
    {
        Lossy.Disabled = true;
        switch (p_index)
        {
            // Jpg
            case 0:
                _outFormat = OutFormatList[0];
                Quality.Editable = true;
                break;

            // Png
            case 1:
                _outFormat = OutFormatList[1];
                Quality.Editable = false;
                break;

            // Webp
            case 2:
                _outFormat = OutFormatList[2];
                Quality.Editable = true;
                Lossy.Disabled = false;
                break;
        }
        _app.OutFormat = _outFormat;
    }

    private void OnPressImportButton()
    {
        ImportFileDialog.Visible = true;
    }

    private void OnImportFileSelected(string[] p_path)
    {
        ImportLabel.Text = p_path[0].Replace(Path.GetFileName(p_path[0]), "");
        _app.SetSourcesPath(p_path);
    }

    private void OnPressExportButton()
    {
        ExportFileDialog.Visible = true;
    }

    private void OnExportDirSelected(string p_path)
    {
        ExportLabel.Text = p_path;
        _app.OutPath = p_path;
    }

    private async void OnStartButton()
    {
        if (_lockStartButton)
            return;

        _lockStartButton = true;
        if (ImportLabel.Text == "" || ImportLabel.Text == "...")
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text = "Unassigned import path";
            _lockStartButton = false;
            return;
        }

        if (ExportLabel.Text == "" || ExportLabel.Text == "...")
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text = "unassigned export path";
            _lockStartButton = false;
            return;
        }
        SuccessTex.Visible = false;
        LoadingTex.Visible = true;
        SuccessItemList?.Clear();

        await _app.Start();
        LoadingTex.Visible = false;
        SuccessTex.Visible = true;
        _lockStartButton = false;
    }

    private void OnQualityChanged(string p_text)
    {
        _qualityParams["quality"] = p_text;
        OnQualityChanged();
    }

    private void OnLossyPressed()
    {
        string lossy = Lossy.ButtonPressed.ToString();
        _qualityParams["lossy"] = lossy;
        OnQualityChanged();
    }

    private void OnQualityChanged()
    {
        _app.SetQualityParams(_qualityParams["quality"], _qualityParams["lossy"]);
    }

    private void OnCloseErrorButton()
    {
        ErrorPanel.Visible = false;
    }

    private void OnFinalizeTask(Result p_response)
    {
        if (!p_response.Success)
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text = p_response.Message;
            return;
        }
        SuccessItemList?.AddItem(p_response.FileName);
    }
}