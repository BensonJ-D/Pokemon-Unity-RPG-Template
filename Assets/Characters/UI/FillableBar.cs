using System;
using System.Collections;
using Characters.Monsters;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.UI
{
    [Serializable]
    public class FillableBar
    {
        [SerializeField] private Text currentValueLabel;
        [SerializeField] private Text maximumValueLabel;
        [SerializeField] private Image baseFillImage;
        [SerializeField] private Gradient gradient;

        private int MinimumValue { get; set; }
        private float CurrentValue { get; set; }
        private int MaximumValue { get; set; }

        public void SetValue(int newValue) => SetValue(MinimumValue, newValue, MaximumValue);
        public void SetValue(float currentValue, int maximumValue) => SetValue(0, currentValue, maximumValue);

        public void SetValue(int minimumValue, float currentValue, int maximumValue)
        {
            MinimumValue = minimumValue;
            CurrentValue = currentValue;
            MaximumValue = maximumValue;
            if (currentValueLabel) currentValueLabel.text = $"{Mathf.Round(CurrentValue)}";
            if (maximumValueLabel) maximumValueLabel.text = $"{MaximumValue}";

            var valueNormalised = (CurrentValue - MinimumValue) / (MaximumValue - MinimumValue);
            
            if (!baseFillImage) return;
            
            baseFillImage.transform.localScale = new Vector3(valueNormalised, 1f, 1f);
            baseFillImage.color = gradient.Evaluate(valueNormalised);
        }

        public IEnumerator UpdateBar(int delta, uint updatePercentageSpeed = 100)
        {
            var targetValue = Mathf.Clamp(delta + CurrentValue, MinimumValue, MaximumValue);
            var isPositiveDelta = delta > 0;
            var deltaStepSize = (MaximumValue - MinimumValue) / 100f;
            
            var updateMultiplier = updatePercentageSpeed / 100f;
            while(Mathf.Abs(targetValue - CurrentValue) > 0)
            {
                var newValue =  isPositiveDelta ?
                    Mathf.Clamp(CurrentValue + deltaStepSize, MinimumValue, targetValue) :
                    Mathf.Clamp(CurrentValue - deltaStepSize, targetValue, MaximumValue);       
                
                SetValue(MinimumValue, newValue, MaximumValue);
                yield return new WaitForSeconds(0.05f / updateMultiplier);
                
            }
        }
    }
}