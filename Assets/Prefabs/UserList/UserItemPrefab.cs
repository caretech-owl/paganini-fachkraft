using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class UserItemEvent : UnityEvent<User>
{
}

public class UserItemPrefab : MonoBehaviour
{
    public Button UserButton;
    public TMPro.TMP_Text MnemonicToken;

    public Button SettingButton;

    public UserItemEvent OnSelected;
    public UserItemEvent OnSettingSelected;

    private User UserItem;

    // Start is called before the first frame update
    void Start()
    {
        UserButton.onClick.AddListener(itemSelected);
        SettingButton.onClick.AddListener(settingSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillUser(User user)
    {
        MnemonicToken.text = user.Mnemonic_token;
        UserItem = user;
    }

    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(UserItem);
        }
    }

    private void settingSelected()
    {
        if (OnSettingSelected != null)
        {
            OnSettingSelected.Invoke(UserItem);
        }
    }

}
