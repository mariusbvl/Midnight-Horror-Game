using System.Collections;
using FPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class BookShelfPuzzle : MonoBehaviour
{
   public static BookShelfPuzzle Instance { get; private set; }
   [Header("General")]
   public string correctCombination;
   [SerializeField] private GameObject[] allBooks;
   [SerializeField] private GameObject[]  firstRowBooks;
   [SerializeField] private GameObject[]  secondRowBooks;
   [SerializeField] private GameObject[]  thirdRowBooks;
   [SerializeField] private GameObject[]  fourthRowBooks;
   private int _firstRowRandomIndex;
   private int _secondRowRandomIndex;
   private int _thirdRowRandomIndex;
   private int _fourthRowRandomIndex;
   [Header("TryBook")] 
   [SerializeField] private float tryDuration;
   [SerializeField] private float returnDuration;
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      _firstRowRandomIndex = Random.Range(0, firstRowBooks.Length);
      _secondRowRandomIndex = Random.Range(0, secondRowBooks.Length);
      _thirdRowRandomIndex = Random.Range(0, thirdRowBooks.Length);
      _fourthRowRandomIndex = Random.Range(0, fourthRowBooks.Length);
      correctCombination = $"{_firstRowRandomIndex} {_secondRowRandomIndex} {_thirdRowRandomIndex} {_fourthRowRandomIndex}";
   }

   public IEnumerator InteractWithBook()
   {
      yield return TryBook();
      yield return new WaitForSeconds(1f);
      yield return ReturnBook();
   }

   private IEnumerator TryBook()
   {
      if(InteractController.Instance.bookPivot.transform.rotation != InteractController.Instance.bookInitialPivot.transform.rotation) yield break;
      Quaternion startRotation = InteractController.Instance.bookInitialPivot.transform.rotation;
      Quaternion targetRotation = InteractController.Instance.bookTargetPivot.transform.rotation;

      float timeElapsed = 0;
      while (timeElapsed < tryDuration)
      {
         InteractController.Instance.bookPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / tryDuration);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      InteractController.Instance.bookPivot.transform.rotation = targetRotation;
   }

   private IEnumerator ReturnBook()
   {
      
      Quaternion startRotation = InteractController.Instance.bookTargetPivot.transform.rotation;
      Quaternion targetRotation = InteractController.Instance.bookInitialPivot.transform.rotation;

      float timeElapsed = 0;
      while (timeElapsed < returnDuration)
      {
         InteractController.Instance.bookPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / returnDuration);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      InteractController.Instance.bookPivot.transform.rotation = targetRotation;
   }
}
