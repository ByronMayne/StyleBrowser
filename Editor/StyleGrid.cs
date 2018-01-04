using System;
using UnityEditor;
using UnityEngine;

public class StyleGrid : GUIGrid
{
    private const float CONTENT_PADDING = 5f;
    private Rect _checkerScale;
    private Texture2D _checkerBackground;
    private Texture2D _gradientBackground;

    private string[] _iconPaths;
    private GUIStyle[] _styles;
    private bool _stylesDirty;

    public StyleGrid() : base()
    {
        // Reset our size
        _elementCount = 0;
        _stylesDirty = true;
        // Gradient
        {
            _gradientBackground = new Texture2D(1, 16);
            _gradientBackground.name = "[Generated] Gradient Texture";
            _gradientBackground.hideFlags = HideFlags.DontSave;
            Color color = new Color(1, 1, 1, 0);
            Color color2 = new Color(1, 1, 1, 0.4f);
            for (int i = 0; i < 16; i++)
            {
                float c = Mathf.Abs((float)i / 15f * 2f - 1f);
                c *= c;
                _gradientBackground.SetPixel(0, i, Color.Lerp(color, color2, c));
            }
            _gradientBackground.Apply();
            _gradientBackground.filterMode = FilterMode.Point;
        }
        // Checker
        {
            _checkerBackground = new Texture2D(16, 16);
            _checkerBackground.name = "[Generated] Checker Texture";
            _checkerBackground.hideFlags = HideFlags.DontSave;
            Color c0 = new Color(0, 0, 0, 0.05f);
            Color c1 = new Color(1, 1, 1, 0f);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _checkerBackground.SetPixel(j, i, c1);
                }
            }
            for (int k = 8; k < 16; k++)
            {
                for (int l = 0; l < 8; l++)
                {
                    _checkerBackground.SetPixel(l, k, c0);
                }
            }
            for (int m = 0; m < 8; m++)
            {
                for (int n = 8; n < 16; n++)
                {
                    _checkerBackground.SetPixel(n, m, c0);
                }
            }
            for (int num = 8; num < 16; num++)
            {
                for (int num2 = 8; num2 < 16; num2++)
                {
                    _checkerBackground.SetPixel(num2, num, c1);
                }
            }
            _checkerBackground.Apply();
            _checkerBackground.filterMode = FilterMode.Point;
        }

        // Reset out grid
        _checkerScale = new Rect();
        _checkerScale.width = 4;
        _checkerScale.height = 4;
    }

    public override void DoLayout(Rect screenRect)
    {
        if(_stylesDirty)
        {
            LoadStyles();
        }
        base.DoLayout(screenRect);
    }

    public void LoadStyles()
    {
        _styles = new GUIStyle[GUI.skin.customStyles.Length];
        _elementCount = _styles.Length;
        for (int i = 0; i < _styles.Length; i++)
        {
            GUIStyle style = new GUIStyle(GUI.skin.customStyles[i]);
            style.fixedWidth = 0f;
            style.fixedHeight = 0f;
            style.contentOffset = Vector2.zero;
            style.overflow = new RectOffset(0, 0, 0, 0);
            _styles[i] = style;
        }
        _stylesDirty = false;
    }

    protected override void OnElementContextClicked(int index)
    {
        EditorGUIUtility.systemCopyBuffer = _styles[index].name;
        
    }

    protected override void OnDrawElement(Rect rect, int index)
    {
        GUIStyle style = _styles[index];
        // Frame
        if (_selectedIndex == index)
        {
            GUI.Box(rect, GUIContent.none, "TL SelectionButton PreDropGlow");
        }
        else
        {
            GUI.Box(rect, GUIContent.none);
        }
        // Checkerboard
        rect.x += CONTENT_PADDING;
        rect.y += CONTENT_PADDING;
        rect.width -= CONTENT_PADDING * 2f;
        rect.height -= CONTENT_PADDING * 2f;
        GUI.DrawTextureWithTexCoords(rect, _checkerBackground, _checkerScale);
        // Icon
        if (Event.current.type == EventType.Repaint)
        {
            // We have to check if we are going to draw outside the box
            float height = style.CalcHeight(GUIContent.none, rect.width);

            if (height > rect.height)
            {
                // The style's height is based on the width so we have to shrink it down so it 
                // does not draw outside the box
                float width = rect.width * (rect.height / height);
                // Force the height
                rect.width = width;
            }

            style.Draw(rect, false, true, true, false);
        }
    }
}
