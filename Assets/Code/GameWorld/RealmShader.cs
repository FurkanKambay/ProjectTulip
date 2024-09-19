using SaintsField;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class RealmShader : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField, Required] Tilemap wallTilemap;
        [SerializeField, Required] Tilemap blockTilemap;
        [SerializeField, Required] Tilemap curtainTilemap;

        [Header("Shader Properties")]
        [SerializeField, Required] EntityLocationDeterminer playerLocation;
        [SerializeField, Required] Transform portalTop;
        [SerializeField, Required] Transform portalBottom;

        [Header("Config")]
        [SerializeField] bool portalInSight;
        [SerializeField] bool activeRealm;
        [SerializeField, Range(0, 1)] float inactiveSaturation;

        private TilemapRenderer wallRenderer;
        private TilemapRenderer blockRenderer;
        private TilemapRenderer curtainRenderer;
        private Shader realmShader;

        private LocalKeyword shaderPortalInSight;
        private LocalKeyword shaderActiveRealm;

        private MaterialPropertyBlock realmPropertyBlock;
        private static readonly int shaderPlayerPosition = Shader.PropertyToID("_Player_Position");
        private static readonly int shaderPortalCeiling = Shader.PropertyToID("_Portal_Ceiling");
        private static readonly int shaderPortalFloor = Shader.PropertyToID("_Portal_Floor");
        private static readonly int shaderInactiveSaturation = Shader.PropertyToID("_Inactive_Saturation");

        private void Awake()
        {
            AssignRenderers();
            CacheKeywords();

            realmPropertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            SetKeywords();
            SetProperties();
        }

        public void SetPortalInSight(bool value) => portalInSight = value;
        public void SetActiveRealm(bool value) => activeRealm = value;
        public void SetInactiveSaturation(float value) => inactiveSaturation = value;

        private void AssignRenderers()
        {
            wallRenderer = wallTilemap.GetComponent<TilemapRenderer>();
            blockRenderer = blockTilemap.GetComponent<TilemapRenderer>();
            curtainRenderer = curtainTilemap.GetComponent<TilemapRenderer>();
        }

        private void CacheKeywords()
        {
            realmShader = blockRenderer.sharedMaterial.shader;
            shaderPortalInSight = new LocalKeyword(realmShader, "_PORTAL_IN_SIGHT");
            shaderActiveRealm = new LocalKeyword(realmShader, "_ACTIVE_REALM");
        }

        private void SetKeywords()
        {
            // Portal In Sight
            wallRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);
            blockRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);
            // curtainRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);

            // TODO: combine Curtain Reveal and Realm shaders
            // OR: convert Realm to a fullscreen shader

            // Active Realm
            wallRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
            blockRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
            // curtainRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
        }

        private void SetProperties()
        {
            realmPropertyBlock.SetVector(shaderPlayerPosition, playerLocation.Position);
            realmPropertyBlock.SetVector(shaderPortalCeiling, portalTop.position);
            realmPropertyBlock.SetVector(shaderPortalFloor, portalBottom.position);
            realmPropertyBlock.SetFloat(shaderInactiveSaturation, inactiveSaturation);

            wallRenderer.SetPropertyBlock(realmPropertyBlock);
            blockRenderer.SetPropertyBlock(realmPropertyBlock);
            curtainRenderer.SetPropertyBlock(realmPropertyBlock);
        }
    }
}
