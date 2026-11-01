using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OvergrownUrban.CreatureLight
{
    public class CreatureLight : UpdatableAndDeletable
    {
        public PlacedObject pObj;

        public CreatureLightData data => pObj.data as CreatureLightData;

        public LightSource[] Light;

        public float strength
        {
            get
            {
                float val = 0f;
                foreach (AbstractCreature crit in room.abstractRoom.creatures.FindAll(c => c.realizedCreature != null && data.creaturesToDetect.Contains(c.creatureTemplate.type)))
                {
                    float calc = 0f;
                    if (data.innerDetectionRad == 1f)
                    {
                        calc = Custom.DistLess(crit.realizedCreature.mainBodyChunk.pos, pObj.pos, data.DetectRad) ? 1f : 0f;
                    }
                    else
                    {
                        calc = Mathf.InverseLerp(data.DetectRad, data.DetectRad * data.innerDetectionRad, (pObj.pos - crit.realizedCreature.mainBodyChunk.pos).magnitude);
                    }

                    if (calc > val)
                    {
                        val = calc;
                    }
                }

                return val;
            }
        }

        public CreatureLight(Room room, PlacedObject pObj) : base()
        {
            this.room = room;
            this.pObj = pObj;

            Light = new LightSource[2];
            for (int i = 0; i < 2; i++)
            {
                Light[i] = new LightSource(pObj.pos, true, data.colour[i], this);
                Light[i].setRad = data.LightRad;
                Light[i].setAlpha = data.strength[i];
                Light[i].fadeWithSun = data.fadeWithSun[i];
                Light[i].colorFromEnvironment = data.colorType[i] == PlacedObject.LightSourceData.ColorType.Environment;
                Light[i].flat = data.flat[i];
                Light[i].effectColor = Mathf.Max(-1, (int)data.colorType[i] - 2);
                Light[i].setBlinkProperties(data.blinkType[i], data.blinkRate[i]);

                room.AddObject(Light[i]);
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            for (int i = 0; i < 2; i++)
            {
                if (data.colorType[i] == null)
                {
                    Light[i].color = data.colour[i];
                }

                Light[i].setPos = pObj.pos;
                Light[i].setRad = data.LightRad;
                Light[i].setAlpha = (i == 0) ? Mathf.Lerp(data.strength[i], 0f, strength) : Mathf.Lerp(0f, data.strength[i], strength);
                Light[i].fadeWithSun = data.fadeWithSun[i];
                Light[i].colorFromEnvironment = data.colorType[i] == PlacedObject.LightSourceData.ColorType.Environment;
                Light[i].flat = data.flat[i];
                Light[i].effectColor = Mathf.Max(-1, (int)data.colorType[i] - 2);
                Light[i].setBlinkProperties(data.blinkType[i], data.blinkRate[i]);
            }
        }
    }
}
