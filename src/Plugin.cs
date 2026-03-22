using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace OvergrownUrban;

[BepInPlugin("PaleForkVali", "Overgrown Urban", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger;
    bool isInit;
    public const bool devMode = true;
    public const byte ticksPerSecond = 40;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
    }

    void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (isInit) return;
        isInit = true;
        CutsceneChange.Init();
        SteamLizard.Meta.Apply();
        Futile.atlasManager.LoadAtlas(Path.Combine(ModManager.ActiveMods.First(x => x.id == "vali_overgrownurban").path,
            "atlases", "LizardEyes77"));
    }
}
