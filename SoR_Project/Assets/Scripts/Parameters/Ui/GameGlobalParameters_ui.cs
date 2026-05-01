using UnityEngine;
using TMPro;

public class GameGlobalParameters_ui : MonoBehaviour {
	private ParameterController _parameterController;
	private GameInitializer _gameInitializer;
	private ModifierManager _modifierManager;
	private ParametersData _parametersData => GameDataService.Current.parameters;
	[SerializeField] private TextMeshProUGUI _influenceText;
	[SerializeField] private TextMeshProUGUI _totalPopularityText;
	[SerializeField] private TextMeshProUGUI _totalAlignedText;
	[SerializeField] private TextMeshProUGUI _totalAffiliatesText;
	[SerializeField] private TextMeshProUGUI _fameText;
	[SerializeField] private TextMeshProUGUI _determinationText;

	[SerializeField] private TextMeshProUGUI _modifierReportInfluenceText;
	[SerializeField] private TextMeshProUGUI _modifierReportPopularityText;
	[SerializeField] private TextMeshProUGUI _modifierReportAlignedText;
	[SerializeField] private TextMeshProUGUI _modifierReportAffiliatesText;
	[SerializeField] private TextMeshProUGUI _modifierReportFameText;
	[SerializeField] private TextMeshProUGUI _modifierReportDeterminationText;

	private void Start() {
		_parameterController = ServiceLocator.Get<ParameterController>();
		_gameInitializer = ServiceLocator.Get<GameInitializer>();
		_modifierManager = ServiceLocator.Get<ModifierManager>();

		_modifierManager.OnModifiersChanged += UpdateUI;

		if (_gameInitializer.IsInitialized) {
            UpdateUI();
        } else {
            _gameInitializer.OnGameInitialized += UpdateUI;
        }
	}

	private void OnDisable() {
		_modifierManager.OnModifiersChanged -= UpdateUI;
		_gameInitializer.OnGameInitialized -= UpdateUI;
	}

	private void UpdateUI() {
		_influenceText.text = _parametersData.influence.ToString("N0");
		_totalPopularityText.text = _parameterController.totalPopularity.ToString("N0");
		_totalAlignedText.text = (_parameterController.totalAligned / _parameterController.totalPopulation * 100).ToString("F2") + " %";
		_totalAffiliatesText.text = _parameterController.totalAffiliates.ToString("N0");
		_fameText.text = _parametersData.fame.ToString("F2") + "%";
		_determinationText.text = _parametersData.determination.ToString("F2") + "%";

		_modifierReportInfluenceText.text = GetGlobalModifierFinalValue(SOR_Enums.Parameters.Influence);
		_modifierReportFameText.text = GetGlobalModifierFinalValue(SOR_Enums.Parameters.Fame);
		_modifierReportDeterminationText.text = GetGlobalModifierFinalValue(SOR_Enums.Parameters.Determination);
		_modifierReportPopularityText.text = GetProvincialModifierFinalValue(SOR_Enums.Parameters.Popularity);
		_modifierReportAlignedText.text = GetProvincialModifierFinalValue(SOR_Enums.Parameters.Aligned);
		_modifierReportAffiliatesText.text = GetProvincialModifierFinalValue(SOR_Enums.Parameters.Affiliates);
	}

	private string GetGlobalModifierFinalValue(SOR_Enums.Parameters parameter) {
		float finalValue;
		if(_modifierManager.GetReport("Global", parameter) == null) finalValue = 0f;
		else finalValue = _modifierManager.GetReport("Global", parameter).finalValue;
		switch (finalValue) {
			case < 0: return $"- {Mathf.Abs(finalValue):F2}";
			case > 0: return $"+ {finalValue:F2}";
			default: return "0";
		}
	}

	private string GetProvincialModifierFinalValue(SOR_Enums.Parameters parameter) {
		float finalValue;
		finalValue = _modifierManager.GetTotalProvincesParameterValue(parameter);
		switch (finalValue) {
			case < 0: return $"- {Mathf.Abs(finalValue):N0}";
			case > 0: return $"+ {finalValue:N0}";
			default: return "0";
		}

	}
}