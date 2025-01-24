using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

namespace touchsaber
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class touchsaberController : MonoBehaviour
    {
        public static touchsaberController Instance { get; private set; }

        BaseInput bi;

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
        void FixedUpdate() {
            if (bi == null) {
                GameObject go = new GameObject("input", typeof(BaseInput));
                bi = go.GetComponent<BaseInput>();
            }
            //Debug.Log("VERT: " + bi.GetAxisRaw("Vertical") + "HORIZ: " + bi.GetAxisRaw("Horizontal"));
            Vector2 arrows = new Vector2(bi.GetAxisRaw("Horizontal"), bi.GetAxisRaw("Vertical"));
            //Debug.Log("Ping");
            //Debug.Log(Resources.)
            Saber[] sabers = FindObjectsOfType<Saber>();
            Camera camera = Camera.main;
            for (int i = 0; i < bi.touchCount; i++) {
                Debug.Log(i + ": " + bi.GetTouch(i).position); // 0,0 is bottom left
            }

            if (sabers.Length > 0) {
                camera.transform.rotation = Quaternion.identity;
                sabers[0].transform.rotation = Quaternion.Slerp(sabers[0].transform.rotation, Quaternion.Euler(new Vector3(-arrows.y * 60, arrows.x * 60, 0)), .5f);
                sabers[0].transform.position = camera.transform.position + new Vector3(.2f, -.5f, .2f);
                sabers[1].transform.rotation = Quaternion.Slerp(sabers[1].transform.rotation, Quaternion.Euler(new Vector3(-arrows.y * 60, arrows.x * 60, 0)), .5f);
                sabers[1].transform.position = camera.transform.position + new Vector3(-.2f, -.5f, .2f);
            }
        }
    }
}
