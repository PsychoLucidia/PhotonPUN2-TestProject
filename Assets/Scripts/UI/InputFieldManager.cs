using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Contracts;

public class InputFieldManager : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        if (inputField == null)
        {
            Debug.LogError("InputField is null");
        }
    }

    void OnDisable()
    {
        if (inputField != null)
        {
            inputField.text = "";
        }
    }
}
