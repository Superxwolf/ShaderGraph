using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.MaterialGraph
{
    public abstract class PropertyNode : BaseMaterialNode
    {
        [SerializeField]
        private ShaderProperty m_BoundProperty;

        public bool exposed
        {
            get { return m_BoundProperty != null; }
        }

        protected virtual bool HasBoundProperty() { return m_BoundProperty != null; }
        public ShaderProperty boundProperty { get { return m_BoundProperty; } }

        public virtual void BindProperty(ShaderProperty property, bool rebuildShaders)
        {
            m_BoundProperty = property;
        }

        public virtual void RefreshBoundProperty(ShaderProperty toRefresh, bool rebuildShader)
        {
            if (m_BoundProperty != null && m_BoundProperty == toRefresh)
            {
                BindProperty(toRefresh, rebuildShader);
            }
        }

        public IEnumerable<ShaderProperty> FindValidPropertyBindings()
        {
            if (graph is IGenerateGraphProperties)
                return (graph as IGenerateGraphProperties).GetPropertiesForPropertyType(propertyType);

            return new ShaderProperty[0];
        }

        public virtual string GetPropertyName()
        {
            if (m_BoundProperty == null)
                return name + "_" + Math.Abs(GetInstanceID()) + "_Uniform";

            return m_BoundProperty.name;
        }

        public abstract PropertyType propertyType { get; }

        public abstract PreviewProperty GetPreviewProperty();

        protected override void CollectPreviewMaterialProperties (List<PreviewProperty> properties)
        {
            base.CollectPreviewMaterialProperties(properties);
            properties.Add(GetPreviewProperty());
        }

        public void UnbindProperty(ShaderProperty prop)
        {
            if (m_BoundProperty != null && m_BoundProperty == prop)
            {
                m_BoundProperty = null;
                RegeneratePreviewShaders();
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();
            
            // find available properties
            var allowedBindings = FindValidPropertyBindings().ToList();

            var names = new List<string> { "none" };
            names.AddRange(allowedBindings.Select(x => x.name));
            var currentIndex = names.IndexOf(boundProperty == null ? "none" : boundProperty.name);

            EditorGUI.BeginChangeCheck();
            currentIndex = EditorGUILayout.Popup("Bound Property", currentIndex, names.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                ShaderProperty selected = null;
                if (currentIndex > 0)
                    selected = allowedBindings[currentIndex - 1];

                BindProperty(selected, true);
            }
        }
    }
}