using Godot;
using Godot.Collections;
using System;
using System.Globalization;
using System.IO;

namespace TinyConverter;

public partial class TCUI : Control
{
	#region APP

	[Export]
	public NodePath AppPath { get; set; }

	private IApp _app;

	#endregion

	#region FormatMode

	[ExportGroup("Modes")]

	[Export]
	public OptionButton FormatModeOptionButton = null;

	readonly Array<StringName> OutFormatList = new()
	{
		"Jpg",
		"Jpeg",
		"Png"
	};

	public OutFormats _outFormats;

	[Export]
	public LineEdit Quality = null;

	#endregion

	[ExportGroup("Paths")]

	#region ImportPath

	[Export]
	public Button ImportButton = null;

	[Export]
	public Label ImportLabel = null;

	[Export]
	public FileDialog ImportFileDialog = null;

	#endregion

	#region ExportPath

	[Export]
	public Button ExportButton = null;

	[Export]
	public Label ExportLabel = null;

	[Export]
	public FileDialog ExportFileDialog = null;

	#endregion

	#region StartPath

	[Export]
	public Button StartButton = null;

	private bool _lockStartButton = false;

	#endregion

	#region Error

	[Export]
	public PanelContainer ErrorPanel = null;

	[Export]
	public Label ErrorMessage = null;

	[Export]
	public Button CloseErrorButton = null;

	#endregion

	#region Success

	[Export]
	public ItemList SuccessItemList = null;

	[Export]
	public TextureRect LoadingTex = null;

	[Export]
	public TextureRect SuccessTex = null;

	#endregion

	public override void _EnterTree()
	{
		_app = GetNodeOrNull(AppPath) as IApp;
		if (_app is null)
		{
			GD.PrintErr("App not found");
			GetTree().Quit();
		}

		InitializeFormatModes();
		if (FormatModeOptionButton is not null)
			FormatModeOptionButton.ItemSelected += OnFormatModeSelected;

		if (ImportButton is not null)
			ImportButton.ButtonDown += OnPressImportButton;

		if (ImportFileDialog is not null)
			ImportFileDialog.FilesSelected += OnImportFileSelected;

		if (ExportButton is not null)
			ExportButton.ButtonDown += OnPressExportButton;

		if (ExportFileDialog is not null)
			ExportFileDialog.DirSelected += OnExportDirSelected;

		if (StartButton is not null)
			StartButton.ButtonDown += OnStartButton;

		if (Quality is not null)
			Quality.TextChanged += OnQualityChanged;

		if (CloseErrorButton is not null)
			CloseErrorButton.ButtonDown += OnCloseErrorButton;

		_app.FinalizeTask += OnFinalizeTask;
	}

	public override void _ExitTree()
	{
		if (FormatModeOptionButton is not null)
			FormatModeOptionButton.ItemSelected -= OnFormatModeSelected;

		if (ImportButton is not null)
			ImportButton.ButtonDown -= OnPressImportButton;

		if (ImportFileDialog is not null)
			ImportFileDialog.FilesSelected -= OnImportFileSelected;

		if (ExportButton is not null)
			ExportButton.ButtonDown -= OnPressExportButton;

		if (ExportFileDialog is not null)
			ExportFileDialog.DirSelected -= OnExportDirSelected;

		if (StartButton is not null)
			StartButton.ButtonDown -= OnStartButton;

		if (Quality is not null)
			Quality.TextChanged -= OnQualityChanged;

		if (CloseErrorButton is not null)
			CloseErrorButton.ButtonDown -= OnCloseErrorButton;

		_app.FinalizeTask -= OnFinalizeTask;
	}

	private void InitializeFormatModes()
	{
		if (FormatModeOptionButton is null) return;
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
			if (ErrorPanel is not null)
				ErrorPanel.Visible = true;

			if (ErrorMessage is not null)
				ErrorMessage.Text = "Unassigned import path";

			return;
		}

		if (ExportLabel.Text == "" || ExportLabel.Text == "...")
		{
			if (ErrorPanel is not null)
				ErrorPanel.Visible = true;

			if (ErrorMessage is not null)
				ErrorMessage.Text = "unassigned export path";

			return;
		}
		SuccessTex.Visible = false;
		LoadingTex.Visible = true;
		SuccessItemList?.Clear();

		var s = await _app.Start();
		LoadingTex.Visible = false;
		if (!s.Success)
		{
			if (ErrorPanel is not null)
				ErrorPanel.Visible = true;

			if (ErrorMessage is not null)
				ErrorMessage.Text = s.Message;

			return;
		}
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

	private void OnFinalizeTask(string p_name)
	{
		SuccessItemList?.AddItem(p_name);
	}
}