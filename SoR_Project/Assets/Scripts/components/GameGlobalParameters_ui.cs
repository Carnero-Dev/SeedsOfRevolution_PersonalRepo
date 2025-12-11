using UnityEngine;
using TMPro;

public class GameGlobalParameters_ui : MonoBehaviour {
	private ParameterController _parameterController;
	private ParametersData _parametersData => GameDataService.Current.parameters;
	[SerializeField] private TextMeshProUGUI _influenceText;
	[SerializeField] private TextMeshProUGUI _totalPopularityText;
	[SerializeField] private TextMeshProUGUI _totalAlignedText;
	[SerializeField] private TextMeshProUGUI _totalAffiliatesText;
	[SerializeField] private TextMeshProUGUI _fameText;
	[SerializeField] private TextMeshProUGUI _determinationText;

	private void Start() {
		_parameterController = ServiceLocator.Get<ParameterController>();

		_parameterController.OnParametersUpdated += UpdateUI;
		UpdateUI();
	}

	private void UpdateUI() {
		_influenceText.text = _parametersData.influence.ToString("F2");
		_totalPopularityText.text = (_parameterController.totalPopularity / _parameterController.totalPopulation * 100).ToString("F2") + " %";
		_totalAlignedText.text = _parameterController.totalAligned.ToString("F2");
		_totalAffiliatesText.text = _parameterController.totalAffiliates.ToString("F2");
		_fameText.text = _parametersData.fame.ToString("F2") + "%";
		_determinationText.text = _parametersData.determination.ToString("F2") + "%";
	}

}