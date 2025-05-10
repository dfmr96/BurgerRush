using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetData(string label, string value)
    {
        labelText.text = label;
        valueText.text = value;
    }
}
