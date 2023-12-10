namespace NetworkEditor
{
    using Colossal.Entities;
    using Colossal.Json;
    using Colossal.Logging;
    using Game.Common;
    using Game.Input;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.Tools;
    using NetworkEditor.Code.Extensions;
    using NetworkEditor.Extensions;
    using NetworkEditor.Models;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.Rendering;
    using InputAction = UnityEngine.InputSystem.InputAction;
    using Node = Game.Net.Node;

    public sealed partial class NetworkEditorSystem : ToolBaseSystem
    {
        // Lookup and barrier.
        private ComponentLookup<Edge> _edgeDataLookup;
        private ComponentLookup<Node> _nodeDataLookup;
        private ComponentLookup<Composition> _compositionDataLookup;
        private ComponentLookup<NetCompositionData> _netCompositionDataLookup;
        private ComponentLookup<PrefabRef> _prefabRefDataLookup;
        private ComponentLookup<NetGeometryData> _netGeometryDataLookup;
        private ComponentLookup<NetTerrainData> _netTerrainDataLookup;
        private BufferLookup<NetGeometryComposition> _netGeometryCompositionBufferLookup;
        private BufferLookup<NetGeometrySection> _netGeometrySectionBufferLookup;
        private BufferLookup<NetSubSection> _netSubSectionBufferLookup;
        private BufferLookup<NetSectionPiece> _netSectionPiecesBufferLookup;
        private ToolOutputBarrier _toolOutputBarrier;

        // Raycast.
        private ControlPoint _raycastPoint;
        private Entity _selectedEdgeEntity = Entity.Null;

        // References.
        private ILog _log;
        private OverlayRenderSystem _overlayRenderSystem;
        private OverlayRenderSystem.Buffer _overlayBuffer;
        private ToolBaseSystem _previousActiveTool;

        // Input actions.
        private ProxyAction _applyAction;
        private ProxyAction _cancelAction;

        // Tool settings.
        private bool _isApplying;
        private bool _isDirty;

        /// <summary>
        /// Gets the tool's ID string.
        /// </summary>
        public override string toolID => "Network Editor";

        /// <summary>
        /// Gets a value indicating whether gets there is a currently selected <see cref="Edge"/> <see cref="Entity"/>.
        /// </summary>
        public bool HasSelectedEdgeEntity => _selectedEdgeEntity != Entity.Null;

        /// <summary>
        /// Gets the <see cref="EdgeDataUIModel"/> for the currently selected <see cref="Edge"/> <see cref="Entity"/>.
        /// </summary>
        internal EdgeDataUIModel SelectedEntityDataUIModel { get; private set; }

        /// <summary>
        /// Called when the raycast is initialized.
        /// </summary>
        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask = TypeMask.Net;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
            m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
        }

        /// <summary>
        /// Gets the prefab selected by this tool.
        /// </summary>
        /// <returns><c>null</c>.</returns>
        public override PrefabBase GetPrefab() => null;

        /// <summary>
        /// Sets the prefab selected by this tool.
        /// </summary>
        /// <param name="prefab">Prefab to set.</param>
        /// <returns><c>null</c>.</returns>
        public override bool TrySetPrefab(PrefabBase prefab)
        {
            return false;
        }

        /// <summary>
        /// Updates <see cref="_selectedEdgeEntity"/> with the new data received as <see cref="EdgeDataUIModel"/>.
        /// </summary>
        /// <param name="updatedEdgeData"></param>
        internal void UpdateSelectedEdge(EdgeDataUIModel updatedEdgeData)
        {
            SelectedEntityDataUIModel = updatedEdgeData;
            _isDirty = true;

            _log.Debug($"Updated Edge with {SelectedEntityDataUIModel.ToJSONString()}");
        }

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Set log.
            _log = Mod.Instance.Log;

            // Get system references.
            _overlayRenderSystem = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            _overlayBuffer = World.GetOrCreateSystemManaged<OverlayRenderSystem>().GetBuffer(out var _);

            // Get lookup and barrier references.
            _edgeDataLookup = GetComponentLookup<Edge>(false);
            _nodeDataLookup = GetComponentLookup<Node>(false);
            _compositionDataLookup = GetComponentLookup<Composition>(false);
            _netCompositionDataLookup = GetComponentLookup<NetCompositionData>(false);
            _prefabRefDataLookup = GetComponentLookup<PrefabRef>(false);
            _netGeometryDataLookup = GetComponentLookup<NetGeometryData>(false);
            _netGeometryCompositionBufferLookup = GetBufferLookup<NetGeometryComposition>(false);
            _netTerrainDataLookup = GetComponentLookup<NetTerrainData>(false);
            _netGeometrySectionBufferLookup = GetBufferLookup<NetGeometrySection>(false);
            _netSubSectionBufferLookup = GetBufferLookup<NetSubSection>(false);
            _netSectionPiecesBufferLookup = GetBufferLookup<NetSectionPiece>(false);
            _toolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();

            // Set actions.
            _applyAction = InputManager.instance.FindAction("Tool", "Apply");
            _cancelAction = InputManager.instance.FindAction("Tool", "Mouse Cancel");

            // Toggle tool action.
            InputAction hotKey = new("NetworkEditor-Hotkey");
            hotKey.AddCompositeBinding("ButtonWithOneModifier").With("Modifier", "<Keyboard>/ctrl").With("Button", "<Keyboard>/e");
            hotKey.performed += EnableTool;
            hotKey.Enable();
        }

        /// <summary>
        /// Method copied from <see cref="CompositionSelectSystem.CreateCompositionJob.CreateComposition"/>.
        /// Adapted to use <see cref="EntityManager"/> rather than <see cref="EntityCommandBuffer"/>.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        private Entity CreateComposition(Entity prefab, CompositionFlags mask)
        {
            var prefabGeometryData = _netGeometryDataLookup[prefab];
            var prefabGeometrySections = _netGeometrySectionBufferLookup[prefab];
            Entity entity;
            if ((mask.m_General & CompositionFlags.General.Node) != (CompositionFlags.General)0U)
            {
                entity = EntityManager.CreateEntity(prefabGeometryData.m_NodeCompositionArchetype);
            }
            else
            {
                entity = EntityManager.CreateEntity(prefabGeometryData.m_EdgeCompositionArchetype);
            }

            if (EntityManager.TryGetBuffer<NetGeometryComposition>(entity, false, out var buffer))
            {
                buffer.Add(new NetGeometryComposition
                {
                    m_Composition = entity,
                    m_Mask = mask,
                });
            }

            EntityManager.SetComponentData(entity, new PrefabRef(prefab));
            EntityManager.SetComponentData(entity, new NetCompositionData
            {
                m_Flags = mask,
            });
            var netCompositionPiecesBuffer = EntityManager.AddBuffer<NetCompositionPiece>(entity);
            var netCompositionPiecesList = new NativeList<NetCompositionPiece>(32, Allocator.Temp);
            NetCompositionHelpers.GetCompositionPieces(netCompositionPiecesList, prefabGeometrySections.AsNativeArray(), mask, _netSubSectionBufferLookup, _netSectionPiecesBufferLookup);
            netCompositionPiecesBuffer.CopyFrom(netCompositionPiecesList.AsArray());
            for (int i = 0; i < netCompositionPiecesList.Length; i++)
            {
                NetCompositionPiece netCompositionPiece = netCompositionPiecesList[i];
                if (_netTerrainDataLookup.HasComponent(netCompositionPiece.m_Piece))
                {
                    EntityManager.AddComponentData(entity, default(TerrainComposition));
                    break;
                }
            }

            netCompositionPiecesList.Dispose();
            return entity;
        }

        /// <summary>
        /// Method copied from <see cref="CompositionSelectSystem.CreateCompositionJob.FindComposition"/>.
        /// </summary>
        /// <param name="geometryCompositions"></param>
        /// <param name="flags"></param>
        /// <returns><see cref="Entity.Null"/> if no <see cref="Composition"/> are matching.</returns>
        private Entity FindComposition(DynamicBuffer<NetGeometryComposition> geometryCompositions, CompositionFlags flags)
        {
            for (int i = 0; i < geometryCompositions.Length; i++)
            {
                NetGeometryComposition netGeometryComposition = geometryCompositions[i];
                if (netGeometryComposition.m_Mask == flags)
                {
                    return netGeometryComposition.m_Composition;
                }
            }

            return Entity.Null;
        }

        /// <summary>
        /// Called every tool update.
        /// </summary>
        /// <param name="inputDeps">Input dependencies.</param>
        /// <returns>Job handle.</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // Checks for cancel action and resets tool state.
            if (_cancelAction.WasPressedThisFrame())
            {
                SetHighlight(_raycastPoint, false);
                _raycastPoint = default;
                _selectedEdgeEntity = Entity.Null;
                SelectedEntityDataUIModel = new EdgeDataUIModel();
                _isApplying = false;

                return inputDeps;
            }

            // Checks if there is dirty data to update.
            if (_isDirty)
            {
                // Checks if the selected object has all the needed components.
                if (_edgeDataLookup.TryGetComponent(SelectedEntityDataUIModel.Edge, out var edge) &&
                    _prefabRefDataLookup.TryGetComponent(SelectedEntityDataUIModel.Edge, out var prefabRef) &&
                    _netGeometryCompositionBufferLookup.TryGetBuffer(prefabRef.m_Prefab, out var netGeometryCompositionBuffer) &&
                    _compositionDataLookup.TryGetComponent(SelectedEntityDataUIModel.Edge, out var composition) &&
                    _netCompositionDataLookup.TryGetComponent(composition.m_Edge, out var edgeComposition) &&
                    _netCompositionDataLookup.TryGetComponent(composition.m_StartNode, out var startNodeComposition) &&
                    _netCompositionDataLookup.TryGetComponent(composition.m_EndNode, out var endNodeComposition))
                {
                    // Creates the CommandBuffer that will execute updates.
                    var commandBuffer = _toolOutputBarrier.CreateCommandBuffer();

                    // Finds a Composition that matches the up to date flags.
                    var updatedComposition = new Composition
                    {
                        m_Edge = FindComposition(netGeometryCompositionBuffer, new CompositionFlags
                        {
                            m_General = SelectedEntityDataUIModel.General,
                            m_Left = SelectedEntityDataUIModel.Left,
                            m_Right = SelectedEntityDataUIModel.Right,
                        }),
                        m_StartNode = FindComposition(netGeometryCompositionBuffer, startNodeComposition.m_Flags),
                        m_EndNode = FindComposition(netGeometryCompositionBuffer, endNodeComposition.m_Flags),
                    };

                    // Checks for m_Edge being null on the updated Composition.
                    // In that case, creates the new Composition for the Edge.
                    if (updatedComposition.m_Edge == Entity.Null)
                    {
                        updatedComposition.m_Edge = CreateComposition(prefabRef, new CompositionFlags
                        {
                            m_General = SelectedEntityDataUIModel.General,
                            m_Left = SelectedEntityDataUIModel.Left,
                            m_Right = SelectedEntityDataUIModel.Right,
                        });
                    }

                    // Sets the new Composition for the current Edge.
                    commandBuffer.SetComponent<Composition>(SelectedEntityDataUIModel.Edge, updatedComposition);

                    _isDirty = false;
                }
            }

            // Checks if we're currently applying.
            if (_isApplying)
            {
                SetHighlight(_raycastPoint, true);

                // TODO: this prevents from moving to another edge while one is selected
                return inputDeps;
            }

            // Handles raycast results.
            if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
            {
                // Checks if we're still on the same Edge as before.
                if (!_raycastPoint.Equals(controlPoint))
                {
                    // Turns highlight off for the previous Edge and stores the current one.
                    SetHighlight(_raycastPoint, false);
                    _raycastPoint = controlPoint;
                }

                // Highlights the current Edge.
                SetHighlight(_raycastPoint, true);

                // Checks for apply action and updates tool state.
                if (_applyAction.WasPressedThisFrame())
                {
                    _isApplying = true;
                    _selectedEdgeEntity = controlPoint.m_OriginalEntity;
                    SelectedEntityDataUIModel = EdgeDataUIModel.FromEntity(
                        _selectedEdgeEntity,
                        _edgeDataLookup,
                        _compositionDataLookup,
                        _netCompositionDataLookup);

                    _log.Info($"Selected edge: {_selectedEdgeEntity.ToJSONString()}");
                    _log.Info($"Selected model: {SelectedEntityDataUIModel.ToJSONString()}");
                }
            }
            else
            {
                // Resets tool status as Raycast didn't hit anything.
                SetHighlight(_raycastPoint, false);
                _raycastPoint = default;
                _selectedEdgeEntity = Entity.Null;
                SelectedEntityDataUIModel = new EdgeDataUIModel();
                _isApplying = false;
            }

            return inputDeps;
        }

        /// <summary>
        /// Called when the tool starts running.
        /// </summary>
        protected override void OnStartRunning()
        {
            _log.Debug("OnStartRunning");
            base.OnStartRunning();

            // Ensure apply action is enabled.
            _applyAction.shouldBeEnabled = true;
            _cancelAction.shouldBeEnabled = true;

            // Clear any previous raycast result.
            _raycastPoint = default;

            // Clear any applications.
            applyMode = ApplyMode.Clear;
        }

        /// <summary>
        /// Called when the tool stops running.
        /// </summary>
        protected override void OnStopRunning()
        {
            _log.Debug("OnStopRunning");

            // Disable apply action.
            _applyAction.shouldBeEnabled = false;
            _cancelAction.shouldBeEnabled = false;

            // Cancel cursor entity.
            if (_selectedEdgeEntity != Entity.Null)
            {
                EntityManager.RemoveComponent<Highlighted>(_selectedEdgeEntity);
                EntityManager.AddComponent<BatchesUpdated>(_selectedEdgeEntity);
                _selectedEdgeEntity = Entity.Null;
            }

            // Reset any applying action.
            _isApplying = false;

            base.OnStopRunning();
        }

        /// <summary>
        /// Enables the tool (called by hotkey action).
        /// </summary>
        /// <param name="context">Callback context.</param>
        private void EnableTool(InputAction.CallbackContext context)
        {
            // Activate this tool if it isn't already active.
            if (m_ToolSystem.activeTool != this)
            {
                // Valid prefab selected - switch to this tool.
                m_ToolSystem.selected = Entity.Null;
                m_ToolSystem.activeTool = this;
            }
        }

        /// <summary>
        /// Sets or unsets the <see cref="Highlighted"/> component for the provided <see cref="ControlPoint.m_OriginalEntity"/>.
        /// </summary>
        /// <param name="controlPoint"></param>
        /// <param name="highlight"></param>
        private void SetHighlight(ControlPoint controlPoint, bool highlight)
        {
            if (highlight)
            {
                EntityManager.AddComponent<Highlighted>(controlPoint.m_OriginalEntity);
                EntityManager.AddComponent<BatchesUpdated>(controlPoint.m_OriginalEntity);

                if (_edgeDataLookup.TryGetComponent(controlPoint.m_OriginalEntity, out var edge) &&
                    _nodeDataLookup.TryGetComponent(edge.m_Start, out var startNode) &&
                    _nodeDataLookup.TryGetComponent(edge.m_End, out var endNode))
                {
                    _overlayBuffer.DrawCircle(Color.blue, Color.blue.WithTransparency(0.5f), 1f, 0, new float2(0f, 1f), startNode.m_Position, 20f);
                    _overlayBuffer.DrawCircle(Color.red, Color.red.WithTransparency(0.5f), 1f, 0, new float2(0f, 1f), endNode.m_Position, 20f);
                }
            }
            else
            {
                EntityManager.RemoveComponent<Highlighted>(controlPoint.m_OriginalEntity);
                EntityManager.AddComponent<BatchesUpdated>(controlPoint.m_OriginalEntity);
            }
        }
    }
}
