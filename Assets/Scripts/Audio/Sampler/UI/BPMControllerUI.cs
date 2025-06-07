using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SamplerQuest.Audio.Sampler
{
    public class BPMControllerUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BPMController bpmController;
        [SerializeField] private Slider bpmSlider;
        [SerializeField] private TMP_Text bpmText;
        [SerializeField] private Toggle smoothTransitionToggle;
        [SerializeField] private TMP_InputField transitionTimeInput;

        private void Start()
        {
            if (bpmController == null)
            {
                Debug.LogError("BPMController reference is missing!");
                return;
            }

            // Initialize UI
            bpmSlider.minValue = 20f;
            bpmSlider.maxValue = 300f;
            bpmSlider.value = bpmController.GetCurrentBPM();
            UpdateBPMText(bpmSlider.value);

            // Add listeners
            bpmSlider.onValueChanged.AddListener(OnBPMChanged);
            smoothTransitionToggle.onValueChanged.AddListener(OnSmoothTransitionChanged);
            transitionTimeInput.onEndEdit.AddListener(OnTransitionTimeChanged);

            // Initialize toggle and input field
            smoothTransitionToggle.isOn = false;
            transitionTimeInput.text = "1.0";
        }

        private void OnBPMChanged(float value)
        {
            bpmController.SetBPM(value);
            UpdateBPMText(value);
        }

        private void OnSmoothTransitionChanged(bool value)
        {
            // You can add additional logic here if needed
        }

        private void OnTransitionTimeChanged(string value)
        {
            if (float.TryParse(value, out float time))
            {
                // You can add additional logic here if needed
            }
        }

        private void UpdateBPMText(float bpm)
        {
            if (bpmText != null)
            {
                bpmText.text = $"BPM: {bpm:F1}";
            }
        }
    }
} 