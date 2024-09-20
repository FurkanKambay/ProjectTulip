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
        [SerializeField] float curtainRevealSpeed;

        private float curtainRevealProgress;

        private TilemapRenderer wallRenderer;
        private TilemapRenderer blockRenderer;
        private TilemapRenderer curtainRenderer;
        private Shader realmShader;

        private LocalKeyword shaderPortalInSight;
        private LocalKeyword shaderActiveRealm;

        private MaterialPropertyBlock propertyBlock;
        private static readonly int shaderPortalCeiling = Shader.PropertyToID("_Portal_Ceiling");
        private static readonly int shaderPortalFloor = Shader.PropertyToID("_Portal_Floor");
        private static readonly int shaderInactiveSaturation = Shader.PropertyToID("_Inactive_Saturation");
        private static readonly int shaderRevealProgress = Shader.PropertyToID("_Reveal_Progress");

        private void Awake()
        {
            AssignRenderers();
            CacheKeywords();

            propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            float maxDeltaTime = Time.deltaTime * curtainRevealSpeed;
            int location = playerLocation.Location.GetHashCode();
            curtainRevealProgress = Mathf.MoveTowards(curtainRevealProgress, location, maxDeltaTime);

#if UNITY_EDITOR
            CacheKeywords();
#endif

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

            var shaderIsCurtain = new LocalKeyword(realmShader, "_IS_CURTAIN");
            curtainRenderer.material.SetKeyword(shaderIsCurtain, true);
        }

        private void SetKeywords()
        {
            // Portal In Sight
            wallRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);
            blockRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);
            curtainRenderer.material.SetKeyword(shaderPortalInSight, portalInSight);

            // Active Realm
            wallRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
            blockRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
            curtainRenderer.material.SetKeyword(shaderActiveRealm, activeRealm);
        }

        private void SetProperties()
        {
            propertyBlock.SetVector(shaderPortalCeiling, portalTop.position);
            propertyBlock.SetVector(shaderPortalFloor, portalBottom.position);
            propertyBlock.SetFloat(shaderInactiveSaturation, inactiveSaturation);
            propertyBlock.SetFloat(shaderRevealProgress, curtainRevealProgress);

            wallRenderer.SetPropertyBlock(propertyBlock);
            blockRenderer.SetPropertyBlock(propertyBlock);
            curtainRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
