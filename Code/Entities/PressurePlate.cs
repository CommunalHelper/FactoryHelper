using Celeste;
using Celeste.Mod.Entities;
using FactoryHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace FactoryHelper.Entities {
    [CustomEntity("FactoryHelper/PressurePlate")]
    public class PressurePlate : Solid {
        private readonly HashSet<string> _activationIds = new();
        private readonly bool _multiplayerMode = false;

        private readonly Button _button;
        private bool _currentButtonState = false;
        private bool _previousButtonState = false;

        public PressurePlate(Vector2 position, string activationIds, bool multiplayerMode) 
            : base(position, 16, 6, false) {
            string[] activationIdArray = activationIds.Split(',');
            Collider.Position.Y += 10;

            _multiplayerMode = multiplayerMode;
            foreach (string activationId in activationIdArray) {
                if (activationId != "") {
                    _activationIds.Add(activationId);
                }
            }

            Add(CaseImage = new Image(GFX.Game["objects/FactoryHelper/pressurePlate/plate_case0"]));
            CaseImage.Position -= new Vector2(2, 2);
            _button = new Button(position);
            Depth = 8020;
            _button.Depth = Depth + 1;
        }

        public PressurePlate(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Attr("activationIds"), data.Bool("multiplayerMode", false)) {
        }

        public Image CaseImage { get; private set; }

        public override void Added(Scene scene) {
            base.Added(scene);
            scene.Add(_button);
        }

        public override void Removed(Scene scene) {
            scene.Remove(_button);
            base.Removed(scene);
        }

        public override void Update() {
            base.Update();
            List<Entity> actors = Scene.Tracker.GetEntities<Actor>();
            _currentButtonState = false;
            foreach (Entity actor in actors) {
                if (Collide.Check(actor, _button)) {
                    if (actor.GetType().FullName == "Celeste.Mod.CelesteNet.Client.Entities.Ghost") {
                        if (_multiplayerMode) { _currentButtonState = true; break; }
                    } else {
                        _currentButtonState = true;
                        break;
                    }
                }
            }

            if (_currentButtonState != _previousButtonState) {
                SendOutSignals(_currentButtonState);
            }

            int target = _currentButtonState ? 2 : -2;
            _button.Image.Position.Y = Calc.Approach(_button.Image.Position.Y, target, 40f * Engine.DeltaTime);

            _previousButtonState = _currentButtonState;
        }


        private void SendOutSignals(bool shouldActivate = true) {
            foreach (FactoryActivator activator in Scene.Tracker.GetComponents<FactoryActivator>()) {
                if (_activationIds.Contains(activator.ActivationId)) {
                    if (shouldActivate) {
                        activator.Activate(false);
                    } else {
                        activator.Deactivate(false);
                    }
                }
            }
        }

        [Tracked]
        public class Button : Entity {
            public Button(Vector2 position) : base(position) {
                Add(Image = new Image(GFX.Game["objects/FactoryHelper/pressurePlate/plate_button0"]));
                Collider = new Hitbox(14, 7, 1, 6);
                Image.Position.X -= 2;
                Image.Position.Y -= 2;
            }

            public Image Image { get; private set; }
        }
    }
}
