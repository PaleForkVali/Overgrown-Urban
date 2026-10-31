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

        public bool CritDetected => room.abstractRoom.creatures.FindAll(c => c.realizedCreature != null && data.creaturesToDetect.Contains(c.creatureTemplate.type) && Custom.DistLess(c.realizedCreature.mainBodyChunk.pos, pObj.pos, data.DetectRad)).Count > 0;

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
                Light[i].setAlpha = CritDetected ? (i == 0 ? 0f : data.strength[1]) : (i == 0 ? data.strength[0] : 0f);
                Light[i].fadeWithSun = data.fadeWithSun[i];
                Light[i].colorFromEnvironment = data.colorType[i] == PlacedObject.LightSourceData.ColorType.Environment;
                Light[i].flat = data.flat[i];
                Light[i].effectColor = Mathf.Max(-1, (int)data.colorType[i] - 2);
                Light[i].setBlinkProperties(data.blinkType[i], data.blinkRate[i]);
            }
        }
    }
}
