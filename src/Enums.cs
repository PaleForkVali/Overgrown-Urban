using DevInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvergrownUrban
{
    public static class Enums
    {
        public static void Register()
        {
            PlacedObjectType.Register();
            DevObjectCategory.Register();
        }

        public static void Unregister()
        {
            PlacedObjectType.UnRegister();
            DevObjectCategory.UnRegister();
        }

        public static class PlacedObjectType
        {
            public static PlacedObject.Type CreatureLight;

            public static void Register()
            {
                CreatureLight = new PlacedObject.Type("CreatureLight", true);
            }

            public static void UnRegister()
            {
                if (CreatureLight != null) CreatureLight.Unregister(); CreatureLight = null;
            }
        }

        public static class DevObjectCategory
        {
            public static ObjectsPage.DevObjectCategories OvergrownUrban;

            public static void Register()
            {
                OvergrownUrban = new ObjectsPage.DevObjectCategories("OvergrownUrban", true);
            }

            public static void UnRegister()
            {
                if (OvergrownUrban != null) OvergrownUrban.Unregister(); OvergrownUrban = null;
            }
        }
    }
}
