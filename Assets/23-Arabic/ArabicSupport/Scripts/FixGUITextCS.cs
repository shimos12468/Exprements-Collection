using UnityEngine;
using ArabicSupport;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

[RequireComponent(typeof(TextMeshPro))]
public class FixGUITextCS : MonoBehaviour {
    [TextArea]
	public string text;
	public bool tashkeel = true;
	public bool hinduNumbers = true;
    //public LocalizedString stringRef = new LocalizedString() { TableReference = "My String Table", TableEntryReference = "Hello World" };

    public bool updateText = true;

    public TMP_Text value;
	
    private void OnValidate()
    {   if (value) return;
        value = GetComponent<TMP_Text>();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (updateText)
        {
            if (text.Contains("\n"))
            {
                int c =text.Count(x => x == '\n');
                print($"it has {c} enter");
            }


            UpdateTextRuntime();
            updateText = false;
        }
    }
    public void UpdateTextRuntime()
	{
         value.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
