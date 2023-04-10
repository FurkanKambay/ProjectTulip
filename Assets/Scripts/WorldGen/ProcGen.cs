using UnityEngine;

namespace Game.WorldGen
{
    public class ProcGen : MonoBehaviour
    {
        public ComputeShader shader;
        public RenderTexture texture;

        public int width = 512;
        public int height = 512;

        private void Start() => Generate();

        private void SetShaderProps()
        {
            shader.SetTexture(0, "result", texture);
            shader.SetFloat("resolution_x", width);
            shader.SetFloat("resolution_y", height);
            shader.Dispatch(0, width / 8, height / 8, 1);
        }

        [ContextMenu("Generate")]
        private void Generate()
        {
            texture = new RenderTexture(width, height, 24) { enableRandomWrite = true };
            texture.Create();
            SetShaderProps();
        }
    }
}
