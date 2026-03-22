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
    bool isEnablePassed = false;
    public const bool devMode = true;
    public const byte ticksPerSecond = 40;

    public void OnEnable()
    {
        if (!isEnablePassed)
        {
            Logger = base.Logger;
            //Fisobs in OnModsInit calls InitiateResources or something
            //so that makes it sometimes miss method depending on load order
            //the intended path is to register fisobs before onmodsinit
            SteamLizard.Meta.Apply();
            On.RainWorld.OnModsInit += OnModsInit;
            isEnablePassed = true;
        }
    }

    void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (isInit) return;
        isInit = true;
        CutsceneChange.Init();
    }
}
