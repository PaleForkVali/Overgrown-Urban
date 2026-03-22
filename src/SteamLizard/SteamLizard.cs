using System.Collections.Generic;
using System.Linq;
using RWCustom;
using Smoke;
using UnityEngine;
using static OvergrownUrban.SteamLizard.Meta;

namespace OvergrownUrban.SteamLizard;

public class SteamLizard(AbstractCreature abstractCreature, World world) : Lizard(abstractCreature, world)
{
	#region ConfigVariables
	//editable
	//what to count as actual end of attack
	//also what defines how far steam can actually go in any circumstances
	const float attackRange = 200f;
	//
	const float maxSteamingChargeUpSeconds = 15f;
	const float fullnessOnSpawn = 0.5f;
	const float steamPercentageForAttack = 0.8f;
	const float maximumDistanceToAttemptAttack = 160f;
	//at initial speed, what percentage of distance to t
	const float steamVelocityCoefficient = 0.3f;
	//the logic is complicated around here, but this loosely affects how many ticks
	//would stun target for
	const float targetStunTicks = 70f;
	//how far from bodychunk surface steam particle may be to count
	const float sizeOfStunningParticle = 20f;
	//how much faster steam depletes than it is created
	const int ratioOfSteaming = 9;
	//time for steaming animation
	const int steamingAnimationTicks = 40;
	//time since last seeing prey to stop steaming
	const int ticksSinceLastSeenCancel = 40;
	//jaw openness treshold for starting steaming
	const float jawOpenPercentage = 0.5f;
	const float knockBackPerTickCoefficient = 0.01f;
	//degree adjustments for emitted steam. always positive
	const float emittingTargetAdjustDegrees = 10f;
	//degree of cone in which lizard would attempt to steam
	const float targetAttemptFireConeDegree = 80f;
	
	
	//don't touch
	const ushort maxSteamingChargeUpTicks = (ushort)(maxSteamingChargeUpSeconds * Plugin.ticksPerSecond);
	
	#endregion

	#region Runtime variables
	readonly Counter steamAvailable = new (maxSteamingChargeUpTicks, countsUp: true) { counter = (int)(fullnessOnSpawn * maxSteamingChargeUpTicks) };
	bool attacking => animation == steaming;
	SteamLizardAI SteamAI => AI as SteamLizardAI; 
	SteamSmoke steamSmoke;
	StaticSoundLoop steamSoundLoop;
	FloatRect steamConfines;
	readonly List<EntityID> spasmingCreatures = [];

	float DegreeOffsetOfTarget
	{
		get
		{
			Vector2 targetPosition = AI.focusCreature!.representedCreature.realizedCreature.mainBodyChunk.pos;
			Vector2 bodyDirection = (bodyChunks[0].pos - bodyChunks[1].pos).normalized;
			return Custom.AimFromOneVectorToAnother(bodyDirection, (targetPosition - mainBodyChunk.pos).normalized);

		}
	}
	#endregion

