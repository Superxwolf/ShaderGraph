﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.Graphing;

namespace UnityEditor.Graphing.Drawing
{
    public abstract class AbstractGraphInspector : Editor
    {
        private readonly TypeMapper m_DataMapper = new TypeMapper(typeof(BasicNodeInspector));

        [SerializeField] protected List<AbstractNodeInspector> m_Inspectors = new List<AbstractNodeInspector>();

        protected IGraphAsset m_GraphAsset;

        protected abstract void AddTypeMappings(Action<Type, Type> map);

        public override void OnInspectorGUI()
        {
            UpdateInspectors();

            foreach (var inspector in m_Inspectors)
            {
                inspector.OnInspectorGUI();
            }
        }

        protected virtual void UpdateInspectors()
        {
            if (m_GraphAsset == null)
                return;

            var selectedNodes = m_GraphAsset.drawingData.selection.Select(m_GraphAsset.graph.GetNodeFromGuid).ToList();
            if (m_Inspectors.All(i => i.node != null) && selectedNodes.Select(n => n.guid).SequenceEqual(m_Inspectors.Select(i => i.nodeGuid)))
                return;

            m_Inspectors.Clear();
            foreach (var node in selectedNodes.OfType<SerializableNode>())
            {
                var inspector = CreateInspector(node);
                inspector.Initialize(node);
                m_Inspectors.Add(inspector);
            }
        }

        private AbstractNodeInspector CreateInspector(INode node)
        {
            var type = m_DataMapper.MapType(node.GetType());
            return CreateInstance(type) as AbstractNodeInspector;
        }

        public virtual void OnEnable()
        {
            m_GraphAsset = target as IGraphAsset;
            m_DataMapper.Clear();
            AddTypeMappings(m_DataMapper.AddMapping);
        }
    }
}