using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;
using OvergrownUrban.CreatureLight;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace OvergrownUrban;

[BepInPlugin("PaleForkVali", "Overgrown Urban", "0.1.0")]
[BepInDependency("lb-fgf-m4r-ik.modpack", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("io.github.dual.fisobs", BepInDependency.DependencyFlags.HardDependency)]

sealed class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger;

    public const bool devMode = false;

    bool isInit;

    public void OnDisable()
    {
        Enums.Unregister();
    }

    public void OnEnable()
    {
        Logger = base.Logger;

        //Fisobs in OnModsInit calls InitiateResources or something
        //so that makes it sometimes miss method depending on load order
        //the intended path is to register fisobs before onmodsinit

        SteamLizard.Meta.Apply();
        On.RainWorld.OnModsInit += OnModsInit;
        Enums.Register();

        Devtools.Hook();
    }

    void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (isInit) return;
        isInit = true;
        CutsceneChange.Init();
    }
}
