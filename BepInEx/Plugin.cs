namespace NetworkEditor
{
    using System.Reflection;
    using BepInEx;
    using Game;
    using Game.Common;
    using Game.SceneFlow;
    using HarmonyLib;

#if BEPINEX_V6
    using BepInEx.Unity.Mono;
#endif

    /// <summary>
    /// BepInEx plugin to substitute for IMod support.
    /// </summary>
    [BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// Plugin unique GUID.
        /// </summary>
        public const string GUID = "com.github.ST-Apps.CS2.NetworkEditor";

        // IMod instance reference.
        private Mod _mod;

        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        public void Awake()
        {
            _mod = new();
            _mod.OnLoad();

            _mod.Log.Info("Plugin.Awake");

            // Apply Harmony patches.
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }

        /// <summary>
        /// Harmony postfix to <see cref="SystemOrder.Initialize"/> to substitute for IMod.OnCreateWorld.
        /// </summary>
        /// <param name="updateSystem"><see cref="GameManager"/> <see cref="UpdateSystem"/> instance.</param>
        [HarmonyPatch(typeof(SystemOrder), nameof(SystemOrder.Initialize))]
        [HarmonyPostfix]
        private static void InjectSystems(UpdateSystem updateSystem) => Mod.Instance.OnCreateWorld(updateSystem);
    }
}
