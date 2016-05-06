using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class LoaderManager : MonoBehaviour {

	public static LoaderManager Instance;

	public bool isDebug;
	
	public Image ImgLogo;
	public Text TxtLog, TxtPercentage, TxtVersion;
	public Slider SliderLoader;
	public string Token, Username;
	public string SocketUrl = "ws://192.168.186.27:3000/socket.io/?EIO=4&transport=websocket";
	public string ServerUrl = "http://192.168.186.27:3000";
	public bool isLoggedIn = false;

	private string CurrentVersion = "0.0.1";
	private string SavedEmail = "";
	private string SavedPassword = "";
	private string SaveFile = "/GameData.dat";
	private string UpdateUrl = "https://www.google.com.hk";
	private float[] Process = new float[]{10f, 20f, 30f};

	private float LoadingPercentage, TargetPercentage;
	private AsyncOperation async = null;

	// Start to Check Everything
	void Start () {
		DontDestroyOnLoad (gameObject);
		Instance = this;
		if (isDebug) {
			StartCoroutine (LoadScene ());
			return;
		}
		StartCoroutine (GetLocalSave ());
	}

	void FixedUpdate () { 
		UpdatePercentage ();
	}

	private void UpdatePercentage () {
		if (TxtPercentage == null) { 
			return;
		}
		LoadingPercentage = Mathf.Lerp (LoadingPercentage, TargetPercentage, Time.deltaTime * 3);
		TxtPercentage.text = LoadingPercentage.ToString("F1") + "%";
		SliderLoader.value = LoadingPercentage;
	}

	private void UpdateTargetPercentage (float target) {
		TargetPercentage = target;
	}

	private void UpdateTxtLog (string Log) {
		TxtLog.text = Log;
	}

	private void LoadData () {
		Debug.Log ("LoadData");

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + SaveFile, FileMode.Open);
		Debug.Log (Application.persistentDataPath);
		GameData data = (GameData)bf.Deserialize(file);
		file.Close();

		CurrentVersion = data.Version;
		SavedEmail = data.Email;
		SavedPassword = data.Password;
	}

	private void SaveData () {
		Debug.Log ("SaveData");

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + SaveFile);
		GameData data = new GameData ();

		data.Version = CurrentVersion;
		data.Email = SavedEmail;
		data.Password = SavedPassword;

		bf.Serialize (file, data);
		file.Close ();
	}

	public void UpdateSaveData(string token) {
		Debug.Log ("Update SaveData");
		Token = token;
		isLoggedIn = true;
		SavedEmail = UIManager.Instance.InputEmail.text;
		SavedPassword = UIManager.Instance.InputPassword.text;
		SaveData ();
	}

	public void EraseSaveData() {
		Debug.Log ("Erase SaveData");
		SavedEmail = "";
		SavedPassword = "";
		SaveData ();
	}

	IEnumerator GetLocalSave() {
		UpdateTxtLog("Get Local Save");

		if (File.Exists (Application.persistentDataPath + SaveFile)) {
			LoadData ();
			Debug.Log ("Local Save--- Email: " + SavedEmail + " Password: " + SavedPassword); 
		} else {
			SaveData ();
		}

		UpdateTargetPercentage (Process[0]);
		yield return StartCoroutine (CheckVersion ());
	}

	IEnumerator CheckVersion() {
		UpdateTxtLog("Checking Version");

		var www = new WWW (ServerUrl + "/version");
		yield return www;

		if (www.error == null) { //check version if no error
			JSONObject j = new JSONObject (www.text);
			if (CurrentVersion == j["version"].ToString ().Replace("\"","")) {
				UpdateTargetPercentage (Process[1]);
				yield return StartCoroutine (AutoLogin ());
			} else {
				AlertBoxManager.Instance.AlertBox ("Need to Update", true, UpdateUrl);
			}
		} else {
			AlertBoxManager.Instance.AlertBox(www.error.ToString(), false, "");
		}
	}

	IEnumerator AutoLogin() {
		Debug.Log ("Auto Login");
		UpdateTxtLog("AutoLogin");

		if (SavedEmail != "") {
			var wwwForm = new WWWForm ();
			wwwForm.AddField ("username", SavedEmail);
			wwwForm.AddField ("password", SavedPassword);
			var www = new WWW (ServerUrl + "/login", wwwForm);
			yield return www;

			if (www.error == null) {
				JSONObject j = new JSONObject (www.text);
				Token = j["user"].ToString().Replace ("\"", "");
				isLoggedIn = true;
			} else {
				if (www.error == "401 Unauthorized") {
					EraseSaveData ();
					AlertBoxManager.Instance.AlertBox ("Email or Password is not correct", false, "");	
				} else {
					AlertBoxManager.Instance.AlertBox (www.error, false, "");
				}
			}
		}

		UpdateTargetPercentage (Process[2]); 
		yield return StartCoroutine (LoadScene ());
	}

	IEnumerator LoadScene() {
		UpdateTxtLog("Load Scene");

		async = Application.LoadLevelAsync ("Main");
		while (!async.isDone) {
			yield return async.isDone;
			UpdateTargetPercentage (Process[2] + async.progress*(100-Process[2]));
		}
		yield return async;
	}
}

[Serializable]
class GameData {
	public string Version;
	public string Email;
	public string Password;
}