using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace GameOfLife
{
    public class UiPanel : MonoBehaviour
    {
        [SerializeField] private GameObject minimizedPanel;
        [SerializeField] private GameObject maximizedPanel;
        [SerializeField] private Button minimizeButton;
        [SerializeField] private Button maximizeButton;
        [SerializeField] private TMP_Text widthText;
        [SerializeField] private Slider widthSlider;
        [SerializeField] private TMP_Text heightText;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private TMP_Text fullnessText;
        [SerializeField] private Slider fullnessSlider;
        [SerializeField] private Button generateButton;
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text startButtonText;

        private GridBuildingSystem _gridBuildingSystem;
        private GameManager _gameManager;
        private string _widthPrefix, _heightPrefix, _fullnessPrefix;
        
        private void Awake()
        {
            _widthPrefix = widthText.text;
            _heightPrefix = heightText.text;
            _fullnessPrefix = fullnessText.text;
            
            var world = World.DefaultGameObjectInjectionWorld;
            _gridBuildingSystem = world.GetOrCreateSystem<GridBuildingSystem>();
            _gameManager = world.GetOrCreateSystem<GameManager>();
            var gridSize = _gridBuildingSystem.GridSize;
            
            widthSlider.minValue = Grid.MIN_SIZE;
            widthSlider.maxValue = Grid.MAX_SIZE;
            widthSlider.onValueChanged.AddListener(value =>
            {
                UpdateSliderText(value, widthText, _widthPrefix, 0);
            });
            
            widthSlider.value = gridSize.x;

            heightSlider.minValue = Grid.MIN_SIZE;
            heightSlider.maxValue = Grid.MAX_SIZE;
            heightSlider.onValueChanged.AddListener(value =>
            {
                UpdateSliderText(value, heightText, _heightPrefix, 0);
            });
            
            heightSlider.value = gridSize.y;
            
            fullnessSlider.onValueChanged.AddListener(value =>
            {
                UpdateSliderText(value, fullnessText, _fullnessPrefix, 2);
            });
            
            fullnessSlider.value = 0.5f;

            generateButton.onClick.AddListener(OnGenerateButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
            minimizeButton.onClick.AddListener(() => MinimizePanel(true));
            maximizeButton.onClick.AddListener(()=> MinimizePanel(false));
        }

        private void OnEnable()
        {
            UpdatePanel(_gameManager.IsSimulationOn);
        }

        private void OnStartButtonClicked()
        {
            var isPlayModeOn = !_gameManager.IsSimulationOn;
            _gameManager.IsSimulationOn = isPlayModeOn;
            UpdatePanel(isPlayModeOn);
        }

        private void OnGenerateButtonClicked()
        {
            var gridSize = new int2((int)math.round(widthSlider.value), (int)math.round(heightSlider.value));

            _gridBuildingSystem.GridSize = gridSize;
            _gridBuildingSystem.Fullness = fullnessSlider.value;
            _gridBuildingSystem.BuildGrid();
        }

        private void UpdatePanel(bool isPlayModeOn)
        {
            generateButton.interactable = !isPlayModeOn;
            widthSlider.interactable = !isPlayModeOn;
            heightSlider.interactable = !isPlayModeOn;
            startButtonText.SetText(isPlayModeOn ? "Stop" : "Start");
        }

        private static void UpdateSliderText(float value, TMP_Text text, string prefix, int signCount)
        {
            var multiplier = math.pow(10, signCount);
            value = math.round(value * multiplier) / multiplier;

            text.SetText($"{prefix}: {value}");
        }

        private void MinimizePanel(bool value)
        {
            minimizedPanel.SetActive(value);
            maximizedPanel.SetActive(!value);
        }
    }
}
