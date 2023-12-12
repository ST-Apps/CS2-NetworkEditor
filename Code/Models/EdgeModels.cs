namespace NetworkEditor.Models
{
    using cohtml.Net;
    using Colossal.UI.Binding;
    using Game.Net;
    using Game.Prefabs;
    using NetworkEditor.Extensions;
    using Unity.Entities;

    [CoherentType]
    internal struct UINetItem : IJsonWritable, IJsonReadable
    {
        [CoherentProperty("edge")]
        public UIEdge Edge;

        [CoherentProperty("startNode")]
        public UINode StartNode;

        [CoherentProperty("endNode")]
        public UINode EndNode;

        public static UINetItem FromEntity(
            Entity entity,
            ComponentLookup<Edge> edgeDataLookup,
            ComponentLookup<Composition> compositionDataLookup,
            ComponentLookup<NetCompositionData> netCompositionDataLookup)
        {
            if (entity == Entity.Null)
            {
                return default;
            }

            if (edgeDataLookup.TryGetComponent(entity, out var edge) &&
                compositionDataLookup.TryGetComponent(entity, out var composition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_Edge, out var edgeComposition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_StartNode, out var startNodeComposition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_EndNode, out var endNodeComposition))
            {
                return new UINetItem
                {
                    Edge = new UIEdge
                    {
                        Entity = entity,
                        CompositionFlags = new UICompositionFlags
                        {
                            General = edgeComposition.m_Flags.m_General,
                            Left = edgeComposition.m_Flags.m_Left,
                            Right = edgeComposition.m_Flags.m_Right,
                        },
                        Width = edgeComposition.m_Width,
                        MiddleOffset = edgeComposition.m_MiddleOffset,
                        WidthOffset = edgeComposition.m_WidthOffset,
                        NodeOffset = edgeComposition.m_NodeOffset,
                    },
                    StartNode = new UINode
                    {
                        Entity = edge.m_Start,
                        CompositionFlags = new UICompositionFlags
                        {
                            General = startNodeComposition.m_Flags.m_General,
                            Left = startNodeComposition.m_Flags.m_Left,
                            Right = startNodeComposition.m_Flags.m_Right,
                        },
                    },
                    EndNode = new UINode
                    {
                        Entity = edge.m_End,
                        CompositionFlags = new UICompositionFlags
                        {
                            General = endNodeComposition.m_Flags.m_General,
                            Left = endNodeComposition.m_Flags.m_Left,
                            Right = endNodeComposition.m_Flags.m_Right,
                        },
                    },
                };
            }

            return default;
        }

        public void Read(IJsonReader reader)
        {
            reader.ReadMapBegin();

            reader.ReadProperty("edge");
            Edge = default;
            Edge.Read(reader);

            reader.ReadProperty("startNode");
            StartNode = default;
            StartNode.Read(reader);

            reader.ReadProperty("endNode");
            EndNode = default;
            EndNode.Read(reader);

            reader.ReadMapEnd();
        }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName("edge");
            writer.Write<UIEdge>(Edge);

            writer.PropertyName("startNode");
            writer.Write<UINode>(StartNode);

            writer.PropertyName("endNode");
            writer.Write<UINode>(EndNode);

            writer.TypeEnd();
        }
    }

    [CoherentType]
    internal struct UIEdge : IJsonWritable, IJsonReadable
    {
        [CoherentProperty("entity")]
        public Entity Entity;

        [CoherentProperty("flags")]
        public UICompositionFlags CompositionFlags;

        [CoherentProperty("width")]
        public float Width;

        [CoherentProperty("middleOffset")]
        public float MiddleOffset;

        [CoherentProperty("widthOffset")]
        public float WidthOffset;

        [CoherentProperty("nodeOffset")]
        public float NodeOffset;

        public void Read(IJsonReader reader)
        {
            reader.ReadMapBegin();

            reader.ReadProperty("entity");
            reader.Read(out this.Entity);

            reader.ReadProperty("flags");
            CompositionFlags = default;
            CompositionFlags.Read(reader);

            reader.ReadProperty("width");
            reader.Read(out this.Width);

            reader.ReadProperty("middleOffset");
            reader.Read(out this.MiddleOffset);

            reader.ReadProperty("widthOffset");
            reader.Read(out this.WidthOffset);

            reader.ReadProperty("nodeOffset");
            reader.Read(out this.NodeOffset);

            reader.ReadMapEnd();
        }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName("entity");
            writer.Write(Entity);

            writer.PropertyName("flags");
            writer.Write(CompositionFlags);

            writer.PropertyName("width");
            writer.Write(Width);

            writer.PropertyName("middleOffset");
            writer.Write(MiddleOffset);

            writer.PropertyName("widthOffset");
            writer.Write(WidthOffset);

            writer.PropertyName("nodeOffset");
            writer.Write(NodeOffset);

            writer.TypeEnd();
        }
    }

    [CoherentType]
    internal struct UINode : IJsonWritable, IJsonReadable
    {
        [CoherentProperty("entity")]
        public Entity Entity;

        [CoherentProperty("flags")]
        public UICompositionFlags CompositionFlags;

        public void Read(IJsonReader reader)
        {
            reader.ReadMapBegin();

            reader.ReadProperty("entity");
            reader.Read(out this.Entity);

            reader.ReadProperty("flags");
            CompositionFlags = default;
            CompositionFlags.Read(reader);

            reader.ReadMapEnd();
        }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName("entity");
            writer.Write(Entity);

            writer.PropertyName("flags");
            writer.Write(CompositionFlags);

            writer.TypeEnd();
        }
    }

    [CoherentType]
    internal struct UICompositionFlags : IJsonWritable, IJsonReadable
    {
        [CoherentProperty("general")]
        public CompositionFlags.General General;

        [CoherentProperty("left")]
        public CompositionFlags.Side Left;

        [CoherentProperty("right")]
        public CompositionFlags.Side Right;

        public void Read(IJsonReader reader)
        {
            reader.ReadMapBegin();

            reader.ReadProperty("general");
            new DictionaryReader<string, bool>().Read(reader, out var generalDict);
            General = generalDict.ToFlags<CompositionFlags.General>();

            reader.ReadProperty("left");
            new DictionaryReader<string, bool>().Read(reader, out var leftDict);
            Left = leftDict.ToFlags<CompositionFlags.Side>();

            reader.ReadProperty("right");
            new DictionaryReader<string, bool>().Read(reader, out var rightlDict);
            Right = rightlDict.ToFlags<CompositionFlags.Side>();

            reader.ReadMapEnd();
        }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(GetType().FullName);

            writer.PropertyName("general");
            new DictionaryWriter<string, bool>().Write(writer, General.ToDictionary());

            writer.PropertyName("left");
            new DictionaryWriter<string, bool>().Write(writer, Left.ToDictionary());

            writer.PropertyName("right");
            new DictionaryWriter<string, bool>().Write(writer, Right.ToDictionary());

            writer.TypeEnd();
        }
    }
}
