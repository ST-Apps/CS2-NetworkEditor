namespace NetworkEditor
{
    using System;
    using cohtml.Net;
    using Colossal.Json;
    using Colossal.Logging;
    using Colossal.UI.Binding;
    using Game.SceneFlow;
    using Game.Tools;
    using Game.UI;
    using NetworkEditor.Models;
    using NetworkEditor.Utils;

    /// <summary>
    /// A tool UI system for NetworkEditor.
    /// </summary>
    public sealed partial class NetworkEditorUISystem : UISystemBase
    {
        // Injection references.
        private static readonly string _jsResourceId = "NetworkEditor.Resources.ui.js";
        private static readonly string _eventsNamespaceKey = "NetworkEditor";
        private static readonly string _eventEdgeUpdatedName = "EdgeUpdated";
        private static readonly string _eventApplyChangesName = "ApplyChanges";
        private static readonly string _currentEdgeName = "CurrentEdge";
        private static readonly string _isConfigurationValidName = "ConfigurationValid";

        // Cached references.
        private View _uiView;
        private ToolSystem _toolSystem;
        private NetworkEditorSystem _networkEditorSystem;
        private ILog _log;

        // Internal status.
        private bool _toolIsActive = false;

        // Injection resources.
        private string _jsResource;

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Set log.
            _log = Mod.Instance.Log;

            // Set references.
            _uiView = GameManager.instance.userInterface.view.View;
            _toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            _networkEditorSystem = World.GetOrCreateSystemManaged<NetworkEditorSystem>();

            // Read injection data.
            _jsResource = UIFileUtils.ReadJS(_jsResourceId);

            // Register event handlers.
            AddBinding(new TriggerBinding<UINetItem>(
                _eventsNamespaceKey,
                _eventEdgeUpdatedName,
                new Action<UINetItem>(OnEdgeUpdated)));
            AddBinding(new TriggerBinding(
                _eventsNamespaceKey,
                _eventApplyChangesName,
                new Action(ApplyChanges)));

            AddUpdateBinding(
                new GetterValueBinding<UINetItem>(
                    _eventsNamespaceKey,
                    _currentEdgeName,
                    () => _networkEditorSystem.SelectedEdgeUIModel));
            AddUpdateBinding(
                new GetterValueBinding<bool>(
                    _eventsNamespaceKey,
                    _isConfigurationValidName,
                    () => _networkEditorSystem.IsConfigurationValid));
        }

        /// <summary>
        /// Called every UI update.
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();

            // Check for line tool activation.
            if (_toolSystem.activeTool == _networkEditorSystem &&
                _networkEditorSystem.HasSelectedEdgeEntity)
            {
                if (!_toolIsActive)
                {
                    // Attach our custom controls.
                    // Inject scripts.
                    _log.Debug("Adding panel");
                    UIFileUtils.ExecuteScript(_uiView, _jsResource);

                    // Record current tool state.
                    _toolIsActive = true;
                }
            }
            else
            {
                // Line tool not active - clean up if this is the first update after deactivation.
                if (_toolIsActive)
                {
                    _log.Debug("Removing panel");

                    // Remove DOM activation.
                    // TODO: add a method in the react component that shows/hides the panel, we don't need to create and destroy this everytime
                    UIFileUtils.ExecuteScript(_uiView, "var panel = document.getElementById(\"network-editor-root\"); if (panel) panel.parentElement.removeChild(panel);");

                    // Record current tool state.
                    _toolIsActive = false;
                }
            }
        }

        private void OnEdgeUpdated(UINetItem updatedEdgeData)
        {
            _log.Info($"Current Edge updated with new data: {updatedEdgeData.ToJSONString()}");

            _networkEditorSystem.UpdateSelectedEdge(updatedEdgeData);
        }

        private void ApplyChanges()
        {
            _log.Info($"Applying changes to current Edge.");

            _networkEditorSystem.ApplyChanges();
        }
    }
}
