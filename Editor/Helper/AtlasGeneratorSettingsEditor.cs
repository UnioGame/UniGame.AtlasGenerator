/// <summary>
/// ButtonMethodAttribute,
/// modified from https://github.com/Deadcows/MyBox/blob/master/Attributes/ButtonMethodAttribute.cs
/// </summary>
/// 

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace UniModules.UniGame.AtlasGenerator.Editor.Helper
{
    namespace UnityAtlasGenerator.Helper.Internal
    {
#if ODIN_INSPECTOR
        using Sirenix.OdinInspector.Editor;
#endif
        

        [CustomEditor(typeof(AtlasGeneratorSettings), true), CanEditMultipleObjects]
        public class AtlasGeneratorSettingsEditor : UnityEditor.Editor
        {
            private List<MethodInfo> _methods;
            private AtlasGeneratorSettings _target;
            private bool _isOdinSupportActive = false;

#if ODIN_INSPECTOR
            private PropertyTree _propertyTree;
#endif
            
            private void OnEnable()
            {
#if ODIN_INSPECTOR
                _isOdinSupportActive = true;
#endif
                
                _target = target as AtlasGeneratorSettings;
                if (_target == null) return;

                _methods = AtlasGeneratorMethodHandler.CollectValidMembers(_target.GetType());
            }

            private void OnDisable()
            {
#if ODIN_INSPECTOR
                _propertyTree?.Dispose();
#endif
            }

            public override void OnInspectorGUI()
            {
#if ODIN_INSPECTOR
                _propertyTree = _propertyTree ?? PropertyTree.Create(_target);
#endif
                DrawBaseEditor();

                if (!_isOdinSupportActive)
                    DrawMethods(_methods);

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawMethods(List<MethodInfo> methods)
            {
                if (_methods == null) return;
                AtlasGeneratorMethodHandler.OnInspectorGUI(_target, _methods);
            }
            
            private void DrawBaseEditor()
            {
#if ODIN_INSPECTOR
                _propertyTree?.Draw();
                _propertyTree?.ApplyChanges();
#else
			    base.OnInspectorGUI();
#endif
            }
        }
    }
}