using System.Collections;
using FPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class BookShelfPuzzle : MonoBehaviour
{
   public static BookShelfPuzzle Instance { get; private set; }
   [Header("General")] 
   public bool canTryBook;
   public string correctCombination;
   [SerializeField] private GameObject[] correctBooks;
   [SerializeField] private GameObject[] allBooks;
   [SerializeField] private GameObject[]  firstRowBooks;
   [SerializeField] private GameObject[]  secondRowBooks;
   [SerializeField] private GameObject[]  thirdRowBooks;
   [SerializeField] private GameObject[]  fourthRowBooks;
   private int _firstRowRandomIndex;
   private int _secondRowRandomIndex;
   private int _thirdRowRandomIndex;
   private int _fourthRowRandomIndex;
   [SerializeField]private bool[] correctBooksBool;
   [Header("TryBook")] 
   [SerializeField] private float tryDuration;
   [SerializeField] private float returnDuration;

   [Header("BookShelf's pivots")] 
   [SerializeField] private Transform bookshelfPivot;
   [SerializeField] private Transform topBookShelfPivot;
   [SerializeField] private float bookShelfMoveDuration;

   [Header("Audio")] 
   [SerializeField] private AudioClip rotateBookAudio;
   [SerializeField] private AudioClip bookShelfMoveSound;
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      canTryBook = true;
      _firstRowRandomIndex = Random.Range(0, firstRowBooks.Length);
      _secondRowRandomIndex = Random.Range(0, secondRowBooks.Length);
      _thirdRowRandomIndex = Random.Range(0, thirdRowBooks.Length);
      _fourthRowRandomIndex = Random.Range(0, fourthRowBooks.Length);
      correctCombination = $"{_firstRowRandomIndex + 1} {_secondRowRandomIndex + 1} {_thirdRowRandomIndex + 1} {_fourthRowRandomIndex + 1}";
      correctBooks[0] = firstRowBooks[_firstRowRandomIndex];
      correctBooks[1] = secondRowBooks[_secondRowRandomIndex];
      correctBooks[2] = thirdRowBooks[_thirdRowRandomIndex];
      correctBooks[3] = fourthRowBooks[_fourthRowRandomIndex];
   }

   public IEnumerator InteractWithBook()
   {
      if(!canTryBook) yield break;
      GameObject book = InteractController.Instance.bookParent;
      yield return TryBook();
      if (!CheckBook(book))
      {
         ReturnAllBooks();
         ResetCorrectBookArray();
         yield return new WaitForSeconds(0.5f);
         yield return ReturnBook();
      }
      if(AreAllCorrectBook())
      {
         canTryBook = false;
         SoundFXManager.Instance.PlaySoundFxClip(bookShelfMoveSound, bookshelfPivot.transform, 1f,1f);
         yield return MoveBookShelf();
      }
   }
   
   private IEnumerator MoveBookShelf()
   {
     Vector3 startPosition = bookshelfPivot.transform.position;
     Vector3 targetPosition = topBookShelfPivot.transform.position;

     float timeElapsed = 0;
     while (timeElapsed < bookShelfMoveDuration)
     {
        bookshelfPivot.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / bookShelfMoveDuration);
        timeElapsed += Time.deltaTime;
        yield return null;
     }
     bookshelfPivot.position = targetPosition;
   }
   private IEnumerator TryBook()
   {
      if(InteractController.Instance.bookPivot.transform.rotation != InteractController.Instance.bookInitialPivot.transform.rotation) yield break;
      Quaternion startRotation = InteractController.Instance.bookInitialPivot.transform.rotation;
      Quaternion targetRotation = InteractController.Instance.bookTargetPivot.transform.rotation;
      
      SoundFXManager.Instance.PlaySoundFxClip(rotateBookAudio, InteractController.Instance.book.transform, 1f, 1f);
      
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
      if(InteractController.Instance.bookPivot.transform.rotation != InteractController.Instance.bookTargetPivot.transform.rotation) yield break;
      Quaternion startRotation = InteractController.Instance.bookTargetPivot.transform.rotation;
      Quaternion targetRotation = InteractController.Instance.bookInitialPivot.transform.rotation;
      
      SoundFXManager.Instance.PlaySoundFxClip(rotateBookAudio, InteractController.Instance.book.transform, 1f, 1f);
      
      float timeElapsed = 0;
      while (timeElapsed < returnDuration)
      {
         InteractController.Instance.bookPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / returnDuration);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      InteractController.Instance.bookPivot.transform.rotation = targetRotation;
   }
      
   private IEnumerator ReturnBook(GameObject bookParent, Transform bookPivot)
   {
      
      Quaternion startRotation = bookParent.transform.Find("targetPivot").transform.rotation;
      Quaternion targetRotation = bookParent.transform.Find("initialPivot").transform.rotation;

      SoundFXManager.Instance.PlaySoundFxClip(rotateBookAudio, bookPivot, 1f, 1f);
      
      float timeElapsed = 0;
      while (timeElapsed < returnDuration)
      {
         bookPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / returnDuration);
         timeElapsed += Time.deltaTime; 
         yield return null;
      }
      bookPivot.transform.rotation = targetRotation;
   }
   private void ReturnAllBooks()
   {
      if(!AreAnyCorrectBooks()) return;
      for (int i = 0; i < allBooks.Length; i++)
      {
         GameObject bookParent = allBooks[i].gameObject;
         GameObject book = bookParent.GetComponentInChildren<BoxCollider>().gameObject;
         Transform bookPivot = book.transform.parent.gameObject.transform;
         Transform initialPivot = bookParent.transform.Find("initialPivot");
         if (bookPivot.transform.rotation != initialPivot.transform.rotation)
         {
            SoundFXManager.Instance.PlaySoundFxClip(rotateBookAudio, book.transform, 1f, 1f);
            StartCoroutine(ReturnBook(bookParent, bookPivot));
         }
      }
   }
   
   private bool CheckBook(GameObject book)
   {
      foreach (GameObject b in correctBooks)
      {
         if (b == book)
         {
            AddCorrectBook();
            return true;
         }
      }
      return false;
   }

   private void AddCorrectBook()
   {
      for (int i = 0; i < correctBooksBool.Length; i++)
      {
         if (!correctBooksBool[i])
         {
            correctBooksBool[i] = true;
            break;
         }
      }
   }
   
   private void ResetCorrectBookArray()
   {
      for (int i = 0; i < correctBooksBool.Length; i++)
      {
         correctBooksBool[i] = false;
      }
   }

   private bool AreAnyCorrectBooks()
   {
      for (int i = 0; i < correctBooksBool.Length; i++)
      {
         if (correctBooksBool[i])
         {
            return true;
         }
      }
      return false;
   }
   
   private bool AreAllCorrectBook()
   {
      for (int i = 0; i < correctBooksBool.Length; i++)
      {
         if (!correctBooksBool[i])
         {
            return false;
         }
      }
      return true;
   }
}
