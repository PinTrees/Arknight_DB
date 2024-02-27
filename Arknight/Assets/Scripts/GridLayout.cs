using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayout : MonoBehaviour
{
    public int paddingTop;
    public int paddingBottom;

    public int width;
    public int cellsize;
    public int spacing;
    RectTransform rect;
    public void Initialized()
    {
        rect = this.gameObject.GetComponent<RectTransform>();
    }
    public void Refresh(int count)
    {
        int height = count / width;
        if (!(count % width).Equals(0)) height++;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, ((cellsize + spacing) * height) + paddingTop + paddingBottom);
    }
}
