using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FPC;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
   public static SaveManager Instance { get; private set; }

   public int nrOfBatteries = 2;
   public float batterySliderValue = 100f;
   public float cameraSensitivityValue = 0.1f;
   public float masterVolumeValue = 1f;
   public float musicVolumeValue = 0.5f;
   public float sfxVolumeValue = 0.5f;
   
   
   private void Awake()
   {
      if (Instance != null && Instance != this)
         Destroy(gameObject);
      else
         Instance = this;
      DontDestroyOnLoad(gameObject);
      Load();
   }

   public void Load()
   {
      if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
      {
         BinaryFormatter bf = new BinaryFormatter();
         FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
         PlayerDataStorage data = (PlayerDataStorage)bf.Deserialize(file);
         
         nrOfBatteries = data.nrOfBatteries;
         batterySliderValue = data.batterySliderValue;
         cameraSensitivityValue = data.cameraSensitivityValue;
         masterVolumeValue = data.masterVolumeValue;
         musicVolumeValue = data.musicVolumeValue;
         sfxVolumeValue = data.sfxVolumeValue;
         
         file.Close();
      }
   }

   public void Save()
   {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
      PlayerDataStorage data = new PlayerDataStorage();

      data.nrOfBatteries = nrOfBatteries;
      data.batterySliderValue = batterySliderValue;
      data.cameraSensitivityValue = cameraSensitivityValue;
      data.masterVolumeValue = masterVolumeValue;
      data.musicVolumeValue = musicVolumeValue;
      data.sfxVolumeValue = sfxVolumeValue;
      
      bf.Serialize(file, data);
      file.Close();
   }

   public void SaveBatteries()
   {
      if (InteractController.Instance != null)
      {
         nrOfBatteries = InteractController.Instance.nrOfBatteries;
      }
      
      if (FlashlightAndCameraController.Instance != null)
      {
         batterySliderValue = FlashlightAndCameraController.Instance.consumeSlider.value;
      }
      Save();
   }
}

[Serializable]
class PlayerDataStorage
{
   public int nrOfBatteries;
   public float batterySliderValue;
   public float cameraSensitivityValue;
   public float masterVolumeValue;
   public float musicVolumeValue;
   public float sfxVolumeValue;
}
