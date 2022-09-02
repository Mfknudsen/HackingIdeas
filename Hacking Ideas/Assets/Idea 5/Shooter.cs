using UnityEngine;

namespace Idea_5
{
    public class Shooter : MonoBehaviour
    {
        public Color color;
        public bool rightHand;

        [SerializeField] private float triggerThreshold;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform spawnPoint;

        private bool shot;

        private PlayerInput input;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void OnEnable()
        {
            this.input = new PlayerInput();
            this.input.Player.Enable();

            (rightHand ? this.input.Player.GrabRight : this.input.Player.GrabLeft).performed += context =>
            {
                float value = context.ReadValue<float>();
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (this.shot && value < this.triggerThreshold)
                    this.shot = false;
                else if (!this.shot && value > this.triggerThreshold)
                {
                    this.shot = true;
                    Shoot();
                }
            };
        }

        private void Shoot()
        {
            GameObject obj = Instantiate(this.bulletPrefab);
            Vector3 spawnPos = spawnPoint.position;
            obj.transform.position = spawnPos;
            obj.transform.LookAt(spawnPos + transform.forward, Vector3.up);

            obj.GetComponent<Bullet>().color = this.color;

            Renderer r = obj.GetComponent<Renderer>();
            r.material.EnableKeyword("_Color");
            r.material.SetColor(ColorID, this.color);
        }
    }
}