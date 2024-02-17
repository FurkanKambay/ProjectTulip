using System;
using UnityEngine;

namespace Tulip.WorldGen
{
    public class ProcGen : MonoBehaviour
    {
        public ComputeShader computeShader;

        [Header("Colors")]
        [SerializeField] Color emptyColor = new(0, 100, 150);
        [SerializeField] Color groundColor = new(20, 20, 20);

        [Header("Resolution")]
        [SerializeField, Range(1, 128)] int xFactor;
        [SerializeField, Range(1, 128)] int yFactor;

        [Header("Preview")]
        [SerializeField] int width = 128;
        [SerializeField] int height = 128;

        [Header("Settings")]
        [SerializeField] int surfaceHeight = 50;
        [SerializeField, Range(0, 1)] float wallCutoff = .5f;
        [SerializeField] int[] neighborCutoffSteps;

        private ComputeBuffer buffer;
        private float[] data;

        private void Update() => Generate();

        [ContextMenu("Generate")]
        private void Generate()
        {
            buffer = new ComputeBuffer(width * height, sizeof(int));

            int indexMain = computeShader.FindKernel("cs_main");
            int indexSmooth = computeShader.FindKernel("cs_smooth");

            computeShader.SetBuffer(indexMain, "result", buffer);
            computeShader.SetFloat("width", width);
            computeShader.SetFloat("height", height);
            computeShader.SetFloat("wall_cutoff", wallCutoff);
            computeShader.SetInt("surface_height", surfaceHeight);

            computeShader.SetBuffer(indexSmooth, "result", buffer);

            computeShader.Dispatch(indexMain, width / 8, height / 8, 1);

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

                Gizmos.color = value == 0 ? emptyColor : groundColor;
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one * 1f);
            }
        }

        private void OnValidate()
        {
            width = xFactor * 8;
            height = yFactor * 8;
            surfaceHeight = Mathf.Clamp(surfaceHeight, 0, height);

            Array.Resize(ref data, width * height);
            Generate();
        }
    }
}
