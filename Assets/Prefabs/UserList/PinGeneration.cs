using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinGeneration : MonoBehaviour
{

    public GameObject CodeWrapper;
    public TMPro.TMP_Text ErrorText;
    public ButtonPrefab GenerateButton;
    private TMPro.TMP_Text[] PinElements;

    // Start is called before the first frame update
    void Start()
    {
        // Find and cache the TextMeshPro components inside CodeWrapper
        PinElements = CodeWrapper.GetComponentsInChildren<TMPro.TMP_Text>();
        ErrorText.gameObject.SetActive(false);        

        Clearlist();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePIN()
    {
        PaganiniRestAPI.User.GeneratePIN(AppState.CurrentUser.Id, GeneratePinSucceeded, GeneratePinFailed);
        GenerateButton.RenderBusyState(true);        
    }

    public void DisplayPIN(string pin)
    {
        if (pin.Length != PinElements.Length)
        {
            Debug.LogError("PIN length doesn't match the number of PinElements.");
            return;
        }

        // Set each TextMeshPro element to the corresponding character in the PIN
        for (int i = 0; i < pin.Length; i++)
        {
            PinElements[i].text = pin[i].ToString();
        }
        
    }

    public void Clearlist()
    {
        foreach (var element in PinElements)
        {
            element.text = "-"; // or any placeholder you want when clearing
        }
    }

    private void GeneratePinSucceeded(UserPinAPI userPin)
    {
        DisplayPIN(userPin.pin);
        GenerateButton.RenderBusyState(false);
        ErrorText.gameObject.SetActive(false);
    }

    private void GeneratePinFailed(string error)
    {
        Debug.LogError("Failed to generate PIN: " + error);
        GenerateButton.RenderBusyState(false);
        ErrorText.gameObject.SetActive(true);
    }

}
