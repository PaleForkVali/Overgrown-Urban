using JetBrains.Annotations;
using Menu;

namespace TestMod;

public static class CutsceneChange
{
	public static void Init()
	{
		On.World.ctor += WorldOnctor;
		On.Menu.MenuScene.BuildScene += MenuSceneOnBuildScene;
	}

	static void MenuSceneOnBuildScene(On.Menu.MenuScene.orig_BuildScene orig, Menu.MenuScene self)
	{
		if ( true //lastCreatedWorldAcronym == "O7"
		    && self.menu is SlugcatSelectMenu
		    && self.owner is SlugcatSelectMenu.SlugcatPage page)
		{
			SlugcatStats.Name slugcatNumber = page.slugcatNumber;
			MenuScene.SceneID replacementScene = new($"O7-SleepScreen-{slugcatNumber.value}");
			if (SlugBase.Assets.CustomScene.Registry.TryGet(replacementScene, out _))
			{
				self.sceneID = replacementScene;
			}
		}
		orig(self);
	}

	[CanBeNull] static string lastCreatedWorldAcronym = null; 

	static void WorldOnctor(On.World.orig_ctor orig, World self, RainWorldGame game, Region region, string name, bool singleroomworld)
	{
		orig(self, game, region, name, singleroomworld);
		lastCreatedWorldAcronym = region.name;
	}
}