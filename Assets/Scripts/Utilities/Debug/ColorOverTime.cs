using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOverTime : MonoBehaviour {

    Color _start;
    Color _end;
    float _time;

    bool _initialized = false;

    float _currentTime = 0f;

    Renderer rend;

	public void Init(Color start, Color end, float time)
	{
        _start = start;
        _end = end;
        _time = time;

        _initialized = true;
        _currentTime = 0f;

        rend = GetComponent<Renderer>();
	}
	
	void Update () {
        if(_initialized) {
            _currentTime += Time.deltaTime / _time;
            rend.material.color = Color.Lerp(_start, _end, _currentTime);
            if (_currentTime > _time)
                _initialized = false;
        }
	}
}
