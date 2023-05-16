using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class OffsetSliderUI : MonoBehaviour
    {
        public float Value => (_slider.value - 0.5f) * _rangeMilliseconds;
        
        [SerializeField] private float _rangeMilliseconds;
        [SerializeField] private TextMeshProUGUI _offsetGraphic;
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Update()
        {
            _offsetGraphic.text = $"{Value:0.00}ms";
        }
    }
}