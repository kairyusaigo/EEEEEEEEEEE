using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertBoxManager : MonoBehaviour {

	public static AlertBoxManager Instance;

	public Canvas CanvasAlert;
	public Button BtnConfirm;
	public Text TxtMsg;
	public string Url;

	private bool NeedRestart = false;

	void Start () {
		DontDestroyOnLoad (gameObject);
		Instance = this;
		HideAlertBox ();
	}

	public void AlertBox (string Msg, bool needRestart, string url) {		
		TxtMsg.text = Msg;
		NeedRestart = needRestart;
		Url = url;
		ShowAlertBox ();
	}

	private void ShowAlertBox () {
		CanvasAlert.enabled = true;
	}

	private void HideAlertBox () {
		CanvasAlert.enabled = false;
	}

	public void BtnConfirmClicked () {
		HideAlertBox ();
		if (Url != "") {
			Application.OpenURL (Url);
		}
		if (NeedRestart) {
			Application.LoadLevel ("Loader");
		}
		Url = "";
	}
}
