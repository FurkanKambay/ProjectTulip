using System;
using UnityEngine;

namespace Game.WorldGen
{
    public class ProcGen : MonoBehaviour
    {
        public ComputeShader computeShader;

        [Header("Resolution")]
        [SerializeField, Range(1, 128)] private int xFactor;
        [SerializeField, Range(1, 128)] private int yFactor;

        [Header("Preview")]
        [SerializeField] private int width = 128;
        [SerializeField] private int height = 128;

        [Header("Settings")]
        [SerializeField, Range(0, 1)] private float wallCutoff = .5f;
        [SerializeField] private int[] neighborCutoffSteps;

        private ComputeBuffer buffer;
        private float[] data;

        private void Update() => Generate();

        [ContextMenu("Generate")]
        private void Generate()
        {
            buffer = new ComputeBuffer(width * height, sizeof(int));

            int indexMain = computeShader.FindKernel("cs_main");
            computeShader.SetBuffer(indexMain, "result", buffer);
            computeShader.SetFloat("width", width);
            computeShader.SetFloat("height", height);
            computeShader.SetFloat("wall_cutoff", wallCutoff);
            computeShader.Dispatch(indexMain, width / 8, height / 8, 1);

            int indexSmooth = computeShader.FindKernel("cs_smooth");
            computeShader.SetBuffer(indexSmooth, "result", buffer);

            for (int i = 0; i < neighborCutoffSteps?.Length; i++)
            {
                computeShader.SetInt("neighbor_cutoff", neighborCutoffSteps?[i] ?? 4);
                computeShader.Dispatch(indexSmooth, width / 8, height / 8, 1);
            }

            buffer.GetData(data);
            buffer.Release();
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < data?.Length; i++)
            {
                int x = i % width;
                int y = i / width;
                float value = data[i];

                Gizmos.color = value == 0 ? Color.black : Color.white;
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one * 1f);
            }
        }

        private void OnValidate()
        {
            width = xFactor * 8;
            height = yFactor * 8;

            Array.Resize(ref data, width * height);
            Generate();
        }
    }
}
