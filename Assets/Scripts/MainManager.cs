using UnityEngine;
using System.Collections;
using SocketIO;

public class MainManager : MonoBehaviour {

	public static MainManager Instance;

	void Awake () {
		Instance = this;
	}

	void Start () {
		CheckLogin ();
	}

	private void CheckLogin() {
		if (!LoaderManager.Instance.isLoggedIn) {
			UIManager.Instance.ShowLogin ();
		} else {
			SocketStart ();
			UIManager.Instance.HideLogin ();
		}
	}

	public void CheckUsername() {
		Debug.Log ("CheckUsername");
		if (LoaderManager.Instance.Username == "") {
			TutorialStart ();
		} else {
			UIManager.Instance.ShowLobby ();
			Debug.Log ("game start!");
		}
	}

	public void TutorialStart() {
		Debug.Log ("Tutorial Start");
		UIManager.Instance.ShowTutorial ();
	}

	private void SocketStart() {
		Debug.Log ("Socket Start");
		var go = GameObject.Find ("SocketManager").GetComponent<SocketIOComponent> ();
		go.url = LoaderManager.Instance.SocketUrl + "&user=" + LoaderManager.Instance.Token;
		go.enabled = true;
		SocketManager.Instance.enabled = true;
	}

	private WWW SendToServer (string Url, string[] Data) {
		var wwwForm = new WWWForm ();
		for (var i = 0; i < Data.Length; i=i+2) {
			wwwForm.AddField (Data[i], Data[i+1]);
		}
		return new WWW (Url,wwwForm);
	}

	private void GetToken(WWW www) {
		Debug.Log ("LoginSuccess");
		Debug.Log (www.text);
		JSONObject j = new JSONObject (www.text);
		LoaderManager.Instance.UpdateSaveData (j["user"].ToString ().Replace ("\"", ""));
		CheckLogin ();
	}

	public void Forgot(string[] Data) {
		StartCoroutine (CoForgot(LoaderManager.Instance.ServerUrl + "/forgot", Data));
	}

	public void Login(string[] Data) {
		StartCoroutine (CoLogin(LoaderManager.Instance.ServerUrl + "/login", Data));
	}

	public void Register(string[] Data) {
		StartCoroutine (CoRegister(LoaderManager.Instance.ServerUrl + "/register", Data));
	}

	IEnumerator CoLogin (string Url, string[] Data) {
		Debug.Log ("CoLogin");
		var www = SendToServer (Url, Data);
		yield return www;
		if (www.error == null) {
			GetToken (www);
		} else {
			if (www.error == "401 Unauthorized") {
				AlertBoxManager.Instance.AlertBox ("Email or Password is not correct", false, "");	
			} else {
				AlertBoxManager.Instance.AlertBox (www.error, false, "");
			}
		}
	}

	IEnumerator CoRegister (string Url, string[] Data) {
		Debug.Log ("Register");
		var www = SendToServer (Url, Data);
		yield return www;
		if (www.error == null) {
			UIManager.Instance.RegisterDone ();
		} else {
			AlertBoxManager.Instance.AlertBox (www.error, false, "");
		}
	}

	IEnumerator CoForgot (string Url, string[] Data) {
		Debug.Log ("CoForgot");
		var www = SendToServer (Url, Data);
		yield return www;
		if (www.error == null) {
			AlertBoxManager.Instance.AlertBox ("Please check your email", false, "");
		} else {
			AlertBoxManager.Instance.AlertBox (www.error, false, "");
		}
	}

}
