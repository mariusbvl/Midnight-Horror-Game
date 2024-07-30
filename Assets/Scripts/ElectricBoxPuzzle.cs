using System.Collections;
using FPC;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ElectricBoxPuzzle : MonoBehaviour
{
   public static ElectricBoxPuzzle Instance { get; private set; }
   [Header("General")] 
   public bool isPuzzleOngoing;
   public Coroutine blinkingCoroutine;
   [Header("Buttons")] 
   [SerializeField] private Button switchButton;
   [SerializeField] private Button redButton;

   [Header("Lights")] 
   [SerializeField] private Light[] lightIndicator;
   [SerializeField] private Light[] levelLightIndicator;
   [SerializeField] private bool[] lightBool;
   [SerializeField] private bool[] pressedLightBool;
   [SerializeField] private bool[] levelLightBool;
   [SerializeField] private float blinkingInterval;
   private int _currentLightIndex;
   private Light _currentLight;
   
   [Header("Timer")] 
   [SerializeField] public TMP_Text timerText;
   [SerializeField] public float puzzleSeconds;
   public Coroutine electricBoxTimerCoroutine;

   [Header("SwitchButton")] 
   [SerializeField] private Transform rotatorPivot;
   [SerializeField] private Transform[] rotatorTurnsTransform;
   [SerializeField] private float rotateTime;
   private int _rotatorIndex;

   [Header("RedButton")] 
   [SerializeField] private Transform redButtonPivot;
   [SerializeField] private Transform redButtonIdleTransform;
   [SerializeField] private Transform redButtonPressedTransform;
   [SerializeField] public float halfPressDuration;
   private bool redButtonIsPressing;

   [Header("LittleElectricBox")] 
   [SerializeField] private Transform littleElectricBoxDoorPivot;
   [SerializeField] private Transform littleElectricBoxDoorOpenPivot;
   [SerializeField] private float doorRotateTime;


   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
   }

   public void StartPuzzle()
   {
      _rotatorIndex = 1;
      StartCoroutine(TwistRotator(rotatorTurnsTransform[_rotatorIndex].rotation));
      if (electricBoxTimerCoroutine != null)
      {
         StopCoroutine(electricBoxTimerCoroutine);
      }
      electricBoxTimerCoroutine = StartCoroutine(ElectricBoxTimer());
      redButton.gameObject.SetActive(true);
      EventSystem.current.SetSelectedGameObject(redButton.gameObject);
      switchButton.gameObject.SetActive(false);
      isPuzzleOngoing = true;
      if (blinkingCoroutine != null)
      {
         StopCoroutine(blinkingCoroutine);
      }
      blinkingCoroutine = StartCoroutine(BlinkingCoroutine());
   }

   public void ResetPuzzle()
   {
      _rotatorIndex = 0;
      for (int i = 0; i < lightIndicator.Length; i++)
      {
         lightIndicator[i].color = Color.red;
         lightBool[i] = false;
         pressedLightBool[i] = false;
      }

      for (int i = 0; i < levelLightIndicator.Length; i++)
      {
         levelLightIndicator[i].color = new Color(1f , 0.7600516f, 0 , 1f);
         levelLightBool[i] = false;
      }
      StartCoroutine( TwistRotator(rotatorTurnsTransform[_rotatorIndex].rotation));
   }
   
   public void PressButton()
   {
      if(redButtonIsPressing) return;
      pressedLightBool[_currentLightIndex] = !pressedLightBool[_currentLightIndex];
      ResetLightColor();
      StartCoroutine(RedButtonAnimation());
      CheckForNextLevel();
   }


   private void CheckForNextLevel()
   {
      if(!IsLevelComplete()) return;
      _rotatorIndex++;
      AddLevelCompletion();
      ChangeLevelLight();
      StartCoroutine( TwistRotator(rotatorTurnsTransform[_rotatorIndex].rotation));
      Debug.Log(IsPuzzleComplete());
      CheckForPuzzleComplete();
      if(IsPuzzleComplete()) return;
      ResetLastLevel();
      blinkingInterval -= 0.33f;
   }
   
   private void CheckForPuzzleComplete()
   {
      if(!IsPuzzleComplete()) return;
      StartCoroutine(RotateLittleElectricBoxDoor());
      GameManager.Instance.CloseElectricBoxPuzzle();
      InteractController.Instance.isLittleElectricBoxOpen = true;
   }

   public bool IsPuzzleComplete()
   {
      for (int i = 0; i < levelLightBool.Length; i++)
      {
         if (!levelLightBool[i])
         {
            return false;
         }
      }
      return true;
   }
   private void ChangeLevelLight()
   {
      for (int i = 0; i < levelLightBool.Length; i++)
      {
         if(!levelLightBool[i]) return;
         levelLightIndicator[i].color = Color.green;
      }
   }
   private void AddLevelCompletion()
   {
      for (int i = 0; i < levelLightBool.Length; i++)
      {
         if (!levelLightBool[i])
         {
            levelLightBool[i] = true;
            break;
         }
      }
   }
   
   private void ResetLastLevel()
   {
      for (int i = 0; i < pressedLightBool.Length; i++)
      {
         pressedLightBool[i] = false;
      }
      for (int i = 0; i < lightIndicator.Length; i++)
      {
         lightIndicator[i].color = Color.red;
      }
   }
   
   private bool IsLevelComplete()
   {
      for (int i = 0; i < pressedLightBool.Length; i++)
      {
         if (!pressedLightBool[i])
         {
            return false;
         }
      }
      return true;
   }
   public IEnumerator BlinkingCoroutine()
   {
      while (isPuzzleOngoing)
      {
         yield return new WaitForSeconds(blinkingInterval);
         _currentLightIndex = Random.Range(0, lightIndicator.Length);
         _currentLight = lightIndicator[_currentLightIndex];
         _currentLight.color = Color.green;
         lightBool[_currentLightIndex] = true;
         ResetLightColor();
      }
      blinkingCoroutine = null;
   }

   private void ResetLightColor()
   {
      for (int i = 0; i < lightBool.Length; i++)
      {
         if (lightBool[i] && i != _currentLightIndex && !pressedLightBool[i])
         {
            lightIndicator[i].color = Color.red;
            lightBool[i] = false;
         }
      }
   }
   
   public IEnumerator ElectricBoxTimer()
   {
      float remainingTime = puzzleSeconds;
      while (remainingTime > 0)
      {
         remainingTime--;
         timerText.text = $"{remainingTime}";
         yield return new WaitForSeconds(1f);
      }
      if (remainingTime != 0) yield break;
      GameManager.Instance.CloseElectricBoxPuzzle();
   }
   
   
   private IEnumerator RotateLittleElectricBoxDoor()
   {
      Quaternion startRotation = littleElectricBoxDoorPivot.rotation;
      Quaternion targetRotation = littleElectricBoxDoorOpenPivot.rotation;
      float timeElapsed = 0;
      while (timeElapsed < doorRotateTime)
      {
         littleElectricBoxDoorPivot.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / doorRotateTime);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      rotatorPivot.rotation = targetRotation;
   }
   
   private IEnumerator TwistRotator(Quaternion target)
   {
      Quaternion startRotation = rotatorPivot.rotation;
      float timeElapsed = 0;
      while (timeElapsed < rotateTime)
      {
         rotatorPivot.rotation = Quaternion.Slerp(startRotation, target, timeElapsed / rotateTime);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      rotatorPivot.rotation = target;
   }
   
   private IEnumerator RedButtonAnimation()
   {
      
      redButtonIsPressing = true;
      yield return StartCoroutine(PressButtonCoroutine(redButtonPressedTransform.position));
      yield return StartCoroutine(PressButtonCoroutine(redButtonIdleTransform.position));
      redButtonIsPressing = false;
   }
   
   private IEnumerator PressButtonCoroutine( Vector3 target)
   {
      Vector3 startPosition = redButtonPivot.position;

      float timeElapsed = 0;
      while (timeElapsed < halfPressDuration)
      {
         redButtonPivot.position = Vector3.Lerp(startPosition, target, timeElapsed / halfPressDuration);
         timeElapsed += Time.deltaTime;
         yield return null;
      }
      redButtonPivot.position = target;
   }
}
