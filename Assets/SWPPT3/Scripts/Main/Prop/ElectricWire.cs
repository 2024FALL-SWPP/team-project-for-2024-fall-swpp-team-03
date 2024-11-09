using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ElectricWire : StatefulProp
    {
        public Color emissionColor = Color.yellow;
        public float emissionIntensityOn = 1.5f;
        public float emissionIntensityOff = 0f;

        private Renderer _objectRenderer;
        private Material _objectMaterial;

        void Start()
        {
            _objectRenderer = GetComponent<Renderer>();
            if (_objectRenderer != null)
            {
                _objectMaterial = _objectRenderer.material;
            }
            else
            {
                Debug.LogWarning("Renderer component not found on the object!");
            }
        }

        public override void StateChangeEvent()
        {
            if (_objectMaterial != null)
            {
                if (this.State == (int)StateLevel.On)
                {
                    _objectMaterial.EnableKeyword("_EMISSION");
                    _objectMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensityOn);
                }
                else if (this.State == (int)StateLevel.Off)
                {
                    _objectMaterial.DisableKeyword("_EMISSION");
                    _objectMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensityOff);
                }
            }
        }
    }
}
