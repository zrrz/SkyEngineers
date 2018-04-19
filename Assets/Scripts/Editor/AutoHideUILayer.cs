using UnityEngine;
using UnityEditor;

namespace AstralByte.Editor
{
    /// Copyright 2017 by Astral Byte Ltd. 
    /// Source: http://www.astralbyte.co.nz/code/AutoHideUILayer.cs
    ///
    /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
    /// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
    /// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
    /// persons to whom the Software is furnished to do so, subject to the following conditions:
    ///
    /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
    /// Software.
    ///
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
    /// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
    /// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
    /// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    ///
    /// <summary>
    /// Auto hides the UI layer inside the Editor so it doesn't get in the way of 3D objects in scene view.
    ///
    /// Installation:  Put in any folder named "Editor"  Requires Unity 5.2 or later (for Selection.selectionChanged)
    ///
    /// Usage: When any object is selected in the editor hierarchy that is on the UI layer, 5 by default, the camera will 
    /// be automatically changed to 2D ISO and zoomed to the current selection and the UI layer shown. By clicking on any 
    /// object NOT on the UI layer, this script will then hide UI layer, change camera back to 3D/perspective mode and
    /// zoom on the selected object.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoHideUILayer
    {
        const string MENU_TOGGLE = "AstralByte/Toggle auto hide UI";
        const int UI_LAYER_NUMBER = 5;  // default, change as needed

        static bool _isUiShown = true;
        static bool _isEnabled = true;

        static AutoHideUILayer()
        {
            Selection.selectionChanged += OnSelectionChanged;
            // prevent race condition allowing editor time to initialize
            EditorApplication.delayCall += () => {
                _isUiShown = (Tools.visibleLayers & UI_LAYER_NUMBER) == UI_LAYER_NUMBER;
                _isEnabled = EditorPrefs.GetBool(MENU_TOGGLE, true);
            };
            // refresh again after one frame
            EditorApplication.update += () => { SetChecked(); EditorApplication.update -= SetChecked; };
        }

        [MenuItem(MENU_TOGGLE)]
        static void ToggleEnabled()
        {
            _isEnabled = !_isEnabled;
            SetChecked();
            EditorPrefs.SetBool(MENU_TOGGLE, _isEnabled);
            Debug.LogFormat("AutoHideUILayer is now <b><color=yellow>{0}</color></b>", _isEnabled ? "enabled" : "disabled");
        }

        [MenuItem(MENU_TOGGLE, true)]
        static bool ValidateChecked()
        {
            SetChecked();
            return true;
        }

        static void SetChecked()
        {
            Menu.SetChecked(MENU_TOGGLE, _isEnabled);
        }


        public static void OnSelectionChanged()
        {
            var obj = Selection.activeObject as GameObject;
            // only change settings when object in hierarchy is selected
            if (obj && !AssetDatabase.Contains(obj))
                SetUILayer(obj && obj.layer == UI_LAYER_NUMBER);
        }

        static void SetUILayer(bool doShow)
        {
            if (_isUiShown == doShow || !_isEnabled)
                return;
            if (doShow)
                Tools.visibleLayers |= 1 << UI_LAYER_NUMBER;
            else
                Tools.visibleLayers &= ~(1 << UI_LAYER_NUMBER);
            SceneView.lastActiveSceneView.orthographic = doShow;
            SceneView.lastActiveSceneView.in2DMode = doShow;
            SceneView.lastActiveSceneView.FrameSelected();
            _isUiShown = doShow;
        }
    }
}