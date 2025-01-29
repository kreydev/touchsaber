using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Input;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using VRUIControls;
using System.Reflection;
using System.IO;


public enum ctrlMode { touch, keybmouse, controller, arrows}

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
        //Text dbgtxt;
        Camera goodcam;
        //GameObject cube;
        int s1 = 0;
        int s2 = 1;
        List<Saber> sabers2 = new List<Saber>();
        Saber[] sabers;

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake() {

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
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
        void FixedUpdate()
        {
            if (bi == null)
            {
                GameObject go = new GameObject("input", typeof(BaseInput), typeof(Canvas));
                bi = go.GetComponent<BaseInput>();
                goodcam = new GameObject("good", typeof(Camera)).GetComponent<Camera>();
                goodcam.CopyFrom(GameObject.Find("Cam").GetComponent<Camera>());

            }
            //foreach (Saber s in sabers) {
            //    Debug.Log(s.name);
            //    print("zawg");
            //    Application.Quit();
            //}
        }
        void Update() {
            //if (cube == null) {
            //    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.name = "holder";
            //    cube.transform.localScale = new Vector3(.02f, .02f, .02f);
            //}
            //if (FindObjectsOfType<VRController>().Length > 0){

            //    foreach (VRController ctrl in FindObjectsOfType<VRController>()) {
            //        //ctrl.transform.LookAt(cube.transform);
            //        ctrl.transform.rotation = Quaternion.identity;
            //        ctrl.transform.SetParent(null, false);
            //        cube.transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z));
            //        //ctrl.transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z));
            //    }
            //}
            if (!UnityEngine.Input.GetKey(KeyCode.F1) && !UnityEngine.Input.GetKey(KeyCode.F2)) {
                goodcam.transform.position = new Vector3(0.00f, 1.20f, -2.20f);
            }

            Vector2 arrows = new Vector2(bi.GetAxisRaw("Horizontal"), bi.GetAxisRaw("Vertical"));
            if (UnityEngine.Input.GetKey(KeyCode.F2)) {
                goodcam.transform.localEulerAngles += new Vector3(-arrows.y * 3f, arrows.x * 3f, 0);
            } else if (!UnityEngine.Input.GetKey(KeyCode.F1)) {
                goodcam.transform.rotation = Quaternion.identity;
            }

            sabers = FindObjectsOfType<Saber>();
            sabers2.Clear();
            foreach (Saber saber in sabers) {
                if (Vector3.Distance(saber.transform.position, goodcam.transform.position) < 4) {
                    sabers2.Add(saber);
                }
            }

            string objs = "---===---\n";
            foreach (GameObject g in FindObjectsOfType<GameObject>())
            {
                objs += g.name + "\n";
            }
            objs += "---===---";

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                StandardLevelReturnToMenuController pauser = FindObjectOfType<StandardLevelReturnToMenuController>();
                pauser.ReturnToMenu();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.C)) {
                Debug.Log("SABERS 2: " + sabers2.Count);
                Debug.Log("SABERS: " + sabers.Length);
            }

            if (sabers2.Count == 0 && UnityEngine.Input.GetKey(KeyCode.F1) && !UnityEngine.Input.GetKey(KeyCode.F2)) {
                goodcam.transform.position += new Vector3(0, arrows.y * .2f, arrows.x * .2f);
            }

            if (sabers2.Count > 0) {
                // arrow keys
                if (Plugin.mode == ctrlMode.arrows)
                {
                    sabers2[0].transform.rotation = Quaternion.Slerp(sabers2[0].transform.rotation, Quaternion.Euler(new Vector3(-arrows.y * 60, arrows.x * 60, 0)), .5f);
                    sabers2[0].transform.position = goodcam.transform.position + new Vector3(.2f, -.5f, .5f);
                    sabers2[1].transform.rotation = Quaternion.Slerp(sabers2[1].transform.rotation, Quaternion.Euler(new Vector3(-arrows.y * 60, arrows.x * 60, 0)), .5f);
                    sabers2[1].transform.position = goodcam.transform.position + new Vector3(-.2f, -.5f, .5f);
                }

                // touchscreen
                else if (Plugin.mode == ctrlMode.touch)
                {
                    for (int i = 0; i < bi.touchCount; i++)
                    {
                        if (i == 0)
                        {
                            sabers2[1].transform.LookAt(goodcam.transform);
                            sabers2[1].transform.Rotate(180, 180, 180);
                            sabers2[1].transform.eulerAngles = new Vector3((bi.GetTouch(i).position.y - Screen.height / 3) * -.2f, (bi.GetTouch(i).position.x - 3 * Screen.width / 8) * .1f, 0);
                            sabers2[1].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.GetTouch(i).position.x, bi.GetTouch(i).position.y, -goodcam.transform.position.z)) + new Vector3(0, 0, .2f);
                            sabers2[1].transform.localPosition += new Vector3(0, 0, -.5f);
                        }
                        if (i == 1)
                        {
                            sabers2[0].transform.LookAt(goodcam.transform);
                            sabers2[0].transform.Rotate(180, 180, 180);
                            sabers2[0].transform.eulerAngles = new Vector3((bi.GetTouch(i).position.y - Screen.height / 3) * -.2f, (bi.GetTouch(i).position.x - 5 * Screen.width / 8) * .1f, 0);
                            sabers2[0].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.GetTouch(i).position.x, bi.GetTouch(i).position.y, -goodcam.transform.position.z)) + new Vector3(0, 0, .2f);
                            sabers2[0].transform.localPosition += new Vector3(0, 0, -.5f);
                        }
                    }
                }

                // controller
                else if (Plugin.mode == ctrlMode.controller)
                {
                    Vector2 lstick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    Vector2 rstick = new Vector2(Input.GetAxisRaw("Axis3"), Input.GetAxisRaw("Axis4"));
                    sabers2[0].transform.rotation = Quaternion.Slerp(sabers2[0].transform.rotation, Quaternion.Euler(new Vector3(-lstick.y * 60, lstick.x * 60, 0)), .5f);
                    sabers2[0].transform.position = goodcam.transform.position + new Vector3(.2f, -.5f, .5f);
                    sabers2[1].transform.rotation = Quaternion.Slerp(sabers2[1].transform.rotation, Quaternion.Euler(new Vector3(-rstick.y * 60, rstick.x * 60, 0)), .5f);
                    sabers2[1].transform.position = goodcam.transform.position + new Vector3(-.2f, -.5f, .5f);
                }

                // just mouse + keyboard
                else if (Plugin.mode == ctrlMode.keybmouse) {
                    if (GetKey(KeyCode.S))
                    {
                        s2 = 0;
                        s1 = 1;
                    }
                    else
                    {
                        s2 = 1;
                        s1 = 0;
                    }
                    sabers2[s1].transform.LookAt(goodcam.transform);
                    sabers2[s1].transform.Rotate(180, 180, 180);
                    if (sabers2.Count != 1) {
                        sabers2[s2].transform.LookAt(goodcam.transform);
                        sabers2[s2].transform.Rotate(180, 180, 180);
                    }
                    if (GetKey(KeyCode.A) && GetKey(KeyCode.D))
                    {
                        sabers2[s1].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f + 50f, 0);
                        sabers2[s1].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(1f, 0, .2f);

                        if (sabers2.Count != 1)
                        {
                            sabers2[s2].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f - 50f, 0);
                            sabers2[s2].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(-1f, 0, .2f);
                        }
                    }
                    else if (GetKey(KeyCode.A))
                    {
                        sabers2[s1].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                        sabers2[s1].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(0, .2f, .2f);

                        if (sabers2.Count != 1)
                        {
                            sabers2[s2].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                            sabers2[s2].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(0, -.2f, .2f);
                        }
                    }
                    else if (GetKey(KeyCode.D))
                    {
                        sabers2[s2].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                        sabers2[s2].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(0, .2f, .2f);

                        if (sabers2.Count != 1)
                        {
                            sabers2[s1].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                            sabers2[s1].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(0, -.2f, .2f);
                        }
                    }
                    else
                    {
                        sabers2[s1].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                        sabers2[s1].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(.2f, 0, .2f);

                        if (sabers2.Count != 1)
                        {
                            sabers2[s2].transform.eulerAngles = new Vector3((bi.mousePosition.y - Screen.height / 3) * -.2f, (bi.mousePosition.x - 1 * Screen.width / 2) * .1f, 0);
                            sabers2[s2].transform.position = goodcam.ScreenToWorldPoint(new Vector3(bi.mousePosition.x, bi.mousePosition.y, -goodcam.transform.position.z)) + new Vector3(-.2f, 0, .2f);
                        }
                    }
                }
            }
        }
    }
}