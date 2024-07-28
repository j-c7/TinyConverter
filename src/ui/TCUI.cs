using Godot.Collections;
using System.Globalization;
using System.IO;

namespace TinyConverter;

public partial class TCUI : UIBase
{
	#region APP

	[Export]
	public NodePath AppPath { get; set; }

	private IApp _app;

	#endregion

	#region FormatMode

	[ExportGroup("Modes")]

	[Export]
	[AssertNode("")]
	public OptionButton FormatModeOptionButton = null;

	readonly Array<StringName> OutFormatList = new()
	{
		"Jpg",
		"Jpeg",
		"Png"
	};

	public OutFormats _outFormats;

	[Export]
	[AssertNode("")]
	public LineEdit Quality = null;

	#endregion

	[ExportGroup("Nodes")]

	#region ImportPath

	[Export]
	[AssertNode("")]
	public Button ImportButton = null;

	[Export]
	[AssertNode("")]
	public Label ImportLabel = null;

	[Export]
	[AssertNode("")]
	public FileDialog ImportFileDialog = null;

	#endregion

	#region ExportPath

	[Export]
	[AssertNode("")]
	public Button ExportButton = null;

	[Export]
	[AssertNode("")]
	public Label ExportLabel = null;

	[Export]
	[AssertNode("")]
	public FileDialog ExportFileDialog = null;

	#endregion

	#region StartPath

	[Export]
	[AssertNode("")]
	public Button StartButton = null;

	private bool _lockStartButton = false;

	#endregion

	#region Error

	[Export]
	[AssertNode("")]
	public PanelContainer ErrorPanel = null;

	[Export]
	[AssertNode("")]
	public Label ErrorMessage = null;

	[Export]
	[AssertNode("")]
	public Button CloseErrorButton = null;

	#endregion

	#region Success

	[Export]
	[AssertNode("")]
	public ItemList SuccessItemList = null;

	[Export]
	[AssertNode("")]
	public TextureRect LoadingTex = null;

	[Export]
	[AssertNode("")]
	public TextureRect SuccessTex = null;

	#endregion

	public override void _EnterTree()
	{
		base._EnterTree();
		_app = GetNodeOrNull(AppPath) as IApp;
		AssertNode(_app, "App node not found");
		
		InitializeFormatModes();
		FormatModeOptionButton.ItemSelected += OnFormatModeSelected;
		ImportButton.ButtonDown += OnPressImportButton;
		ImportFileDialog.FilesSelected += OnImportFileSelected;
		ExportButton.ButtonDown += OnPressExportButton;
		ExportFileDialog.DirSelected += OnExportDirSelected;
		StartButton.ButtonDown += OnStartButton;
		Quality.TextChanged += OnQualityChanged;
		CloseErrorButton.ButtonDown += OnCloseErrorButton;

		_app.FinalizeTask += OnFinalizeTask;
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
		CloseErrorButton.ButtonDown -= OnCloseErrorButton;

		_app.FinalizeTask -= OnFinalizeTask;
	}

	private void InitializeFormatModes()
	{
		foreach (var item in OutFormatList)
			FormatModeOptionButton.AddItem(item);
	}

	private void OnFormatModeSelected(long p_index)
	{
		switch (p_index)
		{
			//to Jpg
			case 0:
				_outFormats = OutFormats.Jpg;
				Quality.Editable = true;
				break;

			// to JPeg
			case 1:
				_outFormats = OutFormats.Jpeg;
				Quality.Editable = true;
				break;

			// to Png
			case 2:
				_outFormats = OutFormats.Png;
				Quality.Editable = false;
				break;
		}
		_app.OutFormat = _outFormats;
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
		_app.SetOutPath(p_path);
	}

	private async void OnStartButton()
	{
		if(_lockStartButton)
			return;
		
		_lockStartButton = true;
		if (ImportLabel.Text == "" || ImportLabel.Text == "...")
		{
			ErrorPanel.Visible = true;
			ErrorMessage.Text = "Unassigned import path";

			return;
		}

		if (ExportLabel.Text == "" || ExportLabel.Text == "...")
		{
			ErrorPanel.Visible = true;
			ErrorMessage.Text = "unassigned export path";

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
		float quality = float.Parse(p_text, CultureInfo.InvariantCulture);
		quality = Mathf.Clamp(quality, 0.0f, 1.0f);
		_app.JpgQuality = quality;
	}

	private void OnCloseErrorButton()
	{
		ErrorPanel.Visible = false;
	}

	private void OnFinalizeTask(Response p_response)
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