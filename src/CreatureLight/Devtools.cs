using DevInterface;
using OvergrownUrban.CreatureLight;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OvergrownUrban.CreatureLight
{
    public static class Devtools
    {
        public static void Hook()
        {
            On.Room.Loaded += AddObjects;
            On.DevInterface.ObjectsPage.DevObjectGetCategoryFromPlacedType += SetCat;
            On.DevInterface.ObjectsPage.CreateObjRep += MakeRep;
            On.PlacedObject.GenerateEmptyData += GenerateEmptyData;
        }

        private static void GenerateEmptyData(On.PlacedObject.orig_GenerateEmptyData orig, PlacedObject self)
        {
            if (self.type == Enums.PlacedObjectType.CreatureLight)
            {
                self.data = new CreatureLightData(self);
            }
            else
            {
                orig(self);
            }
        }

        private static void MakeRep(On.DevInterface.ObjectsPage.orig_CreateObjRep orig, ObjectsPage self, PlacedObject.Type tp, PlacedObject pObj)
        {
            if (tp == Enums.PlacedObjectType.CreatureLight)
            {
                if (pObj == null)
                {
                    pObj = new PlacedObject(tp, null);
                    pObj.pos = self.owner.room.game.cameras[0].pos + Vector2.Lerp(self.owner.mousePos, new Vector2(-683f, 384f), 0.25f) + Custom.DegToVec(Random.value * 360f) * 0.2f;
                    self.RoomSettings.placedObjects.Add(pObj);
                }

                PlacedObjectRepresentation rep = new CreatureLightRep(self.owner, tp.ToString() + "_Rep", self, pObj, tp.ToString());

                self.tempNodes.Add(rep);
                self.subNodes.Add(rep);
            }
            else
            {
                orig(self, tp, pObj);
            }
        }

        private static ObjectsPage.DevObjectCategories SetCat(On.DevInterface.ObjectsPage.orig_DevObjectGetCategoryFromPlacedType orig, ObjectsPage self, PlacedObject.Type type)
        {
            if (type == Enums.PlacedObjectType.CreatureLight)
            {
                return Enums.DevObjectCategory.OvergrownUrban;
            }

            return orig(self, type);
        }

        private static void AddObjects(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);

            for (var w = 1; w <= 2; w++)
            {
                if (w == 2 && self.warpPoints.Count > 0)
                {
                    continue;
                }

                for (var i = 0; i < self.roomSettings.placedObjects.Count; i++)
                {
                    var pObj = self.roomSettings.placedObjects[i];

                    if (self.abstractRoom.firstTimeRealized || w == 1 && pObj.deactivatedByWarpFilter || w == 2 && !pObj.deactivatedByWarpFilter)
                    {
                        continue;
                    }

                    if (pObj.active && pObj.type == Enums.PlacedObjectType.CreatureLight)
                    {
                        self.AddObject(new CreatureLight(self, pObj));
                    }
                }
            }
        }
    }
}