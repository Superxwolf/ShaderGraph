using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public class Texture2DInputMaterialSlot : Texture2DMaterialSlot
    {
        [SerializeField]
        private SerializableTexture m_Texture = new SerializableTexture();

        public Texture texture
        {
            get { return m_Texture.texture; }
            set { m_Texture.texture = value; }
        }

        public Texture2DInputMaterialSlot()
        {}

        public Texture2DInputMaterialSlot(
            int slotId,
            string displayName,
            string shaderOutputName,
            ShaderStage shaderStage = ShaderStage.Dynamic,
            bool hidden = false)
            : base(slotId, displayName, shaderOutputName, SlotType.Input, shaderStage, hidden)
        {}

        public override VisualElement InstantiateControl()
        {
            return new TextureSlotControlView(this);
        }

        public override string GetDefaultValue(GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            return matOwner.GetVariableNameForSlot(id);
        }

        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            var prop = new TextureShaderProperty();
            prop.overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            prop.modifiable = false;
            prop.generatePropertyBlock = true;
            prop.value.texture = texture;
            properties.AddShaderProperty(prop);
        }

        public override PreviewProperty GetPreviewProperty(string name)
        {
            var pp = new PreviewProperty(PropertyType.Texture)
            {
                name = name,
                textureValue = texture
            };
            return pp;
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var slot = foundSlot as Texture2DInputMaterialSlot;
            if (slot != null)
                m_Texture = slot.m_Texture;
        }
    }
}
