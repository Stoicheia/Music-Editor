using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BeatSnapDivisorUI : MonoBehaviour
    {
        public event Action<int> OnChangeDivs;
        public int DivsPerBeat => _divsPerBeat;
        
        [Header("Elements")] 
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _divisorValueField;
        [Header("Settings")] 
        [SerializeField] private List<int> _allowedDivisors;
        [SerializeField] private int _initialSliderIndex = 0;
        [SerializeField] private bool _invertValueString = true;

        private int _divsPerBeat;
        private void Start()
        {
            _slider.wholeNumbers = true;
            _slider.minValue = 0;
            _slider.maxValue = _allowedDivisors.Count - 1;
            
            _slider.onValueChanged.AddListener(value =>
            {
                SetValue((int)(value + 0.001f));
            });

            _slider.value = _initialSliderIndex;
            SetValue(_initialSliderIndex);
        }

        private void SetValue(int index)
        {
            _divsPerBeat = _allowedDivisors[index];
            _divisorValueField.text = _invertValueString ? $"1/{_divsPerBeat}" : $"{_divsPerBeat}";
            OnChangeDivs?.Invoke(_divsPerBeat);
        }
    }
}