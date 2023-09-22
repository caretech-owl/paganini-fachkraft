using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLabeledPrefab : MonoBehaviour
{

    public GameObject InactiveLabel;
    public GameObject ActiveLabel;

    private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        ToggleActive(toggle.isOn);

        toggle.onValueChanged.AddListener(ToggleActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleActive(bool active)
    {
        InactiveLabel.SetActive(!active);
        ActiveLabel.SetActive(active);
    }
}
