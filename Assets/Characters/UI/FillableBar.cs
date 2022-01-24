using System.Collections;
using Characters.Monsters;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.UI
{
    public class FillableBar : MonoBehaviour
    {
        [SerializeField] private Text currentValueLabel;
        [SerializeField] private Text maximumValueLabel;
        [SerializeField] private Image baseFillImage;
        [SerializeField] private Gradient gradient;

        private float CurrentValue { get; set; }
        private int MaximumValue { get; set; }
        
        public void SetValue(float currentValue, int maximumValue)
        {
            CurrentValue = currentValue;
            MaximumValue = maximumValue;
            currentValueLabel.text = $"{Mathf.Round(CurrentValue)}";
            maximumValueLabel.text = $"{MaximumValue}";
            
            var hpNormalise = CurrentValue / MaximumValue;
            baseFillImage.transform.localScale = new Vector3(hpNormalise, 1f, 1f);
            baseFillImage.color = gradient.Evaluate(hpNormalise);
        }

        public IEnumerator UpdateHealth(int delta, uint updatePercentageSpeed = 100)
        {
            var targetValue = Mathf.Clamp(delta + CurrentValue, 0, MaximumValue);
            var deltaDirection = Mathf.Abs(delta) / delta;
            var deltaStepSize = MaximumValue / 100f;
            
            var updateMultiplier = updatePercentageSpeed / 100f;
            while(Mathf.Abs(targetValue - CurrentValue) > 0)
            {
                var newValue = Mathf.Clamp(CurrentValue + deltaStepSize * deltaDirection, 0, MaximumValue);       
                
                SetValue(newValue, MaximumValue);
                yield return new WaitForSeconds(0.05f / updateMultiplier);
                
            }
        }
    }
}