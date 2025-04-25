using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Weapon", menuName = "Tank Weapons/ Weapon", order = 0)]

public class WeaponSO : ScriptableObject
{
    #region VARIABLES
    //Weapon Definition
    public WeaponType Type;
    public string Name;
    //Weapon Model and Transform Info
    public GameObject WeaponModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;
    public Vector3 SpawnScale;

    //Weapon Configuration
    public WeaponConfigSO WeaponConfig;
    public VisualTrailConfigSO TrailConfig;

    //Privates
    private MonoBehaviour _activeMonoBehaviour;
    private GameObject _weaponModel;
    private float _lastShootTime;
    private ParticleSystem _trailParticleSystem; //The weapon Model must have a particle system component located at the tip of the weapon and forward vector looking to shoot direction
    private ObjectPool<TrailRenderer> _trailPool;
    #endregion

    #region WEAPON METHODS
    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        _activeMonoBehaviour = ActiveMonoBehaviour;
        _trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        _weaponModel = Instantiate(WeaponModelPrefab);
        _weaponModel.transform.SetParent(Parent);
        _weaponModel.transform.localPosition = SpawnPoint;
        _weaponModel.transform.localRotation = Quaternion.Euler(SpawnRotation);
        _weaponModel.transform.localScale = SpawnScale;

        _trailParticleSystem = _weaponModel.GetComponentInChildren<ParticleSystem>();

        _lastShootTime = 0.0f;

    }
    #endregion

    #region TRAIL METHODS
    private TrailRenderer CreateTrail()
    {
        //Trail Creation
        GameObject trailInstance = new GameObject("Weapon Trail");
        TrailRenderer trailComponent = trailInstance.AddComponent<TrailRenderer>();

        //Trail Setup
        trailComponent.colorGradient = TrailConfig.TrailColorGradient;
        trailComponent.material = TrailConfig.TrailMaterial;
        trailComponent.widthCurve = TrailConfig.TrailWidthCurve;
        trailComponent.time = TrailConfig.TrailDuration;
        trailComponent.minVertexDistance = TrailConfig.TrailMinVertexDistance;

        //Trail Initial State
        trailComponent.emitting = false;
        trailComponent.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trailComponent;

    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer trailInstance = _trailPool.Get();
        trailInstance.gameObject.SetActive(true);
        trailInstance.transform.position = startPoint;

        yield return null; //To avoid using poistions from last frame

        trailInstance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            trailInstance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= TrailConfig.TrailSimSpeed * Time.deltaTime;
            yield return null;
        }

        trailInstance.transform.position = endPoint;

        
        if (hit.collider != null) {
            //TODO: HANDLE HIT VISUAL FEEDBACK
        }

        yield return new WaitForSeconds(TrailConfig.TrailDuration);
        yield return null;
        trailInstance.emitting = false ;
        trailInstance.gameObject.SetActive(false); 
        _trailPool.Release(trailInstance);

    }
    #endregion

    #region WEAPON SHOOT
    public void Shoot()
    {
        if (Time.time > WeaponConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;
            _trailParticleSystem.Play();

            Vector3 shootDirection = _trailParticleSystem.transform.forward;
            if (WeaponConfig.HasSpread)
            {
                shootDirection += ShootSpread();
            }
            shootDirection.Normalize();

            //Raycast Shoot
            if(Physics.Raycast(_trailParticleSystem.transform.position, shootDirection, out RaycastHit hit, Mathf.Infinity, WeaponConfig.HitMask))
            {
                _activeMonoBehaviour.StartCoroutine(PlayTrail(_trailParticleSystem.transform.position,
                                                               hit.point,
                                                               hit));
            }
            else
            {
                _activeMonoBehaviour.StartCoroutine(PlayTrail(_trailParticleSystem.transform.position,
                                                              _trailParticleSystem.transform.position + (shootDirection * TrailConfig.TrailMissDistance),
                                                              new RaycastHit()));
            }
        }
    }

    private Vector3 ShootSpread() { 
        Vector3 spreadAmount = new Vector3(
            Random.Range(-WeaponConfig.SpreadAmmount.x, WeaponConfig.SpreadAmmount.x),
            Random.Range(-WeaponConfig.SpreadAmmount.y, WeaponConfig.SpreadAmmount.y),
            Random.Range(-WeaponConfig.SpreadAmmount.z, WeaponConfig.SpreadAmmount.z)
            );
        Vector3 shootSpread = new Vector3(spreadAmount.x * WeaponConfig.SpreadAxis.x,
                                          spreadAmount.y * WeaponConfig.SpreadAxis.y,
                                          spreadAmount.z * WeaponConfig.SpreadAxis.z);
        return shootSpread;
    }

    #endregion
}
