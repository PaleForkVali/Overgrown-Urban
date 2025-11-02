using System;
using Menu;

namespace TestMod;

public static class CutsceneChange
{
	public static void Init()
	{
		On.Menu.MenuScene.BuildScene += MenuSceneOnBuildScene;
	}

	static void MenuSceneOnBuildScene(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
	{
		if (self.menu is SlugcatSelectMenu
		    && self.owner is SlugcatSelectMenu.SlugcatPageContinue pageContinue
		    && (Plugin.devMode 
		    || pageContinue.saveGameData.shelterName is string shelterName
		    && shelterName.Substring(0, shelterName.IndexOf("_", StringComparison.InvariantCulture)) == "O7"))
		{
			SlugcatStats.Name slugcatNumber = pageContinue.slugcatNumber;
			MenuScene.SceneID replacementScene = new($"O7-SleepScreen-{slugcatNumber.value}");
			if (SlugBase.Assets.CustomScene.Registry.TryGet(replacementScene, out _))
			{
				self.sceneID = replacementScene;
			}
		}
		orig(self);
	}

}