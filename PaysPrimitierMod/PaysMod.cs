using MelonLoader;
using PMAPI;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using PMAPI.CustomSubstances;
using PMAPI.OreGen;
using UnityEngine.SceneManagement;
using System.Text.Json;
using Il2CppInterop.Runtime.Startup;
using MelonLoader.InternalUtils;
using Il2Cpp;
using System.Xml.Linq;
using static Il2CppMono.Globalization.Unicode.SimpleCollator;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Reflection;
using System.Drawing;
using PMAPI.CustomAssetsManager;
using UnityEngine.Networking;
using System.Collections;
using Dummiesman;
using static Il2Cpp.CubeGenerator;
using Il2CppSystem.Collections.Generic;

namespace PaysPrimitierMod
{
    public class PaysMod : MelonMod
    {
        public class OurData
        {
            public int Test { get; set; }

            public OurData(int test)
            {
                Test = test;
            }
        }

        Substance laser;
        public static Substance chicken;
        public static Substance egg;
        public static Substance rawChicken;
        public static Substance cookedChicken;

        public override void OnInitializeMelon()
        {
            // Init PMAPI
            PMAPIModRegistry.InitPMAPI(this);

            // Register in our behaviour in IL2CPP so the game knows about it
            ClassInjector.RegisterTypeInIl2Cpp<LaserBeh>(new RegisterTypeOptions
            {
                Interfaces = new[] { typeof(ICubeBehavior)}
            });

            ClassInjector.RegisterTypeInIl2Cpp<ChickenBeh>(new RegisterTypeOptions
            {
                Interfaces = new[] { typeof(ICubeBehavior) }
            });

            ClassInjector.RegisterTypeInIl2Cpp<EggBeh>(new RegisterTypeOptions
            {
                Interfaces = new[] { typeof(ICubeBehavior) }
            });

            ClassInjector.RegisterTypeInIl2Cpp<RawChickenBeh>(new RegisterTypeOptions
            {
                Interfaces = new[] { typeof(ICubeBehavior) }
            });

            // Registering modded substances
            RegisterLaser();
            RegisterChicken();
            RegisterEgg();
            RegisterChickenRawMeat();
            RegisterChickenMeat();
            // Registering our substances in ore generation

            /**
            CustomOreManager.RegisterCustomOre(rood, new CustomOreManager.CustomOreParams
            {
                chance = 0.05f,
                substanceOverride = Substance.Stone | Substance.MoonRock,
                maxSize = 0.5f,
                minSize = 0.1f,
                alpha = 1f
            });
            **/
        }

        // Gets called just when the world was loaded
        public void OnWorldWasLoaded()
        {
            // Outputting mod data. The question mark means that nothing will be outputted if mod data doesn't exist (data == null)
            //MelonLogger.Msg("Mod data is {0}", ExtDataManager.GetData<OurData>()?.Test);
            MelonLogger.Msg("loaded?");
        }

        int count = 0;
        Scene oldScene;
        public void FixedUpdate()
        {
            count++;
            if (count == 10)
            {
                count = 0;
                Scene scene = GameObject.GetScene(0);
                if (scene != oldScene)
                {
                    oldScene = scene;
                    MelonLogger.Msg(scene.name);
                }
            }
            
        }
        
        static Texture testTexture = new Texture2D(0, 0);
        static Texture chickenTexture = new Texture2D(0, 0);
        
        void RegisterLaser()
        {
            Material cmat = new(SubstanceManager.GetMaterial("Glass"))
            {
                name = "Laser",
                color = new UnityEngine.Color(0.9f,0,0,0.6f)
            };

            CustomMaterialManager.RegisterMaterial(cmat);

            var param = SubstanceManager.GetParameter(Substance.Iron).MemberwiseClone().Cast<SubstanceParameters.Param>();
            param.displayNameKey = "SUB_LASER";
            param.material = cmat.name;

            CustomSubstanceParams prams = new CustomSubstanceParams
            {
                enName = "Laser",
                ruName = "Laser",
                jpName = "Laser",
                zhHansName = "Laser",
                deName = "Laser",
                esName = "Laser",
                frName = "Laser",
                behInit = (cb) =>
                {
                    // Adding test behavior
                    var beh = cb.gameObject.AddComponent<LaserBeh>();
                    return beh;
                }
            };
            laser = CustomSubstanceManager.RegisterSubstance("laser", param, prams);


            // Make laser from LED and Glass or LED and AncientAlloy
            CubeMerge.compoundablePairs.Add(new Il2CppSystem.ValueTuple<Substance, Substance>(Substance.LED, Substance.Glass), new Il2CppSystem.ValueTuple<float, Substance, float>(1f, laser, 1f));
            CubeMerge.compoundablePairs.Add(new Il2CppSystem.ValueTuple<Substance, Substance>(Substance.LED, Substance.AncientAlloy), new Il2CppSystem.ValueTuple<float, Substance, float>(1f, laser, 1f));

            //Make LED from Glass and AncientAlloy
            CubeMerge.compoundablePairs.Add(new Il2CppSystem.ValueTuple<Substance, Substance>(Substance.Glass, Substance.AncientAlloy), new Il2CppSystem.ValueTuple<float, Substance, float>(1f, Substance.LED, 1f));
        }

