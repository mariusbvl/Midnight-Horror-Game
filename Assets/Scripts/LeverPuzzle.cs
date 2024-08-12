using System.Collections;
using FPC;
using UnityEngine;

public class LeverPuzzle : MonoBehaviour
{
    public static LeverPuzzle Instance { get; private set; }
    [Header("General")] 
    [SerializeField] public bool isPuzzleComplete;
    [SerializeField] private bool[] leversBool;
    [Header("Handles")] 
    [SerializeField] private float leverRotationDuration;
    [SerializeField] private GameObject[] leverHandles;
    private GameObject _currentHandle;
    [HideInInspector] public Transform leverHandlePivot;
    [HideInInspector] public Transform leverHandleUpPivot;
    [HideInInspector] public Transform leverHandleDownPivot;
    [HideInInspector] public GameObject leverHandleParentGameObject;
    [HideInInspector] public MeshRenderer leverDisplay; 
    [SerializeField] private Material[] leverMaterials;

    [Header("Drawer")]
    [SerializeField] private Transform drawerPivot;
    [SerializeField] private Transform drawerOpenPivot;
    [SerializeField] private float drawerOpenDuration;

    [Header("Audio")] 
    [SerializeField] private AudioClip leverUpSound;
    [SerializeField] private AudioClip leverDownSound;
    [SerializeField] private AudioClip drawerOpenSound;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        StartCoroutine(SwitchRandomLevers());
    }

    public IEnumerator SwitchLever()
    {
        if(isPuzzleComplete) yield break;
        _currentHandle = InteractController.Instance.electricLever;
        int currentIndex = IndexOfCurrentLever(_currentHandle);
        yield return StartCoroutine(RotateLever(leverHandles[IndexOfCurrentLever(_currentHandle)]));
        if (currentIndex == 0)
        {
            yield return StartCoroutine(RotateLever(leverHandles[currentIndex + 1]));
        }else if (currentIndex == leverHandles.Length- 1)
        {
            yield return StartCoroutine(RotateLever(leverHandles[currentIndex - 1]));
        }
        else
        {
            yield return StartCoroutine(RotateLever(leverHandles[currentIndex - 1]));
            yield return StartCoroutine(RotateLever(leverHandles[currentIndex + 1]));
        }

        if (IsPuzzleComplete())
        {
            isPuzzleComplete = true;
            SoundFXManager.Instance.PlaySoundFxClip(drawerOpenSound, drawerPivot, 1f,1f);
            yield return StartCoroutine(OpenDrawer());
        }
    }

    private IEnumerator RotateLever(GameObject currentHandle)
    {
        leverHandlePivot = currentHandle.transform.parent;
        leverHandleParentGameObject = leverHandlePivot.transform.parent.gameObject;
        leverHandleUpPivot = leverHandleParentGameObject.transform.Find("LeverHandleUpPivot");
        leverHandleDownPivot = leverHandleParentGameObject.transform.Find("LeverHandleDownPivot");
        leverDisplay = leverHandleParentGameObject.transform.Find("display")
            .GetComponent<MeshRenderer>();
        Quaternion startRotation = leversBool[IndexOfCurrentLever(currentHandle)]
            ?leverHandleDownPivot.rotation
            :leverHandleUpPivot.rotation;
        Quaternion targetRotation = leversBool[IndexOfCurrentLever(currentHandle)]
            ? leverHandleUpPivot.rotation
            : leverHandleDownPivot.rotation;
        Material targetMaterial =
            leversBool[IndexOfCurrentLever(currentHandle)] ? leverMaterials[0] : leverMaterials[1];
        
        SoundFXManager.Instance.PlaySoundFxClip(leversBool[IndexOfCurrentLever(currentHandle)] ? leverDownSound : leverUpSound, leverHandlePivot, 1f,1f);
        
        float timeElapsed = 0;
        while (timeElapsed < leverRotationDuration)
        {
            leverHandlePivot.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / leverRotationDuration);
            timeElapsed += Time.deltaTime; 
            yield return null;
        }
        leverHandlePivot.rotation = targetRotation;
        leversBool[IndexOfCurrentLever(currentHandle)] = !leversBool[IndexOfCurrentLever(currentHandle)];
        leverDisplay.material = targetMaterial;
        yield return null;
    }

    private int IndexOfCurrentLever(GameObject currentLever)
    {
        for (int i = 0; i < leverHandles.Length; i++)
        {
            if (currentLever == leverHandles[i])
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsPuzzleComplete()
    {
        for (int i = 0; i < leversBool.Length; i++)
        {
            if (!leversBool[i])
            {
                return false;
            }
        }
        return true;
    }
    
    private IEnumerator OpenDrawer()
    {
        Vector3 startPosition = drawerPivot.position;
        Vector3 targetPosition = drawerOpenPivot.position;

        float timeElapsed = 0;
        while (timeElapsed < drawerOpenDuration)
        {
            drawerPivot.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / drawerOpenDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        drawerPivot.position = targetPosition;
    }

    private IEnumerator SwitchRandomLevers()
    {
        int leversToSwitch = Random.Range(1, 3);
        for (int i = 0; i < leversToSwitch; i++)
        {
            int currentIndex = Random.Range(0,5);
            yield return StartCoroutine(RotateLever(leverHandles[currentIndex]));
            if (currentIndex == 0)
            {
                yield return StartCoroutine(RotateLever(leverHandles[currentIndex + 1]));
            }else if (currentIndex == leverHandles.Length- 1)
            {
                yield return StartCoroutine(RotateLever(leverHandles[currentIndex - 1]));
            }
            else
            {
                yield return StartCoroutine(RotateLever(leverHandles[currentIndex - 1]));
                yield return StartCoroutine(RotateLever(leverHandles[currentIndex + 1]));
            }
        }
    }
}
