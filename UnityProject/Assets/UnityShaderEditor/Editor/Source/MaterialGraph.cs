using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.MaterialGraph
{
    public class MaterialGraph : ScriptableObject, IGenerateGraphProperties
    {
        [SerializeField]
        private MaterialProperties m_MaterialProperties;

        [SerializeField]
        private MaterialOptions m_MaterialOptions;

        [SerializeField]
        private PixelGraph m_PixelGraph;

        [SerializeField]
        private bool m_Expanded;
        
        public int GetShaderInstanceID()
        {
            return -1;
            //return m_Shader.GetInstanceID();
        }

        public MaterialProperties materialProperties { get { return m_MaterialProperties; } }
        public MaterialOptions materialOptions { get { return m_MaterialOptions; } }

        public BaseMaterialGraph currentGraph { get { return m_PixelGraph; } }

        public void GenerateSharedProperties(PropertyGenerator shaderProperties, ShaderGenerator propertyUsages, GenerationMode generationMode)
        {
            m_MaterialProperties.GenerateSharedProperties(shaderProperties, propertyUsages, generationMode);
        }

        public IEnumerable<ShaderProperty> GetPropertiesForPropertyType(PropertyType propertyType)
        {
            return m_MaterialProperties.GetPropertiesForPropertyType(propertyType);
        }

        public void OnEnable()
        {
            if (m_MaterialProperties == null)
            {
                m_MaterialProperties = CreateInstance<MaterialProperties>();
                m_MaterialProperties.hideFlags = HideFlags.HideInHierarchy;
            }

            if (m_MaterialOptions == null)
            {
                m_MaterialOptions = CreateInstance<MaterialOptions>();
                m_MaterialOptions.Init();
                m_MaterialOptions.hideFlags = HideFlags.HideInHierarchy;
            }

            if (m_PixelGraph == null)
            {
                m_PixelGraph = CreateInstance<PixelGraph>();
                m_PixelGraph.hideFlags = HideFlags.HideInHierarchy;
                m_PixelGraph.name = name;
            }

            m_PixelGraph.owner = this;
        }

        public void OnDisable()
        {
            //      if (m_MaterialProperties != null)
            //      m_MaterialProperties.OnChangePreviewState -= OnChangePreviewState;
        }

        void OnChangePreviewState(object sender, EventArgs eventArgs)
        {
            m_PixelGraph.previewState = (PreviewState)sender;
        }

        public void CreateSubAssets()
        {
            AssetDatabase.AddObjectToAsset(m_MaterialProperties, this);
            AssetDatabase.AddObjectToAsset(m_MaterialOptions, this);
            AssetDatabase.AddObjectToAsset(m_PixelGraph, this);
        }


        private Material m_Material;
        public Material GetMaterial()
        {
            if (m_PixelGraph == null)
                return null;
            
            return m_PixelGraph.GetMaterial();
        }
    }
}