        void RegisterChicken()
        {
            chickenTexture = CustomAssetsManager.LoadEmbeddedResource("PaysMod/Textures/chicken.png");
            Material cmat = new(SubstanceManager.GetMaterial("Stone"))
            {
                name = "Chicken",
                color = new UnityEngine.Color(1, 1, 1),
                mainTexture = chickenTexture
            };
            CustomMaterialManager.RegisterMaterial(cmat);

            var param = SubstanceManager.GetParameter(Substance.Slime).MemberwiseClone().Cast<SubstanceParameters.Param>();
            param.displayNameKey = "SUB_CHICKEN";
            param.material = cmat.name;

            CustomSubstanceParams prams = new CustomSubstanceParams
            {
                enName = "Chicken",
                ruName = "Chicken",
                jpName = "Chicken",
                zhHansName = "Chicken",
                deName = "Chicken",
                esName = "Chicken",
                frName = "Chicken",
                behInit = (cb) =>
                {
                    // Adding test behavior
                    var beh = cb.gameObject.AddComponent<ChickenBeh>();
                    return beh;
                }
            };
            chicken = CustomSubstanceManager.RegisterSubstance("chicken", param, prams);
            Substance[] foods = new Substance[] {Substance.Apple, Substance.Wheat, Substance.WheatStalk, Substance.Bread, Substance.Slime, Substance.RedSlime};
            for (int i = 0; i<foods.Length; i++)
            {
                ChickenBeh.foods.Add(foods[i]);
            }
        }

        void RegisterEgg()
        {
            Material cmat = new(SubstanceManager.GetMaterial("Stone"))
            {
                name = "Egg",
                color = new UnityEngine.Color(1, 0.96f, 0.9f)
            };

            CustomMaterialManager.RegisterMaterial(cmat);
            var param = SubstanceManager.GetParameter(Substance.Slime).MemberwiseClone().Cast<SubstanceParameters.Param>();
            param.displayNameKey = "SUB_EGG";
            param.material = cmat.name;

            CustomSubstanceParams prams = new CustomSubstanceParams
            {
                enName = "Egg",
                ruName = "Egg",
                jpName = "Egg",
                zhHansName = "Egg",
                deName = "Egg",
                esName = "Egg",
                frName = "EggV2",
                behInit = (cb) =>
                {
                    // Adding test behavior
                    var beh = cb.gameObject.AddComponent<EggBeh>();
                    return beh;
                }
            };
            egg = CustomSubstanceManager.RegisterSubstance("egg", param, prams);

            //Make an egg from apple and wheat
            CubeMerge.compoundablePairs.Add(new Il2CppSystem.ValueTuple<Substance, Substance>(Substance.Apple, Substance.Wheat), new Il2CppSystem.ValueTuple<float, Substance, float>(1f, egg, 1f));
        }

        void RegisterChickenRawMeat()
        {
            Material cmat = new(SubstanceManager.GetMaterial("Stone"))
            {
                name = "RawChickenMeat",
                color = new UnityEngine.Color(1, 0.768f, 0.69f)
            };
            CustomMaterialManager.RegisterMaterial(cmat);
            var param = SubstanceManager.GetParameter(Substance.Slime).MemberwiseClone().Cast<SubstanceParameters.Param>();
            param.displayNameKey = "SUB_RAWCHICKEN";
            param.material = cmat.name;

            CustomSubstanceParams prams = new CustomSubstanceParams
            {
                enName = "Raw Chicken",
                ruName = "Raw Chicken",
                jpName = "Raw Chicken",
                zhHansName = "Raw Chicken",
                deName = "Raw Chicken",
                esName = "Raw Chicken",
                frName = "Raw Chicken",
                behInit = (cb) =>
                {
                    var beh = cb.gameObject.AddComponent<RawChickenBeh>();
                    return beh;
                }
            };

            rawChicken = CustomSubstanceManager.RegisterSubstance("rawchicken", param, prams);
        }

        void RegisterChickenMeat()
        {
            Material cmat = new(SubstanceManager.GetMaterial("Stone"))
            {
                name = "CookedChickenMeat",
                color = new UnityEngine.Color(0.78f, 0.42f, 0.31f)
            };
            CustomMaterialManager.RegisterMaterial(cmat);
            var param = SubstanceManager.GetParameter(Substance.CookedSlime).MemberwiseClone().Cast<SubstanceParameters.Param>();
            param.displayNameKey = "SUB_COOKEDCHICKEN";
            param.material = cmat.name;
            param.recovery = param.recovery * 1.7f;

            CustomSubstanceParams prams = new CustomSubstanceParams
            {
                enName = "Cooked Chicken",
                ruName = "Cooked Chicken",
                jpName = "Cooked Chicken",
                zhHansName = "Cooked Chicken",
                deName = "Cooked Chicken",
                esName = "Cooked Chicken",
                frName = "Cooked Chicken"
            };

            cookedChicken = CustomSubstanceManager.RegisterSubstance("cookedchicken", param, prams);
        }

        PlayerMovement move { get { return GameObject.Find("XR Origin").GetComponent<PlayerMovement>(); } }
        public override void OnUpdate()
        {
            // Spawning blud above our head
            if (Input.GetKeyDown(KeyCode.L))
            {
                // Getting player position
                var mv = move;

                float scale = 0.25f;
                // Generating the cube
                CubeGenerator.GenerateCube(mv.cameraTransform.position + new Vector3(0f, 10f, 1f), new Vector3(scale, scale, scale), chicken);
            }
            else if (Input.GetKeyDown(KeyCode.R)) {
                var mv = move;

                float scale = 0.25f;
                CubeGenerator.GenerateCube(mv.cameraTransform.position + new Vector3(0f, 8f, 1f), new Vector3(scale, scale*4, scale), Substance.Iron);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {

                float scale = 0.08f;
                CubeGenerator.GenerateCube(move.cameraTransform.position + new Vector3(0f, 8f, 1f), new Vector3(scale, scale, scale), Substance.Apple);
            }
            // Writing test data that will be stored in save file
            //if (Input.GetKeyDown(KeyCode.X))
                //ExtDataManager.SetData(new OurData(UnityEngine.Random.RandomRangeInt(1, 100)));
        }
    }

}
