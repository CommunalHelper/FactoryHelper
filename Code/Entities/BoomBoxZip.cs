using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace FactoryHelper.Entities {
    [CustomEntity("FactoryHelper/BoomBoxZip")]
    public class BoomBoxZip : BoomBox {
        private float _percent;
        private Vector2 _start, _target;
        private ZipMoverPathRenderer _pathRenderer;

        private readonly string _spriteDir;
        private readonly Color _ropeColor, _ropeLightColor, _sparkParticleColor;

        public BoomBoxZip(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Attr("activationId", ""), data.Float("initialDelay", 0f), data.Bool("startActive", false), data.Nodes[0] + offset,
                   data.Attr("spriteDir", ""), data.HexColor("ropeColor", Calc.HexToColor("4d3c22")), data.HexColor("ropeLightColor", Calc.HexToColor("766c49")), data.HexColor("sparkParticleColor", Calc.HexToColor("fff538"))) { }

        public BoomBoxZip(Vector2 position, string activationId, float initialDelay, bool startActive, Vector2 target, string spriteDir, Color ropeColor, Color ropeLightColor, Color sparkParticleColor)
            : base(position, activationId, initialDelay, startActive, spriteDir) {
            Add(new Coroutine(ZipMoverSequence()));
            _start = position;
            _target = target;

            _spriteDir = string.IsNullOrEmpty(spriteDir) ? "objects/FactoryHelper/boomBox" : spriteDir;
            _ropeColor = ropeColor;
            _ropeLightColor = ropeLightColor;
            _sparkParticleColor = sparkParticleColor;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            scene.Add(_pathRenderer = new ZipMoverPathRenderer(this));
        }

        private IEnumerator ZipMoverSequence() {
            _start = Position;
            while (true) {
                if (!HasPlayerRider()) {
                    yield return null;
                    continue;
                }

                Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
                StartShaking(0.1f);
                yield return 0.1f;
                StopPlayerRunIntoAnimation = false;

                float at = 0f;
                while (at < 1f) {
                    yield return null;
                    at = Calc.Approach(at, 1f, 2f * Engine.DeltaTime);
                    _percent = Ease.SineIn(at);
                    Vector2 position = Vector2.Lerp(_start, _target, _percent);
                    ScrapeParticlesCheck(position);
                    if (Scene.OnInterval(0.1f)) {
                        _pathRenderer.CreateSparks();
                    }

                    MoveTo(position);
                    _boomCollider.Position = position + new Vector2(Width / 2, Height / 2);
                }

                StartShaking(0.2f);
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                SceneAs<Level>().Shake();
                StopPlayerRunIntoAnimation = true;
                yield return 0.5f;
                StopPlayerRunIntoAnimation = false;

                at = 0f;
                while (at < 1f) {
                    yield return null;
                    at = Calc.Approach(at, 1f, 0.5f * Engine.DeltaTime);
                    _percent = 1f - Ease.SineIn(at);
                    Vector2 position = Vector2.Lerp(_target, _start, Ease.SineIn(at));
                    MoveTo(position);
                    _boomCollider.Position = position + new Vector2(Width / 2, Height / 2);
                }

                StopPlayerRunIntoAnimation = true;
                StartShaking(0.2f);
                yield return 0.5f;
            }
        }

        private void ScrapeParticlesCheck(Vector2 to) {
            if (!Scene.OnInterval(0.03f)) {
                return;
            }

            bool movingY = to.Y != ExactPosition.Y;
            bool movingX = to.X != ExactPosition.X;
            if (movingY && !movingX) {
                int movingDir = Math.Sign(to.Y - ExactPosition.Y);
                Vector2 checkFrom = movingDir == 1 ? BottomLeft : TopLeft;

                int particlesStart = 4;
                if (movingDir == 1) {
                    particlesStart = Math.Min((int)Height - 12, 20);
                }

                int particlesEnd = (int)Height;
                if (movingDir == -1) {
                    particlesEnd = Math.Max(16, (int)Height - 16);
                }

                if (Scene.CollideCheck<Solid>(checkFrom + new Vector2(-2f, movingDir * -2))) {
                    for (int i = particlesStart; i < particlesEnd; i += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2(0f, i + movingDir * 2f), (movingDir == 1) ? (-MathF.PI / 4f) : (MathF.PI / 4f));
                    }
                }

                if (Scene.CollideCheck<Solid>(checkFrom + new Vector2(Width + 2f, movingDir * -2))) {
                    for (int i = particlesStart; i < particlesEnd; i += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopRight + new Vector2(-1f, i + movingDir * 2f), (movingDir == 1) ? (MathF.PI * -3f / 4f) : (MathF.PI * 3f / 4f));
                    }
                }
            } else if (movingX && !movingY) {
                int movingDir = Math.Sign(to.X - ExactPosition.X);
                Vector2 checkFrom = movingDir == 1 ? TopRight : TopLeft;

                int particlesStart = 4;
                if (movingDir == 1) {
                    particlesStart = Math.Min((int)Width - 12, 20);
                }

                int particlesEnd = (int)Width;
                if (movingDir == -1) {
                    particlesEnd = Math.Max(16, (int)Width - 16);
                }

                if (Scene.CollideCheck<Solid>(checkFrom + new Vector2(movingDir * -2, -2f))) {
                    for (int i = particlesStart; i < particlesEnd; i += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2(i + movingDir * 2f, -1f), (movingDir == 1) ? (MathF.PI * 3f / 4f) : (MathF.PI / 4f));
                    }
                }

                if (Scene.CollideCheck<Solid>(checkFrom + new Vector2(movingDir * -2, Height + 2f))) {
                    for (int i = particlesStart; i < particlesEnd; i += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, BottomLeft + new Vector2(i + movingDir * 2f, 0f), (movingDir == 1) ? (MathF.PI * -3f / 4f) : (-MathF.PI / 4f));
                    }
                }
            }
        }

        private class ZipMoverPathRenderer : Entity {
            private readonly BoomBoxZip _zipMover;

            private readonly MTexture _cogTexture;
            private readonly Vector2 _from, _to;
            private readonly Vector2 _sparkAdd;
            private readonly float _sparkDirFromA, _sparkDirFromB;
            private readonly float _sparkDirToA, _sparkDirToB;

            public ZipMoverPathRenderer(BoomBoxZip zipMover) {
                Depth = 5000;
                _zipMover = zipMover;
                _from = _zipMover._start + new Vector2(_zipMover.Width / 2f, _zipMover.Height / 2f);
                _to = _zipMover._target + new Vector2(_zipMover.Width / 2f, _zipMover.Height / 2f);
                _sparkAdd = (_from - _to).SafeNormalize(5f).Perpendicular();
                float backwards = (_from - _to).Angle();
                _sparkDirFromA = backwards + MathF.PI / 8f;
                _sparkDirFromB = backwards - MathF.PI / 8f;
                _sparkDirToA = backwards + MathF.PI - MathF.PI / 8f;
                _sparkDirToB = backwards + MathF.PI + MathF.PI / 8f;
                string customCogSprite = zipMover._spriteDir + "/zipMoverCog";
                _cogTexture = GFX.Game[GFX.Game.Has(customCogSprite) ? customCogSprite : "objects/zipmover/cog"];
            }

            public void CreateSparks() {
                Color sparkParticleColor = _zipMover._sparkParticleColor;
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, _from + _sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkParticleColor, _sparkDirFromA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, _from - _sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkParticleColor, _sparkDirFromB);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, _to + _sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkParticleColor, _sparkDirToA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, _to - _sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkParticleColor, _sparkDirToB);
            }

            public override void Render() {
                DrawCogs(Vector2.UnitY, Color.Black);
                DrawCogs(Vector2.Zero);
            }

            private void DrawCogs(Vector2 offset, Color? colorOverride = null) {
                Vector2 direction = (_to - _from).SafeNormalize();
                Vector2 rightRopeOffset = direction.Perpendicular() * 3f;
                Vector2 leftRopeOffset = -direction.Perpendicular() * 4f;

                Draw.Line(_from + rightRopeOffset + offset, _to + rightRopeOffset + offset, colorOverride ?? _zipMover._ropeColor);
                Draw.Line(_from + leftRopeOffset + offset, _to + leftRopeOffset + offset, colorOverride ?? _zipMover._ropeColor);
                for (float num = 4f - _zipMover._percent * MathF.PI * 8f % 4f; num < (_to - _from).Length(); num += 4f) {
                    Vector2 rightSmallRopePos = _from + rightRopeOffset + direction.Perpendicular() + direction * num;
                    Vector2 leftSmallRopePos = _to + leftRopeOffset - direction * num;
                    Draw.Line(rightSmallRopePos + offset, rightSmallRopePos + direction * 2f + offset, colorOverride ?? _zipMover._ropeLightColor);
                    Draw.Line(leftSmallRopePos + offset, leftSmallRopePos - direction * 2f + offset, colorOverride ?? _zipMover._ropeLightColor);
                }

                float cogRotation = _zipMover._percent * MathF.PI * 2f;
                _cogTexture.DrawCentered(_from + offset, colorOverride ?? Color.White, 1f, cogRotation);
                _cogTexture.DrawCentered(_to + offset, colorOverride ?? Color.White, 1f, cogRotation);
            }
        }
    }
}
