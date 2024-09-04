using Tulip.Data;
using Tulip.GameWorld;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tulip.Editor
{
    [CustomEditor(typeof(StructureData))]
    public class StructureDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Edit in Designer"))
                OpenInDesigner();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OpenInDesigner()
        {
            StructureDesigner designer = EditorSceneManager
                .OpenScene("Assets/Level/Structure Designer.unity")
                .GetRootGameObjects()[0]
                .GetComponent<StructureDesigner>();

            designer.SetStructureData(target as StructureData);
            designer.RevertToAsset();

            Selection.activeObject = designer;
        }
    }
}
