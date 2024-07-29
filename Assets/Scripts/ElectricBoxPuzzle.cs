using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ElectricBoxPuzzle : MonoBehaviour
{
   [Header("General")] 
   public bool isPuzzleOngoing;
   private Coroutine _blinkingCoroutine;
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
   [SerializeField] private TMP_Text timerText;
   [SerializeField] private float puzzleSeconds;

   [Header("SwitchButton")] 
   [SerializeField] private Transform rotatorPivot;
   [SerializeField] private Transform[] rotatorTurnsTransform;
   [SerializeField] private float rotateTime;

   [Header("RedButton")] 
   [SerializeField] private Transform redButtonPivot;
   [SerializeField] private Transform redButtonIdleTransform;
   [SerializeField] private Transform redButtonPressedTransform;
   [SerializeField] public float halfPressDuration;
   private bool redButtonIsPressing;

   

   public void StartPuzzle()
   {
      
      StartCoroutine(TwistRotator(rotatorTurnsTransform[1].rotation));
      StartCoroutine(ElectricBoxTimer());
      redButton.gameObject.SetActive(true);
      EventSystem.current.SetSelectedGameObject(redButton.gameObject);
      switchButton.gameObject.SetActive(false);
      isPuzzleOngoing = true;
      
      if (_blinkingCoroutine != null)
      {
         StopCoroutine(_blinkingCoroutine);
      }
      _blinkingCoroutine = StartCoroutine(BlinkingCoroutine());
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
      ResetLastLevel();
      AddLevelCompletion();
      ChangeLevelLight();
      blinkingInterval -= 0.25f;
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
   private IEnumerator BlinkingCoroutine()
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
      _blinkingCoroutine = null;
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
   
   private IEnumerator ElectricBoxTimer()
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
