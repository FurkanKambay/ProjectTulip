using SaintsField.Playa;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class RealmPortal : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] RealmShader realmA;
        [SerializeField] RealmShader realmB;

        [Header("State")]
        [SerializeField] bool activateRealmA = true;
        [SerializeField] bool activateRealmB;

        private void OnEnable()
        {
            realmA.SetPortalInSight(true);
            realmB.SetPortalInSight(true);
        }

        private void OnDisable()
        {
            realmA.SetPortalInSight(false);
            realmB.SetPortalInSight(false);
        }

        private void Update()
        {
            realmA.SetActiveRealm(activateRealmA);
            realmB.SetActiveRealm(activateRealmB);
        }

        [Button]
        public void Switch()
        {
            activateRealmA = !activateRealmA;
            activateRealmB = !activateRealmB;
        }
    }
}
