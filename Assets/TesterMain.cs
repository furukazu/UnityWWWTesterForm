using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TesterMain : MonoBehaviour {

	public Text LabelStates;
	public InputField ConURL;
	public InputField ReqHeader;
	public InputField ReqBody;
	public InputField ResContent;

	public Button Submit;

	public void OnSubmitPushed(){
		Submit.interactable = false;
		LabelStates.text = "Connecting...";

		PlayerPrefs.SetString ("url", ConURL.text);
		PlayerPrefs.SetString ("header", ReqHeader.text);
		PlayerPrefs.SetString ("body", ReqBody.text);
		PlayerPrefs.Save ();

		StartCoroutine (procConnection ());
	}

	private IEnumerator procConnection(){

		//var method = ConMethod.text;
		//if (string.IsNullOrEmpty (method)) {
		//	method = "POST";
		//}
		//if ("POST".Equals (method, System.StringComparison.OrdinalIgnoreCase)) {
		//	yield break;
		//}

		var url = ConURL.text;
		if (string.IsNullOrEmpty (url)) {
			url = "https://www.yahoo.com/";
		}

		var content = ReqBody.text;
		if (string.IsNullOrEmpty (content)) {
			content = "{}";
		}
		var contents = System.Text.Encoding.UTF8.GetBytes (content);


		var www = new WWW (url,contents,ToDic(ReqHeader.text));
		yield return www;

		do {
			if (!string.IsNullOrEmpty (www.error)) {
				ResContent.text = www.error;
				Submit.interactable = true;
				yield break;
			}
			yield return null;
		} while (!www.isDone) ;

		var result = string.Format (
@"{0}
{1}"
			,ToString(www.responseHeaders),www.text);

		if (result.Length > 4096) {
			result = result.Substring(0,4096) + "...";
		}

		ResContent.text = result;

		LabelStates.text = "Ready";
		Submit.interactable = true;
		yield break;
	}

	private string ToString(System.Collections.Generic.Dictionary<string,string> dic){
		var ret = new System.Text.StringBuilder ();
		foreach (var kv in dic) {
			ret.AppendFormat ("{0}: {1}",kv.Key,kv.Value).AppendLine ();
		}
		return ret.ToString ();
	}

	private System.Collections.Generic.Dictionary<string,string> ToDic(string str){
		var ret = new System.Collections.Generic.Dictionary<string,string> ();

		var sb = new System.IO.StringReader (str);
		for (var ln = sb.ReadLine(); ln != null; ln = sb.ReadLine()) {
			var strs = ln.Split('=');
			if(str.Length<2){ continue;}
			ret[strs[0]] = strs[1];
		}

		return ret;
	}

	// Use this for initialization
	void Start () {
		LabelStates.text = "Ready";

		ConURL.text = PlayerPrefs.GetString ("url", string.Empty);
		ReqHeader.text = PlayerPrefs.GetString ("header", string.Empty);
		ReqBody.text = PlayerPrefs.GetString ("body", string.Empty);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
