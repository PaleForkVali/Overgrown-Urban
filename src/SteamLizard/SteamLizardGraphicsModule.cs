using System;
using UnityEngine;
using Watcher;
using Random = UnityEngine.Random;

namespace OvergrownUrban.SteamLizard;

public class SteamLizardGraphicsModule : LizardGraphics
{
	public SteamLizardGraphicsModule(PhysicalObject owner) : base(owner)
	{
		Random.State prevState = Random.state;
		Random.InitState(lizard.abstractCreature.ID.RandomSeed);
		Array.Resize(ref limbs, 6);
		limbs[4] = limbs[2];
		limbs[5] = limbs[3];
		limbs[2] = new LizardLimb(this, owner.bodyChunks[1], 4, 2.5f, 0.7f, 0.99f, lizard.lizardParams.limbSpeed,
			lizard.lizardParams.limbQuickness, null);
		limbs[3] = new LizardLimb(this, owner.bodyChunks[1], 5, 2.5f, 0.7f, 0.99f, lizard.lizardParams.limbSpeed,
			lizard.lizardParams.limbQuickness, limbs[4]);
		
		Array.Resize(ref bodyParts, bodyParts.Length + 2);
		for (int i = bodyParts.Length-1; i >= 4 ; i--)
		{
			bodyParts[i] = bodyParts[i - 2];
		}
		bodyParts[2] = limbs[2];
		bodyParts[3] = limbs[3];
		
		//renewing sprite positions since Lizard ctor thinks whatever amount of legs is created in it is the final one
		int spriteStart = startOfExtraSprites;
		foreach (var cosmetic in cosmetics)
		{
			cosmetic.startSprite = spriteStart;
			spriteStart += cosmetic.numberOfSprites;
		}
		
		Random.state = prevState;
	}
	
}