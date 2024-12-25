using Celeste;
using Celeste.Mod.Entities;
using FactoryHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace FactoryHelper.Triggers {
    [CustomEntity("FactoryHelper/FactoryActivationTrigger")]
    public class FactoryActivationTrigger : Trigger {
        private readonly bool _lockState;
        private readonly bool _persistent;
        private readonly ActivationModes _mode;
        private readonly HashSet<string> _activationIds = [];

        public FactoryActivationTrigger(EntityData data, Vector2 offset) 
            : base(data, offset) {
            string[] activationIds = data.Attr("activationIds", "").Split(',');

            _persistent = data.Bool("persistent", false);
            _lockState = data.Bool("lockState", true);
            _mode = data.Enum("mode", ActivationModes.Activate);
            Add(Activator = new FactoryActivator());
            Activator.ActivationId = data.Attr("ownActivationId") == string.Empty ? null : data.Attr("ownActivationId");
            Activator.StartOn = Activator.ActivationId == null;

            foreach (string activationId in activationIds) {
                if (activationId != "") {
                    _activationIds.Add(activationId);
                }
            }
        }

        private enum ActivationModes {
            Activate,
            Deactivate
        }

        public FactoryActivator Activator { get; }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if (Activator.IsOn) {
                SetSessionTags(_mode);
                SendOutSignals(_mode);
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Activator.HandleStartup(scene);
        }

        private void SendOutSignals(ActivationModes mode) {
            foreach (FactoryActivator activator in Scene.Tracker.GetComponents<FactoryActivator>()) {
                if (_activationIds.Contains(activator.ActivationId)) {
                    if (mode == ActivationModes.Activate) {
                        activator.Activate(_lockState);
                    } else if (mode == ActivationModes.Deactivate) {
                        activator.Deactivate(_lockState);
                    }
                }
            }
        }

        private void SetSessionTags(ActivationModes mode) {
            if (_persistent) {
                Level level = Scene as Level;
                foreach (string activationId in _activationIds) {
                    level.Session.SetFlag($"FactoryActivation:{activationId}", mode == ActivationModes.Activate);
                }
            }
        }
    }
}
