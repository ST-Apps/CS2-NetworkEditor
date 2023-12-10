using Game.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEditor.Extensions
{
    internal static class NetCompositionDataExtensions
    {

        public static NetCompositionData Clone(this NetCompositionData data)
        {
            return new NetCompositionData
            {
                m_EdgeHeights = data.m_EdgeHeights,
                m_Flags = data.m_Flags,
                m_HeightRange = data.m_HeightRange,
                m_MiddleOffset = data.m_MiddleOffset,
                m_MinLod = data.m_MinLod,
                m_NodeOffset = data.m_NodeOffset,
                m_RoundaboutSize = data.m_RoundaboutSize,
                m_State = data.m_State,
                m_SurfaceHeight = data.m_SurfaceHeight,
                m_SyncVertexOffsetsLeft = data.m_SyncVertexOffsetsLeft,
                m_SyncVertexOffsetsRight = data.m_SyncVertexOffsetsRight,
                m_Width = data.m_Width,
                m_WidthOffset = data.m_WidthOffset,
            };
        }

        public static NetCompositionData WithFlags(this NetCompositionData data, CompositionFlags flags)
        {
            data.m_Flags = flags;
            return data;
        }

    }
}
