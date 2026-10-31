using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ColorType = PlacedObject.LightSourceData.ColorType;
using BlinkType = PlacedObject.LightSourceData.BlinkType;
using RWCustom;

namespace OvergrownUrban.CreatureLight
{
    public class CreatureLightData : PlacedObject.Data
    {
        public Vector2 panelPos;

        public Vector2 lightHandle;

        public Vector2 detectorHandle;

        public Color[] colour = new Color[2];
        
        public ColorType[] colorType = new ColorType[2];

        public BlinkType[] blinkType = new BlinkType[2];

        public float[] blinkRate = new float[2];
        
        public float[] strength = new float[2];

        public bool[] flat = new bool[2];

        public bool[] fadeWithSun = new bool[2];

        public bool[] nightLight = new bool[2];

        public List<CreatureTemplate.Type> creaturesToDetect;

        public float LightRad => lightHandle.magnitude;

        public float DetectRad => detectorHandle.magnitude;

        public static int SortCritList(CreatureTemplate.Type a, CreatureTemplate.Type b)
        {
            if (a.index == b.index)
            {
                return 0;
            }

            return a.index > b.index ? 1 : -1;
        }

        public CreatureLightData(PlacedObject owner) : base(owner)
        {
            lightHandle = new Vector2(0f, 100f);
            detectorHandle = new Vector2(0f, 50f);

            panelPos = Custom.DegToVec(30f) * 100f;

            for (int i = 0; i < 2; i++)
            {
                colour[i] = (i == 0 ? Color.red : Color.green);
                colorType[i] = ColorType.Environment;
                blinkType[i] = BlinkType.None;
                blinkRate[i] = 0f;
                strength[i] = 1f;
                flat[i] = false;
                fadeWithSun[i] = false;
                nightLight[i] = false;
            }

            creaturesToDetect = new List<CreatureTemplate.Type>();
        }

        public override void FromString(string s)
        {
            string[] data = s.Split('~');

            panelPos.x = float.Parse(data[0], System.Globalization.NumberStyles.Float);
            panelPos.y = float.Parse(data[1], System.Globalization.NumberStyles.Float);

            lightHandle.x = float.Parse(data[2], System.Globalization.NumberStyles.Float);
            lightHandle.y = float.Parse(data[3], System.Globalization.NumberStyles.Float);
            detectorHandle.x = float.Parse(data[4], System.Globalization.NumberStyles.Float);
            detectorHandle.y = float.Parse(data[5], System.Globalization.NumberStyles.Float);

            for (int l = 0; l < 2; l++)
            {
                int i = 6 + (l * 10);

                colour[l] = new Color(
                    float.Parse(data[i], System.Globalization.NumberStyles.Float),
                    float.Parse(data[i + 1], System.Globalization.NumberStyles.Float),
                    float.Parse(data[i + 2], System.Globalization.NumberStyles.Float));

                colorType[l] = data[i + 3] == "custom" ? null : ExtEnum<ColorType>.Parse(typeof(ColorType), data[i + 3], true) as ColorType;
                blinkType[l] = ExtEnum<BlinkType>.Parse(typeof(BlinkType), data[i + 4], true) as BlinkType;

                blinkRate[l] = float.Parse(data[i + 5], System.Globalization.NumberStyles.Float);
                strength[l] = float.Parse(data[i + 6], System.Globalization.NumberStyles.Float);

                flat[l] = bool.Parse(data[i + 7]);
                fadeWithSun[l] = bool.Parse(data[i + 8]);
                nightLight[l] = bool.Parse(data[i + 9]);
            }

            string[] critData = data[26].Substring(1, data[26].Length - 2).Split('|');
            if (critData.Length > 0 && critData[0] != "")
            {
                for (int c = 0; c < critData.Length; c++)
                {
                    creaturesToDetect.Add(ExtEnum<CreatureTemplate.Type>.Parse(typeof(CreatureTemplate.Type), critData[c], true) as CreatureTemplate.Type);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.Append(panelPos.x).Append("~").Append(panelPos.y).Append("~");

            s.Append(lightHandle.x).Append("~").Append(lightHandle.y).Append("~");
            s.Append(detectorHandle.x).Append("~").Append(detectorHandle.y).Append("~");

            for (int i = 0; i < 2; i++)
            {
                s.Append(colour[i].r).Append("~").Append(colour[i].g).Append("~").Append(colour[i].b).Append("~");
                s.Append(colorType[i] == null ? "custom" : colorType[i].ToString()).Append("~");
                s.Append(blinkType[i].ToString()).Append("~");
                s.Append(blinkRate[i]).Append("~");
                s.Append(strength[i]).Append("~");
                s.Append(flat[i]).Append("~");
                s.Append(fadeWithSun[i]).Append("~");
                s.Append(nightLight[i]).Append("~");
            }

            s.Append("[");

            for (int i = 0; i < creaturesToDetect.Count; i++)
            {
                s.Append(creaturesToDetect[i].ToString());

                if (i != creaturesToDetect.Count - 1)
                {
                    s.Append("|");
                }
            }

            s.Append("]");

            return SaveUtils.AppendUnrecognizedStringAttrs(s.ToString(), "~", unrecognizedAttributes);
        }
    }
}
