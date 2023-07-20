using Godot;
using System;
using Godot.Collections;
using NAudio.Wave;
using SonicThinking.scripts.autoload;
using SonicThinking.scripts.helpers;
using SonicThinking.scripts.nodes;
using SonicThinking.scripts.sample_providers;

public partial class NAEdgeDetectorNode : NANode
{
	public override void _Ready()
	{
		base._Ready();

		_modeOptions = GetNode<OptionButton>("Mode/Options");
		_modeOptions.ItemSelected += index => _detector.Mode = (EdgeDetectorProvider.TriggerMode)index;
		_detector.Mode = (EdgeDetectorProvider.TriggerMode)_modeOptions.Selected;
		
		_thresholdSlider = GetNode<SliderInput>("Threshold");
		_thresholdSlider.ValueChanged += value => _constantThreshold.Value = (float)value;
		_constantThreshold.Value = (float)_thresholdSlider.Value;

		InputChanged += OnInputChanged;

		Compositor.ForceCache += _cache.Force;
		Compositor.ClearCache += _cache.Clear;
	}

	public override void _ExitTree()
	{
		Compositor.ForceCache -= _cache.Force;
		Compositor.ClearCache -= _cache.Clear;
	}


	private void OnInputChanged(NANode sender, int portIndex, ISampleProvider input)
	{
		switch (portIndex)
		{
			case 0: _source.Source = input;
				break;
			case 1:
				_threshold.Source = input;
				_thresholdSlider.Editable = input == null;
				break;
			default:
				throw new ArgumentException("Invalid input port.");
		}
	}

	// Providers
	private readonly ConstantSampleProvider _constantThreshold = new ConstantSampleProvider();
	private readonly RebindingProvider _source = new RebindingProvider();
	private readonly PrioritySampleProvider _threshold;
	private readonly EdgeDetectorProvider _detector;
	private readonly CachingSampleProvider _cache;

	// Nodes
	private OptionButton _modeOptions;
	private SliderInput _thresholdSlider;

	public NAEdgeDetectorNode()
	{
		_threshold = new PrioritySampleProvider(_constantThreshold);
		_detector = new EdgeDetectorProvider(_source, _threshold);
		_cache = new CachingSampleProvider(_detector);
	}

	public override Dictionary Serialize()
	{
		return new Dictionary()
		{
			{ "threshold", _thresholdSlider.Value },
			{ "mode", _modeOptions.Selected },
		};
	}

	public override void Deserialize(Dictionary state)
	{
		_thresholdSlider.Value = state["threshold"].AsDouble();
		_modeOptions.Selected = state["mode"].AsInt32();
	}

	protected override ISampleProvider GetOutput(int port) => _cache;
}
