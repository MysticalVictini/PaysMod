using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace PaysPrimitierMod
{

    public class Startup
    {

        public Startup() { 
            
        }
    }

    public class Hunger : MonoBehaviour
    {
        Transform player;
        Transform camera;
        Texture2D hunger;
        void Start()
        {
            player = GameObject.Find("Player").transform;
            camera = GameObject.Find("HandyCamera").transform;
            hunger = (Texture2D)PMAPI.CustomAssetsManager.CustomAssetsManager.LoadEmbeddedResource("PaysMod/Textures/hunger.png");

            SetupDisplay(camera.GetChild(0), camera.GetChild(0).GetChild(1), false);
        }

        void SetupDisplay(Transform menu, Transform LifeGauge, bool isVR)
        {
            Transform hungerBar = Instantiate(LifeGauge.gameObject).transform;
            Destroy(hungerBar.GetComponent<PlayerLifeGauge>());
            hungerBar.parent = menu;
            Color col = Color.magenta;
            Transform hungerBarTransform;
            if (isVR)
            {
                hungerBarTransform = hungerBar.GetChild(1).GetChild(0);
                hungerBarTransform.GetComponent<SpriteRenderer>().color = col;
            }
            else
            {
                hungerBarTransform = hungerBar.GetChild(1);
                hungerBarTransform.GetComponent<UnityEngine.UI.Image>().color = col;
            }

            hungerBar.localPosition = hungerBar.localPosition - new Vector3(0, 100f, 0);

        }
    }


}
