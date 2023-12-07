namespace NetworkEditor.Models
{
    using System;
    using Colossal.UI.Binding;
    using Game.Net;
    using Game.Prefabs;
    using Unity.Entities;

    internal class EdgeDataUIModel : IJsonWritable, IJsonReadable
    {
        public static EdgeDataUIModel FromEntity(
            Entity entity,
            ComponentLookup<Edge> edgeDataLookup,
            ComponentLookup<Composition> compositionDataLookup,
            ComponentLookup<NetCompositionData> netCompositionDataLookup)
        {
            if (entity == Entity.Null)
            {
                return new EdgeDataUIModel();
            }

            if (edgeDataLookup.TryGetComponent(entity, out var edge) &&
                compositionDataLookup.TryGetComponent(entity, out var composition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_Edge, out var edgeComposition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_StartNode, out var startNodeComposition) &&
                netCompositionDataLookup.TryGetComponent(composition.m_EndNode, out var endNodeComposition))
            {
                return new EdgeDataUIModel
                {
                    Edge = entity,
                    General = edgeComposition.m_Flags.m_General,
                    Left = edgeComposition.m_Flags.m_Left,
                    Right = edgeComposition.m_Flags.m_Right,
                };
            }

            return new EdgeDataUIModel();
        }

        public void Write(IJsonWriter writer)
        {
            writer.TypeBegin(this.GetType().FullName);

            writer.PropertyName(nameof(this.Edge));
            writer.Write(this.Edge);

            writer.PropertyName(nameof(this.General));
            var generalEnumValues = Enum.GetValues(typeof(CompositionFlags.General));
            writer.MapBegin(generalEnumValues.Length);
            foreach (CompositionFlags.General flag in generalEnumValues)
            {
                writer.Write(flag.ToString());
                writer.Write(this.General.HasFlag(flag));
            }

            writer.MapEnd();

            writer.PropertyName(nameof(this.Left));
            var leftEnumValues = Enum.GetValues(typeof(CompositionFlags.Side));
            writer.MapBegin(leftEnumValues.Length);
            foreach (CompositionFlags.Side flag in leftEnumValues)
            {
                writer.Write(flag.ToString());
                writer.Write(this.Left.HasFlag(flag));
            }

            writer.MapEnd();

            writer.PropertyName(nameof(this.Right));
            var rightEnumValues = Enum.GetValues(typeof(CompositionFlags.Side));
            writer.MapBegin(rightEnumValues.Length);
            foreach (CompositionFlags.Side flag in rightEnumValues)
            {
                writer.Write(flag.ToString());
                writer.Write(this.Right.HasFlag(flag));
            }

            writer.MapEnd();

            writer.TypeEnd();
        }

        public void Read(IJsonReader reader)
        {
            if (reader.ReadProperty(nameof(Edge)))
            {
                reader.Read(out Entity readEdge);
                Edge = readEdge;
            }

            if (reader.ReadProperty(nameof(General)))
            {
                var generalEnumValues = Enum.GetValues(typeof(CompositionFlags.General));
                reader.ReadMapBegin();
                foreach (CompositionFlags.General flag in generalEnumValues)
                {
                    reader.ReadMapKeyValue();
                    reader.Read(out string readFlagName);
                    reader.Read(out bool readFlagValue);

                    var targetFlagName = (CompositionFlags.General)Enum.Parse(typeof(CompositionFlags.General), readFlagName, true);
                    if (readFlagValue)
                    {
                        General |= targetFlagName;
                    }
                    else
                    {
                        General &= ~targetFlagName;
                    }
                }

                reader.ReadMapEnd();
            }

            if (reader.ReadProperty(nameof(Left)))
            {
                var leftEnumValues = Enum.GetValues(typeof(CompositionFlags.Side));
                reader.ReadMapBegin();
                foreach (CompositionFlags.Side flag in leftEnumValues)
                {
                    reader.ReadMapKeyValue();
                    reader.Read(out string readFlagName);
                    reader.Read(out bool readFlagValue);

                    var targetFlagName = (CompositionFlags.Side)Enum.Parse(typeof(CompositionFlags.Side), readFlagName, true);
                    if (readFlagValue)
                    {
                        Left |= targetFlagName;
                    }
                    else
                    {
                        Left &= ~targetFlagName;
                    }
                }

                reader.ReadMapEnd();
            }

            if (reader.ReadProperty(nameof(Right)))
            {
                var rightEnumValues = Enum.GetValues(typeof(CompositionFlags.Side));
                reader.ReadMapBegin();
                foreach (CompositionFlags.Side flag in rightEnumValues)
                {
                    reader.ReadMapKeyValue();
                    reader.Read(out string readFlagName);
                    reader.Read(out bool readFlagValue);

                    var targetFlagName = (CompositionFlags.Side)Enum.Parse(typeof(CompositionFlags.Side), readFlagName, true);
                    if (readFlagValue)
                    {
                        Right |= targetFlagName;
                    }
                    else
                    {
                        Right &= ~targetFlagName;
                    }
                }

                reader.ReadMapEnd();
            }
        }

        public Entity Edge { get; set; }

        public CompositionFlags.General General { get; set; }

        public CompositionFlags.Side Left { get; set; }

        public CompositionFlags.Side Right { get; set; }
    }
}
