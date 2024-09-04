using Tulip.Core;
using Tulip.Data;
using Tulip.GameWorld;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Editor
{
    [CustomEditor(typeof(StructureDesigner))]
    public class StructureDesignerEditor : UnityEditor.Editor
    {
        private StructureData[] structures;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target.IsNot(out StructureDesigner designer) || structures == null)
                return;

            Assert.IsNotNull(designer);

            if (designer.StructureData.Is(out StructureData selectedStructureData))
            {
                Assert.IsNotNull(selectedStructureData);

                GUILayout.Space(16);
                GUILayout.Label($"Editing {selectedStructureData.name}");
            }

            // Save and load to asset
            EditorGUI.BeginDisabledGroup(!selectedStructureData);

            if (GUILayout.Button("Save"))
                designer.SaveToAsset();

            if (GUILayout.Button("Revert"))
                designer.RevertToAsset();

            EditorGUI.EndDisabledGroup();

            // Reset tilemaps
            EditorGUI.BeginDisabledGroup(!designer.AreTilemapsUsed);

            if (GUILayout.Button("Reset All"))
                designer.ResetTilemaps();

            EditorGUI.EndDisabledGroup();

            // Other structures
            GUILayout.Space(16);
            GUILayout.Label("Edit Other Structures:");

            foreach (StructureData structureData in structures)
            {
                if (!structureData)
                    return;

                EditorGUI.BeginDisabledGroup(selectedStructureData == structureData);

                if (GUILayout.Button(structureData.name))
                    OpenInDesigner(structureData);

                EditorGUI.EndDisabledGroup();
            }
        }

        private void OnEnable() => OnValidate();

        private void OnValidate() =>
            structures = Resources.FindObjectsOfTypeAll<StructureData>();

        // ReSharper disable Unity.PerformanceAnalysis
        private void OpenInDesigner(StructureData structureData)
        {
            var designer = target as StructureDesigner;

            if (!designer)
                return;

            designer.SetStructureData(structureData);
            designer.RevertToAsset();
        }
    }
}
