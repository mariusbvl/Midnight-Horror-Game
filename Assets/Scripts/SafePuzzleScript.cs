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
    [Header("Codes")] 
    [SerializeField] private TMP_Text[] codesText;
    private int _randomIndex;

    [Header("Audio")] 
    [SerializeField] private AudioClip numberPressSound;
    [SerializeField] private AudioClip correctCodeSound;
    [SerializeField] private AudioClip wrongCodeSound;
    [SerializeField] private AudioClip handleTwistSound;
    [SerializeField] private AudioClip safeDoorSound;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        _characterCounter = 0;
        correctCode = Random.Range(1000, 9999);

        _randomIndex = Random.Range(0, codesText.Length);
        codesText[_randomIndex].text = correctCode.ToString();
        codesText[_randomIndex].gameObject.SetActive(true);
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
        SoundFXManager.Instance.PlaySoundFxClip(numberPressSound, InteractController.Instance.player.transform , 1f, 0f);
    }

    public void ClearTextField()
    {
        textField.text = "";
        _characterCounter = 0;
        SoundFXManager.Instance.PlaySoundFxClip(numberPressSound, InteractController.Instance.player.transform , 1f, 0f);
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
            SoundFXManager.Instance.PlaySoundFxClip(correctCodeSound, InteractController.Instance.player.transform , 1f, 0f);
            StartCoroutine(OpenSafe());
            InteractController.Instance.isSafeOpen = true;
        }
        else
        {
            SoundFXManager.Instance.PlaySoundFxClip(wrongCodeSound, InteractController.Instance.player.transform , 1f, 0f);
            ClearTextField();
            GameManager.Instance.CloseSafeCodePicker();
            StartCoroutine(InteractController.Instance.FadeText(inccorectPasswordText));
        }
        
    }

    private IEnumerator OpenSafe()
    {
        ClearTextField();
        GameManager.Instance.CloseSafeCodePicker();
        SoundFXManager.Instance.PlaySoundFxClip(handleTwistSound, handlePivot , 1f, 1f);
        yield return StartCoroutine(TwistHandle());
        SoundFXManager.Instance.PlaySoundFxClip(safeDoorSound, safeDoorPivot , 1f, 1f);
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
