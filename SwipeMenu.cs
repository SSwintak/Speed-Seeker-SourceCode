using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField] Scrollbar _ScrollBar;
    [SerializeField] float _ScaleSpeed = 0.1f;
    [SerializeField] float _ScrollSpeed = 0.1f;

    float _ScrollPos;
    float[] _Pos;

    void Start()
    {
        _Pos = new float[transform.childCount];
    }
    void Update()
    {
        // Calculate the distance between each position
        float distance = 1f / (_Pos.Length - 1f);

        // Apply swipe resistance
        SwipResistance(distance);

        // Scale the selections based on focused (centered) selection
        ScaleSelections(distance);
    }

    private void SwipResistance(float distance)
    {
        // Update the positions
        for (int i = 0; i < _Pos.Length; i++)
        {
            _Pos[i] = distance * i;
        }
        if (Input.GetMouseButton(0))
        {
            _ScrollPos = _ScrollBar.value;
        }
        else
        {
            // Apply swipe resistance based on scroll position
            for (int i = 0; i < _Pos.Length; i++)
            {
                // if haven't scrolled pass the threshold return to the selection
                if (_ScrollPos < _Pos[i] + (distance / 2) && _ScrollPos > _Pos[i] - (distance / 2))
                {
                    _ScrollBar.value = Mathf.Lerp(_ScrollBar.value, _Pos[i], _ScrollSpeed);
                }
            }
        }
    }

    void ScaleSelections(float distance)
    {
        Transform child;
        for (int i = 0; i < _Pos.Length; i++)
        {
            // Check if scroll position is within range of a position (centered/focused position)
            if (_ScrollPos < _Pos[i] + (distance / 2) && _ScrollPos > _Pos[i] - (distance / 2))
            {
                child = transform.GetChild(i);

                // Scale the current selection up
                child.localScale = Vector2.Lerp(child.localScale, new Vector2(1f, 1f), _ScaleSpeed);
                for (int j = 0; j < _Pos.Length; j++)
                {
                    // Scale other selections down
                    if (j != i)
                    {
                        child = transform.GetChild(j);
                        child.localScale = Vector2.Lerp(child.localScale, new Vector2(0.8f, 0.8f), _ScaleSpeed);
                    }
                }
            }
        }
    }
}
