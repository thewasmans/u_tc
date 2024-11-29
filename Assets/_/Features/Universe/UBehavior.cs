using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UObject = UnityEngine.Object;

namespace Universe
{
    public class UBehavior : MonoBehaviour
    {
        #region Publics
        public bool m_Debug;
        public bool m_isVerbose;
        public static PoolManager poolManager;
        #endregion

        #region Refs

        #endregion

        #region Debug

        protected void VerboseLog(string message, UnityEngine.Object context = null)
        {
            if (!m_isVerbose) return;
            UnityEngine.Debug.Log(message, context);
        }

        protected void VerboseWarning(string message, UnityEngine.Object context = null)
        {
            if (!m_isVerbose) return;
            UnityEngine.Debug.LogWarning(message, context);
        }

        protected void VerboseError(string message, UnityEngine.Object context = null)
        {
            if (!m_isVerbose) return;
            UnityEngine.Debug.LogError(message, context);
        }

        #endregion

        #region Cached Members
        [NonSerialized]
        private Transform _transform;
        public new Transform transform => _transform ? _transform : _transform = GetComponent<Transform>();

        [NonSerialized]
        private Animation _animation;
        public new Animation animation => _animation ? _animation : _animation = GetComponent<Animation>();

        [NonSerialized]
        private Camera _camera;
        public new Camera camera => _camera ? _camera : _camera = GetComponent<Camera>();

        [NonSerialized]
        private Collider _collider;
        public new Collider collider => _collider ? _collider : _collider = GetComponent<Collider>();

        [NonSerialized]
        private Collider2D _collider2D;
        public new Collider2D collider2D => _collider2D ? _collider2D : _collider2D = GetComponent<Collider2D>();

        [NonSerialized]
        private ConstantForce _constantForce;
        public new ConstantForce constantForce => _constantForce ? _constantForce : _constantForce = GetComponent<ConstantForce>();

        [NonSerialized]
        private HingeJoint _hingeJoint;
        public new HingeJoint hingeJoint =>
            _hingeJoint ? _hingeJoint : _hingeJoint = GetComponent<HingeJoint>();

        [NonSerialized]
        private Light _light;
        public new Light light =>
            _light ? _light : _light = GetComponent<Light>();

        [NonSerialized]
        private ParticleSystem _particleSystem;
        public new ParticleSystem particleSystem =>
            _particleSystem ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        [NonSerialized]
        private Renderer _renderer;
        public new Renderer renderer => _renderer ? _renderer : _renderer = GetComponent<Renderer>();

        [NonSerialized]
        private Rigidbody _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        [NonSerialized]
        private Rigidbody2D _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D : _rigidbody2D = GetComponent<Rigidbody2D>();
        #endregion

        public static void Spawn(AssetReference original, Action<UObject> callback = null, int pool = 0)
        {
            Spawn(original, Vector3.zero, Quaternion.identity, null, callback, pool);
        }

        public static void Spawn(AssetReference original, Transform parent, Action<UObject> callback = null, int pool = 0)
        {
            Spawn(original, Vector3.zero, Quaternion.identity, parent, callback, pool);
        }

        public static void Spawn(AssetReference original, Transform parent, bool instantiateWorldSpace, Action<UObject> callback = null, int pool = 0)
        {
            var pos = instantiateWorldSpace ? Vector3.zero : parent.position;
            var rot = instantiateWorldSpace ? Quaternion.identity : parent.rotation;
            
            Spawn(original, pos, rot, null, callback, pool);
        }
        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Action<UObject> callback = null, int pool = 0)
        {
            Spawn(original, position, rotation, null, callback, pool);
        }
        
        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Transform parent, Action<UObject> callback = null, int pool = 0)
        {
            if (pool <= 0 && !poolManager.HasPool(original))
            {
                original.InstantiateAsync(position, rotation, parent).Completed += handle => callback.Invoke(handle.Result);
                return;
            }
            else
            {
                poolManager.GetOrcreateInstanceOf(original, OnSpawned, pool);
                void OnSpawned(GameObject go)
                {
                    Transform transform = go.transform;
                    transform.SetPositionAndRotation(position, rotation);
                    transform.parent = parent;
                    callback?.Invoke(go);
                }
            }
            
            public static void Release(GameObject go) => PoolManager.Release(go);
        }
    }