	public override void Update(bool eu)
	{
		base.Update(eu);
		#region passing ticks
		steamAvailable.Tick();
		//+1 because the timer Tick adds +1 every tick
		if (attacking) steamAvailable.counter -= ratioOfSteaming + 1;
		steamSoundLoop?.Update();
		SteamDamageUpdate();
		#endregion
		
		#region attack state switch
		if (!attacking 
			&& steamAvailable.normalized >= steamPercentageForAttack
		    && AI.behavior == LizardAI.Behavior.Hunt
		    && AI.focusCreature is not null
			&& AI.preyTracker.currentPrey.critRep.visualContact
			&& Custom.DistLess(AI.focusCreature.representedCreature.realizedCreature.mainBodyChunk.pos, mainBodyChunk.pos, maximumDistanceToAttemptAttack)
			&& Mathf.Abs(DegreeOffsetOfTarget) <= targetAttemptFireConeDegree / 2)
		{
			EnterAnimation(steaming, false);
			timeToRemainInAnimation = steamingAnimationTicks;
		}
		if (attacking 
		    && (!Consious 
		        || steamAvailable.normalized <= 0f 
		        || AI.focusCreature is null 
		        || AI.preyTracker.currentPrey.critRep.ticksSinceSeen > ticksSinceLastSeenCancel))
		{
			animation = null;
		}
		#endregion

		#region attack

		if (attacking)
		{
			jawOpen = Mathf.Clamp01(jawOpen + 0.1f);
			if (jawOpen > jawOpenPercentage)
			{
				if (steamSoundLoop is null)
				{
					steamSoundLoop = new StaticSoundLoop(SoundID.Gate_Water_Steam_LOOP,
						mainBodyChunk.pos, room, 0.6f, 1f);
				}
				else
				{
					steamSoundLoop.volume = 0.6f;
				}
				steamSmoke ??= new SteamSmoke(room);
				Vector2 targetPosition = AI.focusCreature!.representedCreature.realizedCreature.mainBodyChunk.pos;
				
				Vector2 bodyDirection = (bodyChunks[0].pos - bodyChunks[1].pos).normalized;
				
				float aimedRange = Mathf.Min( (targetPosition - mainBodyChunk.pos).magnitude, attackRange);
				//that would make it adjust slightly for shooting
				Vector2 realAttackTarget = aimedRange * Custom.DegToVec(
							Custom.VecToDeg(bodyDirection) +
						Mathf.Clamp(
							DegreeOffsetOfTarget,
							-emittingTargetAdjustDegrees,
							emittingTargetAdjustDegrees));
					
				Vector2 emittedSteamVelocity = realAttackTarget * steamVelocityCoefficient;
				IntVector2 tileBetweenMeAndTarget = room.GetTilePosition(Vector2.Lerp(bodyChunks[0].pos, targetPosition, 0.5f));
				steamConfines = room.TileRect(tileBetweenMeAndTarget).Grow(attackRange);
				steamSmoke.EmitSmoke(
					bodyChunks[0].pos + bodyDirection * 10f, //so it doesn't go from center of head but from jaw 
					emittedSteamVelocity, steamConfines, 
					0.9f);
				bodyChunks[0].vel -= emittedSteamVelocity * knockBackPerTickCoefficient;
			}
		}
		else
		{
			if (steamSoundLoop is not null)
			{
				steamSoundLoop.volume = Mathf.Lerp(steamSoundLoop.volume, 0f, 0.075f);
				if (steamSoundLoop.volume < 0.1f) steamSoundLoop.volume = 0f;
			}
			if(spasmingCreatures.Count > 0) spasmingCreatures.Clear();
		}
		#endregion
	}
	
	public override void NewRoom(Room newRoom)
	{
		base.NewRoom(newRoom);
		if(steamSmoke is not null) steamSmoke.room = newRoom;
		if (steamSoundLoop is not null) steamSoundLoop.room = newRoom;
	}

	//logic borrowed from BoxWorm, slightly altered for functional methods
	void SteamDamageUpdate()
	{
		room?.abstractRoom.creatures.ForEach(creature =>
		{
			if (creature.realizedCreature is not null
			    && !creature.realizedCreature.dead
			    && creature.creatureTemplate.type != SteamLizardCritob.SteamLizard
			    && steamConfines.Vector2Inside(room.MiddleOfTile(creature.pos))
			    && !spasmingCreatures.Contains(creature.ID))
			{
				int totalDamage = creature.realizedCreature.bodyChunks.Aggregate(0, (damage, chunk) =>
				{
					if (steamConfines.Vector2Inside(chunk.pos))
					{
						//up to 7 per particle
						return damage + steamSmoke.particles.Aggregate(0, (hitsCounter, particle) =>
						{
							if (particle.life < 0.1f || Vector2.Distance(particle.pos, chunk.pos) > chunk.rad + sizeOfStunningParticle)
								return hitsCounter;
							return hitsCounter + (int)(particle.life * 7f);
						});
					}
					return damage;
				});
				if (totalDamage > 0)
				{
					Creature realizedTarget = creature.realizedCreature;
					int stunTickAmount = (int)Mathf.Min(targetStunTicks * Mathf.Lerp(realizedTarget.Template.baseStunResistance, 1f, 0.5f),
						totalDamage * 200f);
					realizedTarget.Violence(base.firstChunk,
						Custom.DirVec(base.firstChunk.pos, realizedTarget.firstChunk.pos) * 5f, 
						realizedTarget.firstChunk,
						null, 
						Creature.DamageType.Water,
						0.1f,
						//makes it weaker for creatures with weak resistance, stronger for creatures with strong resistance
						stunTickAmount);
					room.AddObject(new CreatureSpasmer(realizedTarget, allowDead: false, stunTickAmount));
					spasmingCreatures.Add(creature.ID);
				}
			}
		});
	}
	
	public override void InitiateGraphicsModule()
	{
		graphicsModule = new SteamLizardGraphicsModule(this);
	}
}