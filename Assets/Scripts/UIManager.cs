using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

	public InputField InputEmail, InputPassword, InputRegisterEmail, InputRegisterPassword, InputForgotEmail, InputChangeUsername;
	public Canvas CanvasLogin, CanvasRegister, CanvasForgot, CanvasTutorial, CanvasChangeUsername;

	void Awake () {
		Instance = this;
	}

	public void HideLogin() {
		CanvasLogin.enabled = false;
		CanvasRegister.enabled = false;
		CanvasForgot.enabled = false;
	}

	public void ShowLogin() {
		CanvasLogin.enabled = true;
		CanvasRegister.enabled = false;
		CanvasForgot.enabled = false;
		InputEmail.text = "";
		InputPassword.text = "";
	}

	private void ShowRegister() {
		CanvasLogin.enabled = false;
		CanvasRegister.enabled = true;
		CanvasForgot.enabled = false;
		InputRegisterEmail.text = "";
		InputRegisterPassword.text = "";
	}

	private void ShowForgot() {
		CanvasLogin.enabled = false;
		CanvasRegister.enabled = false;
		CanvasForgot.enabled = true;
		InputForgotEmail.text = "";
	}

	public void ShowTutorial() {
		CanvasTutorial.enabled = true;
	}

	public void HideTutorial() {
		CanvasTutorial.enabled = false;
	}

	public void ShowChangeUsername() {
		CanvasChangeUsername.enabled = true;
	}

	public void HideChangeUsername() {
		CanvasChangeUsername.enabled = false;
	}

	private void JoinQuene() {
		SocketManager.Instance.JoinQueue ();
	}
	
	public void BtnLoginClicked () {
		MainManager.Instance.Login (new string[]{"username", InputEmail.text, "password", InputPassword.text});
	}

	public void BtnRegisterClicked() {
		ShowRegister ();
	}

	public void BtnForgotClicked() {
		ShowForgot ();
	}

	public void BtnRegisterConfirmClicked() {
		MainManager.Instance.Register (new string[]{"email", InputRegisterEmail.text, "password", InputRegisterPassword.text});
	}

	public void BtnForgotConfirmClicked() {
		MainManager.Instance.Forgot (new string[]{"email", InputForgotEmail.text});
	}
		
	public void BtnBackClicked() {
		ShowLogin ();
	}

	public void BtnPlayClicked() {
		JoinQuene ();
	}

	public void BtnTutorialClicked() {
		HideTutorial ();
		ShowChangeUsername ();
	}

	public void RegisterDone() {
		MainManager.Instance.Login (new string[]{"username", InputRegisterEmail.text, "password", InputRegisterPassword.text});
	}

	public void BtnChangerUsernameClicked() {
		JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
		j.AddField ("username", InputChangeUsername.text);
		SocketManager.Instance.SocketEmit ("change-username", j);
	}

	public void ShowLobby() {
		HideChangeUsername ();
		HideLogin ();
		HideTutorial ();
	}
}
