using UnityEngine;
using TMPro;

public class GameGlobalParameters_ui : MonoBehaviour {
	private ParameterController _parameterController;
	private GameInitializer _gameInitializer;
	private ParametersData _parametersData => GameDataService.Current.parameters;
	[SerializeField] private TextMeshProUGUI _influenceText;
	[SerializeField] private TextMeshProUGUI _totalPopularityText;
	[SerializeField] private TextMeshProUGUI _totalAlignedText;
	[SerializeField] private TextMeshProUGUI _totalAffiliatesText;
	[SerializeField] private TextMeshProUGUI _fameText;
	[SerializeField] private TextMeshProUGUI _determinationText;

	private void Start() {
		_parameterController = ServiceLocator.Get<ParameterController>();
	}

	private void Update() {
		UpdateUI();
	}

	private void UpdateUI() {
		_influenceText.text = _parametersData.influence.ToString("N0");
		_totalPopularityText.text = (_parameterController.totalPopularity / _parameterController.totalPopulation * 100).ToString("F2") + " %";
		_totalAlignedText.text = _parameterController.totalAligned.ToString("N0");
		_totalAffiliatesText.text = _parameterController.totalAffiliates.ToString("N0");
		_fameText.text = _parametersData.fame.ToString("F2") + "%";
		_determinationText.text = _parametersData.determination.ToString("F2") + "%";
	}

}