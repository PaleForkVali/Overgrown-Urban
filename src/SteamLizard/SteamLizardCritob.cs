using System.Collections.Generic;
using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;
using CreatureType = CreatureTemplate.Type;

namespace OvergrownUrban.SteamLizard;

public class SteamLizardCritob : Critob
{
	public static readonly CreatureTemplate.Type SteamLizard = new("SteamLizard", true);
	
	public SteamLizardCritob() : base(SteamLizard)
	{
		LoadedPerformanceCost = 60f;
		SandboxPerformanceCost = new SandboxPerformanceCost(2f, 0.2f);
	}

	public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit)
	{
		return new SteamLizardAI(acrit, acrit.world);
	}

	public override Creature CreateRealizedCreature(AbstractCreature acrit)
	{
		return new SteamLizard(acrit, acrit.world);
	}

	public override CreatureTemplate CreateTemplate()
	{
		//LizardBreeds.BreedTemplate accepts ancestor
		//however, ancestor only applies to CTemplate
		//BreedTemplate DOES NOT inherit params from ancestor
		//so we create pink lizard template and change it instead
		//some of the more misleading places of the game
		CreatureTemplate t = LizardBreeds.BreedTemplate(CreatureType.PinkLizard,
			StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.LizardTemplate), null, null, null);
		t.type = SteamLizard;
		t.name = "Steam Lizard";
		LizardBreedParams @params = (t.breedParameters as LizardBreedParams)!;
		@params.standardColor = new(0.35f, 0.26f, 0.196f);
		@params.headGraphics[4] = 77;
		
		t.doPreBakedPathing = false;
		t.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.PinkLizard);
		
		return t;
	}

	public override void EstablishRelationships()
	{
		// i hope its relationships would automatically be inherited from pink lizard?..
		// it definitely hunts for slugcat
		Relationships self = new(SteamLizard);
		self.Ignores(SteamLizard);
	}

	public override CreatureState CreateState(AbstractCreature acrit)
	{
		return new LizardState(acrit)
		{
			limbHealth = [1f,1f,1f,1f,1f,1f]
		};
	}

	public override IEnumerable<string> WorldFileAliases()
	{
		yield return "SteamLizard";
		yield return "steamlizard";
		yield return "steamLizard";
		yield return "Steam Lizard";
		yield return "steam lizard";
	}
	
	public override void LoadResources(RainWorld rainWorld)
	{
		Icon = new SimpleIcon(Ext.IconAtlasName("Kill_SteamLizard"), new(0.35f, 0.26f, 0.196f));
	}
}