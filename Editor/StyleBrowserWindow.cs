using System;
using UnityEditor;
using UnityEngine;

namespace StyleBrowser
{
    namespace UnityStyles
    {
        public sealed class StyleBrowserWindow : EditorWindow, IHasCustomMenu
        {
            public enum PreviewType
            {
                Styles,
                Textures
            }

            [MenuItem("Window/Style Browser...")]
            private static void LoadStyleBrowser()
            {
                GetWindow<StyleBrowserWindow>();
            }

            private const float MIN_PREIVEW_SIZE = 60;
            private const float MAX_PREVIEW_SIZE = 300;
            private float _previewSize = 50;
            private PreviewType _activePreview;
            private StyleGrid _styleGrid;
            private GUIContent _stylesButtonLabel;
            private GUIContent _TexturesButtonLabel;

            private string GetSavePath(string name)
            {
                return "Assets/Library/StyleBrowser/Editor/Docs/" + name;
            }

            private void OnEnable()
            {
                _stylesButtonLabel = new GUIContent("Styles");
                _TexturesButtonLabel = new GUIContent("Textures");
                _styleGrid = new StyleGrid();
                _styleGrid.ElementWidth = _previewSize;
                _styleGrid.ElementHeight = _previewSize;
                _styleGrid.ElementCount = 50;
                _styleGrid.UseBestFit = true;
                _styleGrid.OnRepaint += Repaint;
            }

            private void OnDisable()
            {
                _styleGrid.OnRepaint -= Repaint;
                _styleGrid = null;
            }

            private void OnGUI()
            {
                CaptureGroup.Begin(GetSavePath("FullWindow"));
                {
                    CaptureGroup.Begin(GetSavePath("Toolbar"));
                    {
                        DrawToolbar();
                    }
                    CaptureGroup.End();

                    CaptureGroup.Begin(GetSavePath("Grid"));
                    {
                        Rect gridRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 50f, GUILayout.ExpandHeight(true));
                        _styleGrid.DoLayout(gridRect);
                    }
                    CaptureGroup.End();
                }
                CaptureGroup.End();
            }

            private void DrawToolbar()
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                {
                    bool isPreviewingStyles = _activePreview == PreviewType.Styles;
                    bool isPreviewingTextures = _activePreview == PreviewType.Textures;

                    EditorGUI.BeginChangeCheck();
                    {
                        isPreviewingStyles = GUILayout.Toggle(isPreviewingStyles, _stylesButtonLabel, EditorStyles.toolbarButton);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (isPreviewingStyles)
                        {
                            _activePreview = PreviewType.Styles;
                        }
                    }
                    EditorGUI.BeginChangeCheck();
                    {
                        isPreviewingTextures = GUILayout.Toggle(isPreviewingTextures, _TexturesButtonLabel, EditorStyles.toolbarButton);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (isPreviewingTextures)
                        {
                            _activePreview = PreviewType.Textures;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    {
                        _previewSize = GUILayout.HorizontalSlider(_previewSize, MIN_PREIVEW_SIZE, MAX_PREVIEW_SIZE, GUILayout.Width(200));
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        _styleGrid.ElementWidth = _previewSize;
                        _styleGrid.ElementHeight = _previewSize;
                        _styleGrid.FocusElement(_styleGrid.SelectedIndex);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            public void AddItemsToMenu(GenericMenu menu)
            {
                menu.AddItem(new GUIContent("Preform Capture"), false, PreformCaputre);
                menu.AddItem(new GUIContent("Show Capture Debug"), CaptureGroup.ShowDebug, () => { CaptureGroup.ShowDebug = !CaptureGroup.ShowDebug; });
            }

            private void PreformCaputre()
            {
                position = new Rect(50, 50, 700, 400);
                CaptureGroup.PreformCapture(this);
            }
        }
    }
}