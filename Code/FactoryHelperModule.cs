using Celeste;
using Celeste.Mod;
using Monocle;
using System;

namespace FactoryHelper {
    public class FactoryHelperModule : EverestModule {
        public static FactoryHelperModule Instance;

        public FactoryHelperModule() {
            Instance = this;
        }

        public static SpriteBank SpriteBank { get; private set; }
        public static FactoryHelperSession Session => (FactoryHelperSession)Instance._Session;
        public static FactoryHelperSaveData SaveData => (FactoryHelperSaveData)Instance._SaveData;

        public override Type SettingsType => null;
        public override Type SaveDataType => typeof(FactoryHelperSaveData);
        public override Type SessionType => typeof(FactoryHelperSession);

        public override void LoadContent(bool firstLoad) {
            base.LoadContent(firstLoad);
            SpriteBank = new SpriteBank(GFX.Game, "Graphics/FactoryHelper/Sprites.xml");
        }

        public override void Load() {
            FactoryHelperHooks.Load();
        }

        public override void Unload() {
            FactoryHelperHooks.Unload();
        }
    }
}
