using Dummiesman;
using Il2Cpp;
using MelonLoader;
using PMAPI.CustomAssetsManager;
using PMAPI.CustomSubstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PaysPrimitierMod
{
    public class LineRender : MonoBehaviour
    {

        LineRenderer setRend;
        static Material createdMaterial;
        public LineRender(Vector3 startPos, Vector3 endPos)
        {
            GameObject newRender = new GameObject();
            newRender.name = "Renderer";
            newRender.AddComponent<LineRenderer>();

            setRend = newRender.GetComponent<LineRenderer>();
            setRend.widthMultiplier = 0.1f;
            setRend.positionCount = 2;
            setRend.SetPosition(0, startPos);
            setRend.SetPosition(1, endPos);
            setRend.material = CustomSubstanceManager.errorMaterial;
        }

        public LineRender(Vector3 startPos, Vector3 endPos, UnityEngine.Color color)
        {
            GameObject newRender = new GameObject();
            newRender.name = "Renderer";
            newRender.AddComponent<LineRenderer>();

            setRend = newRender.GetComponent<LineRenderer>();
            setRend.widthMultiplier = 0.1f;
            setRend.positionCount = 2;
            setRend.SetPosition(0, startPos);
            setRend.SetPosition(1, endPos);
            if (!createdMaterial)
            {
                createdMaterial = new Material(CustomSubstanceManager.errorMaterial) { color = color, name = "LineRendererMaterial" };
                CustomMaterialManager.RegisterMaterial(createdMaterial);
            }
            setRend.material = createdMaterial;
            setRend.material.color = color;

        }


        public void updatePosition(Vector3 startPos, Vector3 endPos)
        {
            setRend.SetPosition(0, startPos);
            setRend.SetPosition(1, endPos);
        }

        public void updatePosition(Vector3 endPos)
        {
            setRend.SetPosition(1, endPos);
        }

        public void updatePosition(int i, Vector3 pos)
        {
            setRend.SetPosition(i, pos);
        }
    }

    public class ChickenBeh : SlimeBase
    {

        OBJLoader testObject = new OBJLoader();

        public ChickenBeh(IntPtr ptr) : base(ptr)
        {
            // Requesting load of cube save data
            if (model == null)
            {
                model = testObject.getMesh("PaysMod/Models/chicken.obj");
            }
        }

        static Mesh model;
        Rigidbody body;
        float jumpPower = 1;
        CubeBase cube;
        LineRender test;
        //new CubeBase cubeBase;
        static float maxSize = 0.45f;
        void Start()
        {
            if (model == null)
            {
                model = testObject.getMesh("PaysMod/Models/chicken.obj");
            }
            this.transform.GetComponent<MeshFilter>().mesh = model;
            this.transform.GetComponent<BoxCollider>().size = new Vector3(1, 1.65f, 1);
            jumpPower = this.transform.localScale.y;
            cube = this.transform.GetComponent<CubeBase>();
            cube.CollideEvent.AddListener((UnityEngine.Events.UnityAction<CubeBase, float, float>)collide);
            cube.splitType = CubeBase.SplitType.Eliminate;
            body = this.transform.parent.GetComponent<Rigidbody>();
            //test = new LineRender(this.transform.position, this.transform.position, UnityEngine.Color.cyan);
        }

        public void collide(CubeBase itemHit, float speed, float force)
        {
            if (itemHit == null) return;
            if (foods.Contains(itemHit.substance))
            {
                int divideValue = 4;
                if (itemHit.life > 100)
                {
                    divideValue = 3;
                }
                else if (itemHit.life > 500)
                {
                    divideValue = 4;
                }
                else if (itemHit.life > 3000)
                {
                    divideValue = 5;
                }
                else if (itemHit.life > 10000)
                {
                    divideValue = 10;
                }
                float val = itemHit.life / divideValue;
                if (val > 5000)
                {
                    val = 5000;
                }
                if (SubstanceManager.GetParameter(itemHit.substance).isEdible)
                {
                    val *= 1.4f;
                }

                Destroy(itemHit.transform.parent.gameObject);
                cube.Recover(val, true);
                float growSize = this.transform.localScale.magnitude + (val / 400);
                if (growSize > maxSize)
                {
                    growSize = growSize - ((growSize / maxSize - 1f) * maxSize);
                }
                growSize -= this.transform.localScale.magnitude;
                this.transform.localScale += new Vector3(growSize / 3, growSize / 3, growSize / 3);
                jumpPower = this.transform.localScale.y;
            }
        }

        public static Il2CppSystem.Collections.Generic.List<Substance> foods = new Il2CppSystem.Collections.Generic.List<Substance>();

        private bool isGrounded()
        {
            //test.updatePosition(this.transform.position - this.transform.up * 1.65f * this.transform.localScale.y*0.4f, this.transform.position - this.transform.up * 1.65f * this.transform.localScale.y * 0.4f + this.transform.up * (jumpPower*-0.2f));
            return Physics.Raycast(this.transform.position - this.transform.up * 1.65f * this.transform.localScale.y * 0.4f, this.transform.up * -1, jumpPower * 0.2f);
        }

        private bool isGrounded(Vector3 dir)
        {
            //test.updatePosition(this.transform.position, dir);
            return Physics.Raycast(this.transform.position, dir, jumpPower);
        }

        bool isIsolated = false;
        private void updateIsIsolated()
        {
            body = this.transform.parent.GetComponent<Rigidbody>();
            isIsolated = this.transform.parent.childCount == 1;
        }

        private void jumpToward(Vector3 pos, float jumpHeight)
        {
            if (!isIsolated)
            {
                return;
            }
            this.transform.parent.GetComponent<Rigidbody>().velocity = pos - this.transform.position + new Vector3(0, jumpHeight, 0);
        }

        void OnDestroy()
        {
            if (this.GetComponent<CubeBase>() != null && this.GetComponent<CubeBase>().life <= 0)
            {
                CubeGenerator.GenerateCube(this.transform.position, this.transform.localScale * 0.8f, PaysMod.rawChicken);
            }
        }

        float flipN(float N)
        {
            if (N < 0) return N * -1;
            return N;
        }

        bool isXorZTooHigh(Vector3 rot)
        {
            if (flipN(rot.x) >= 90 && flipN(rot.x) < 270 || flipN(rot.z) >= 90 && flipN(rot.z) < 270)
            {
                return true;
            }
            return false;
        }

        int count = 0;
        int nextCount = 220;
        static System.Random rand = new System.Random();
        bool flipping = false;
        private void FixedUpdate()
        {
            Vector3 rot = this.transform.parent.localEulerAngles;
            count++;
            if (count > nextCount)
            {
                updateIsIsolated();
                if (this.isGrounded())
                {
                    jumpToward(this.transform.position + this.transform.forward * 5.5f * jumpPower, 5f * jumpPower);
                }
                count = 0;
                nextCount = rand.Next(75, 250);
            }
            else if (count == nextCount / 2)
            {
                updateIsIsolated();
                if (isIsolated)
                {
                    this.transform.parent.rotation *= Quaternion.Euler(0, ((float)rand.Next(0, 3000) - 1500) / 100, 0);
                }
                if (rand.Next(0, 75) == 71 && this.transform.localScale.y > 0.15f)
                {
                    layEgg();
                }
            }
            else if (count == nextCount / 4 || count == (nextCount / 4) * 3 && isGrounded(this.transform.position - new Vector3(0, 0.2f, 0)))
            {
                updateIsIsolated();
                if (isIsolated)
                {
                    if (isXorZTooHigh(rot))
                    {
                        float x = 0;
                        float z = 0;
                        if (flipN(rot.x) > 35 && flipN(rot.x) < 325)
                        {
                            z = 25;
                            if (flipN(rot.x) > 180)
                            {
                                z = -25;
                            }
                        }
                        if (flipN(rot.z) > 35 && flipN(rot.z) < 325)
                        {
                            x = 25;
                            if (flipN(rot.z) > 180)
                            {
                                x = -25;
                            }
                        }
                        jumpToward(this.transform.position + new Vector3(0, 3, 0), 2f * jumpPower);
                        body.AddTorque(new Vector3(x, 0, z));
                        flipping = true;
                    }
                }
                else
                {
                    flipping = false;
                }
            }
            else if (flipping)
            {
                updateIsIsolated();
                if (body.angularVelocity.x == 0 && body.angularVelocity.z == 0 || !isIsolated)
                {
                    flipping = false;
                }
                else
                {
                    if ((flipN(rot.x) < 25 || flipN(rot.x) > 335) && body.angularVelocity.z > 0)
                    {
                        body.angularVelocity = body.angularVelocity - new Vector3(0, 0, body.angularVelocity.z);
                    }
                    if ((flipN(rot.z) < 25 || flipN(rot.z) > 335) && body.angularVelocity.x > 0)
                    {
                        body.angularVelocity = body.angularVelocity - new Vector3(body.angularVelocity.x, 0, 0);
                    }
                }
            }
            else if (isIsolated && !flipping && flipN(rot.x) > 15 && flipN(rot.x) < 345 && body.angularVelocity.z > 0)
            {
                body.angularVelocity = body.angularVelocity - new Vector3(0, 0, body.angularVelocity.z);
            }
            else if (isIsolated && !flipping && flipN(rot.z) > 15 && flipN(rot.z) < 345 && body.angularVelocity.x > 0)
            {
                body.angularVelocity = body.angularVelocity - new Vector3(body.angularVelocity.x, 0, 0);
            }
        }

        private void layEgg()
        {
            CubeGenerator.GenerateCube(this.transform.position - this.transform.forward * 0.3f, this.transform.localScale * 0.65f, PaysMod.egg);
        }

        public void OnConnectionChanged()
        {
            MelonLogger.Msg("Chicken Connection");
            updateIsIsolated();
        }

    }

    public class EggBeh : MonoBehaviour
    {
        OBJLoader testObject = new OBJLoader();
        public EggBeh(IntPtr ptr) : base(ptr)
        {
            if (model == null)
            {
                model = testObject.getMesh("PaysMod/Models/egg.obj");
            }
        }

        bool heated = false;
        CubeBase cubeBase;
        static Mesh model;
        //CubeBase cubeBase;

        void Start()
        {
            if (model == null)
            {
                model = testObject.getMesh("PaysMod/Models/egg.obj");
            }
            // Get the cube base
            this.transform.GetComponent<MeshFilter>().mesh = model;
            float size = this.transform.localScale.x;
            this.transform.localScale = new Vector3(size, size * 0.85f, size);
            this.transform.GetComponent<BoxCollider>().size = new Vector3(0.3f, 0.35f, 0.3f);
            cubeBase = this.transform.GetComponent<CubeBase>();
            cubeBase.splitType = CubeBase.SplitType.Eliminate;
            if (cubeBase.substance == Substance.Apple)
            {
                cubeBase.substance = PaysMod.egg;
            }
        }

        void OnDestroy()
        {
            if (this.GetComponent<CubeBase>() != null && this.GetComponent<CubeBase>().life <= 0)
            {
                //CubeGenerator.GenerateCube(this.transform.position, this.transform.localScale * 0.8f, PaysMod.rawChicken);
            }
        }

        void OnInitialize()
        {

        }

        int count = 0;
        int timesNotHeated = 0;
        private void FixedUpdate()
        {

            count++;
            if (count > 120 && !heated)
            {
                if (cubeBase.heat.Temperature > 580f)
                {
                    heated = true;
                    this.transform.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(1, 0.85f, 0.77f);
                }
                else
                {
                    timesNotHeated++;
                    if (timesNotHeated > 100)
                    {
                        Destroy(this.transform.parent.gameObject);
                    }
                }
                count = 0;
            }
            else if (count > 8000 && heated)
            {
                CubeGenerator.GenerateCube(this.transform.position, this.transform.localScale, PaysMod.chicken);
                Destroy(this.transform.parent.gameObject);
            }

        }

    }

    public class RawChickenBeh : MonoBehaviour
    {
        public RawChickenBeh(IntPtr ptr) : base(ptr)
        {
            // Requesting load of cube save data
        }


        CubeBase cubeBase;
        //CubeBase cubeBase;

        void OnInitialize()
        {
            cubeBase = this.transform.GetComponent<CubeBase>();
        }

        int count = 0;
        private void FixedUpdate()
        {
            count++;
            if (count > 80)
            {
                if (cubeBase.heat.Temperature > 440f)
                {
                    cubeBase.ChangeSubstance(PaysMod.cookedChicken);
                    Destroy(this);
                }
                count = 0;
            }

        }

    }

    public class LaserBeh : ElectricPart
    {
        public LaserBeh(IntPtr ptr) : base(ptr)
        {
            // Requesting load of cube save data

        }

        void OnInitialize()
        {
            // Get the cube base
            cubeBase = GetComponent<CubeBase>();
            CustomAssetsManager.fixTexture(this.transform);
        }

        public override void OnElectricityUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10f))
            {
                conductPower = ElectricPart.PartType.Conductor;
            }
            else
            {
                conductPower = ElectricPart.PartType.Receiver;
            }
        }

        PartType conductPower = ElectricPart.PartType.Receiver;
        public override ElectricPart.PartType GetPartType(CubeConnector sourceConnector)
        {
            return conductPower;
        }

        // Saved variable
        // When cube is going to save it's data return the data that we want to save

    }

}
