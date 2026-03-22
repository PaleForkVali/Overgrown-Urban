namespace OvergrownUrban.SteamLizard;

public static class Meta
{
	public static Lizard.Animation steaming = new("steaming", true);
	public static void Apply()
	{
		Fisobs.Core.Content.Register(new SteamLizardCritob());
	}
}