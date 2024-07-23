using System.Collections;
using FPC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SafePuzzleScript : MonoBehaviour
{
    public static SafePuzzleScript Instance { get; private set; }
    [Header("Code Panel")]
    public int correctCode;
    [SerializeField] private Button[] numbersArrayButtons;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text textField;
    [SerializeField] private TMP_Text inccorectPasswordText;
    private int _characterCounter;
    private int _introducedIntCode;
    [Header("Safe")] 
    [SerializeField] private Transform handlePivot;
    [SerializeField] private Transform handleTargetPivot;
    [SerializeField] private Transform safeDoorPivot;
    [SerializeField] private Transform safeOpenDoorPivot;
    public int handleAnimationDuration;
    public int doorOpeningAnimationDuration;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        _characterCounter = 0;
        correctCode = Random.Range(1000, 9999);
    }

    public void PressNumber()
    {
        if(_characterCounter == 4) return;
        for (int i = 0; i < numbersArrayButtons.Length; i++)
        {
            if (numbersArrayButtons[i].gameObject == EventSystem.current.currentSelectedGameObject)
            {
                textField.text += i;
                _characterCounter++;
            }
        }
    }

    public void ClearTextField()
    {
        textField.text = "";
        _characterCounter = 0;
    }

    public void ConfirmCode()
    {
        string codeString = textField.text;
        if (int.TryParse(codeString, out _introducedIntCode))
        {
            Debug.Log($"Correct code: {correctCode} \n Introduced Code: {_introducedIntCode}");
        }

        if (correctCode == _introducedIntCode)
        {
            StartCoroutine(OpenSafe());
            InteractController.Instance.isSafeOpen = true;
        }
        else
        {
            ClearTextField();
            GameManager.Instance.CloseSafeCodePicker();
            StartCoroutine(InteractController.Instance.FadeText(inccorectPasswordText));
        }
    }

    private IEnumerator OpenSafe()
    {
        ClearTextField();
        GameManager.Instance.CloseSafeCodePicker();
        yield return StartCoroutine(TwistHandle());
        yield return StartCoroutine(OpenSafeDoor());
    }
    
    private IEnumerator TwistHandle()
    {
        Quaternion startRotation = handlePivot.rotation;
        Quaternion targetRotation = handleTargetPivot.rotation;

        float timeElapsed = 0;

        while (timeElapsed < handleAnimationDuration)
        {
            handlePivot.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / handleAnimationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        handlePivot.transform.rotation = targetRotation;
    }
    
    
    private IEnumerator OpenSafeDoor()
    {
        Quaternion startRotation = safeDoorPivot.rotation;
        Quaternion targetRotation = safeOpenDoorPivot.rotation;

        float timeElapsed = 0;

        while (timeElapsed < doorOpeningAnimationDuration)
        {
            safeDoorPivot.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / doorOpeningAnimationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        safeDoorPivot.rotation = targetRotation;
    }
}
