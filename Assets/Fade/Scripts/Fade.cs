/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Fade : MonoBehaviour
{
	IFade fade;
    public Texture[] maskTexture;

	void Start ()
	{
		Init ();
		fade.Range = cutoutRange;
	}

	float cutoutRange;

	void Init ()
	{
		fade = GetComponent<IFade> ();
	}

	void OnValidate ()
	{
		Init ();
		fade.Range = cutoutRange;
	}

	IEnumerator FadeoutCoroutine (int number,float time, System.Action action)
	{
        GetComponent<FadeImage>().UpdateMaskTexture(maskTexture[number]);

        float endTime = Time.timeSinceLevelLoad + time * (cutoutRange);

		var endFrame = new WaitForEndOfFrame ();

		while (Time.timeSinceLevelLoad <= endTime) {
			cutoutRange = (endTime - Time.timeSinceLevelLoad) / time;
			fade.Range = cutoutRange;
            yield return endFrame;
		}
		cutoutRange = 0;
		fade.Range = cutoutRange;

		if (action != null) {
			action ();
		}
	}

	IEnumerator FadeinCoroutine (int number, float time, System.Action action)
	{
        GetComponent<FadeImage>().UpdateMaskTexture(maskTexture[number]);

        float endTime = Time.timeSinceLevelLoad + time * (1 - cutoutRange);
		
		var endFrame = new WaitForEndOfFrame ();

		while (Time.timeSinceLevelLoad <= endTime) {
			cutoutRange = 1 - ((endTime - Time.timeSinceLevelLoad) / time);
			fade.Range = cutoutRange;
            yield return endFrame;
		}
		cutoutRange = 1;
		fade.Range = cutoutRange;

		if (action != null) {
			action ();
		}
	}

	public Coroutine FadeOut (int number, float time, System.Action action)
	{
        FadeImage.num = number;
        GetComponent<FadeImage>().UpdateMaskTexture(maskTexture[number]);
        StopAllCoroutines ();
		return StartCoroutine (FadeoutCoroutine (number,time, action));
	}

	public Coroutine FadeOut (int number,float time)
	{
		return FadeOut (number,time, null);
	}

	public Coroutine FadeIn (int number,float time, System.Action action)
	{
        FadeImage.num = number;
        GetComponent<FadeImage>().UpdateMaskTexture(maskTexture[number]);
        StopAllCoroutines();
		return StartCoroutine (FadeinCoroutine (number,time, action));
	}

	public Coroutine FadeIn (int number, float time)
	{
		return FadeIn (number,time, null);
	}
}