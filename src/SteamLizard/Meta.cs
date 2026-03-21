using System;
using System.Collections.Generic;
using System.Globalization;
using MoreSlugcats;
using UnityEngine;
using RWCustom;
using Watcher;
using Random = UnityEngine.Random;

namespace OvergrownUrban.SteamLizard;

public static class Meta
{
	public static Lizard.Animation steaming = new("steaming", true);
	public static void Apply()
	{
		Fisobs.Core.Content.Register(new SteamLizardCritob());
	}
}