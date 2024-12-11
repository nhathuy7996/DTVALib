#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace DTVA.Editor{
[InitializeOnLoad]
public static class ToolBarHnn
{
    public static Action OnGUILeftAction,OnGUIRightAction;
    static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
		static Type m_guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
		static Type m_iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
		static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
		static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		static ScriptableObject m_currentToolbar;
    static ToolBarHnn()
    {
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        
        // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
			if (m_currentToolbar == null)
			{
                
				// Find toolbar
				var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
				m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
                
				if (m_currentToolbar != null)
				{ 
                     
#if UNITY_2021_1_OR_NEWER
					var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
					var rawRoot = root.GetValue(m_currentToolbar);
					var mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", OnGUILeft);
					RegisterCallback("ToolbarZoneRightAlign", OnGUIRight);

					void RegisterCallback(string root, Action cb) {
						var toolbarZone = mRoot.Q(root);

						var parent = new VisualElement()
						{
							style = {
								flexGrow = 1,
								flexDirection = FlexDirection.Row,
							}
						};
						var container = new IMGUIContainer();
						container.style.flexGrow = 1;
						container.onGUIHandler += () => { 
							cb?.Invoke();
						}; 
						parent.Add(container);
						toolbarZone.Add(parent);
					}
#else
#if UNITY_2020_1_OR_NEWER
					var windowBackend = m_windowBackend.GetValue(m_currentToolbar);

					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(windowBackend, null);
#else
					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(m_currentToolbar, null);
#endif

					// Get first child which 'happens' to be toolbar IMGUIContainer
					var container = (IMGUIContainer) visualTree[0];

					// (Re)attach handler
					var handler = (Action) m_imguiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					m_imguiContainerOnGui.SetValue(container, handler);
                     Debug.LogError("VVVV "+m_currentToolbar);
					
#endif
				}
			}
    }

    static void OnGUI()
		{
          
		}

    static void OnGUILeft()
		{ 
            OnGUILeftAction?.Invoke();
            
		}
    
    static void OnGUIRight()
		{
            OnGUIRightAction?.Invoke();
		}

}
}
#endif