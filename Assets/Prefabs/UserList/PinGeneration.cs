using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinGeneration : MonoBehaviour
{

    public GameObject CodeWrapper;
    private TMPro.TMP_Text[] PinElements;

    // Start is called before the first frame update
    void Start()
    {
        // Find and cache the TextMeshPro components inside CodeWrapper
        PinElements = CodeWrapper.GetComponentsInChildren<TMPro.TMP_Text>();        

        Clearlist();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePIN()
    {
        PaganiniRestAPI.User.GeneratePIN(AppState.CurrentUser.Id, GeneratePinSucceeded, GeneratePinFailed);        
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
    }

    private void GeneratePinFailed(string error)
    {
        Debug.LogError("Failed to generate PIN: " + error);
    }

}
