using System.Collections;
using UnityEngine;
using SocketIO;

public class SocketManager : MonoBehaviour
{
	public static SocketManager Instance;

	private SocketIOComponent socket;

	void Awake () {
		Instance = this;
	}

	public void Start() 
	{
		socket = GetComponent<SocketIOComponent>();

		socket.On ("open", TestOpen);
		socket.On ("error", TestError);
		socket.On ("close", TestClose);

		socket.On ("start", SocketStart);
		socket.On ("change-username", SocketChangeUsername);
	}

	public void SocketStart(SocketIOEvent e) {
		Debug.Log (e.data);
		try {
			LoaderManager.Instance.Username = e.data ["user"]["username"].ToString ().Replace("\"","");
		} catch {
			Debug.Log ("no username");
		}
		MainManager.Instance.CheckUsername ();
	}

	public void TestOpen(SocketIOEvent e) {
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}

	public void TestError(SocketIOEvent e) {
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	public void TestClose(SocketIOEvent e) {	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	public void JoinQueue() {
		socket.Emit ("join-queue");
	}

	public void SocketEmit(string SocketName, JSONObject j) {
		socket.Emit (SocketName, j);
	}

	public void SocketChangeUsername(SocketIOEvent e) {
		Debug.Log ("socket: change-username: "+ e.data);
		if (e.data ["result"].ToString ().Replace ("\"", "") == "success") {
			MainManager.Instance.CheckUsername ();
		} else {
			AlertBoxManager.Instance.AlertBox ("Change Name Error", false, "");
		}
	}

}
