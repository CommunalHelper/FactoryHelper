using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace FactoryHelper.Entities {
    [CustomEntity("FactoryHelper/BoomBoxZip")]
    public class BoomBoxZip : BoomBox {
        public float percent;
        public Vector2 start, target;
        private ZipMoverPathRenderer pathRenderer;

        public BoomBoxZip(EntityData data, Vector2 offset) 
            : this(data.Position + offset, data.Attr("activationId", ""), data.Float("initialDelay", 0f), data.Bool("startActive", false), data.Attr("spriteDir", ""), data.Nodes[0] + offset) {
        }

        public BoomBoxZip(Vector2 position, string activationId, float initialDelay, bool startActive, string spriteDir, Vector2 target) 
            : base(position, activationId, initialDelay, startActive, spriteDir) {
            Add(new Coroutine(ZipMoverSequence()));
            this.start = this.Position;
            this.target = target;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            scene.Add(pathRenderer = new ZipMoverPathRenderer(this));
        }

        private IEnumerator ZipMoverSequence() {
            start = Position;
            while (true) {
                if (!HasPlayerRider()) {
                    yield return null;
                    continue;
                }
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
                StartShaking(0.1f);
                yield return 0.1f;
                StopPlayerRunIntoAnimation = false;
                float at2 = 0f;
                while (at2 < 1f) {
                    yield return null;
                    at2 = Calc.Approach(at2, 1f, 2f * Engine.DeltaTime);
                    percent = Ease.SineIn(at2);
                    Vector2 vector = Vector2.Lerp(start, target, percent);
                    ScrapeParticlesCheck(vector);
                    if (Scene.OnInterval(0.1f)) {
                        pathRenderer.CreateSparks();
                    }

                    MoveTo(vector);
                    _boomCollider.Position = vector + new Vector2(Width / 2, Height / 2);
                }

                StartShaking(0.2f);
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                SceneAs<Level>().Shake();
                StopPlayerRunIntoAnimation = true;
                yield return 0.5f;
                StopPlayerRunIntoAnimation = false;
                at2 = 0f;
                while (at2 < 1f) {
                    yield return null;
                    at2 = Calc.Approach(at2, 1f, 0.5f * Engine.DeltaTime);
                    percent = 1f - Ease.SineIn(at2);
                    Vector2 position = Vector2.Lerp(target, start, Ease.SineIn(at2));
                    MoveTo(position);
                    _boomCollider.Position = position + new Vector2(Width / 2, Height / 2);
                }

                StopPlayerRunIntoAnimation = true;
                StartShaking(0.2f);
                yield return 0.5f;
            }
        }

        private void ScrapeParticlesCheck(Vector2 to) {
            if (!base.Scene.OnInterval(0.03f)) {
                return;
            }

            bool flag = to.Y != base.ExactPosition.Y;
            bool flag2 = to.X != base.ExactPosition.X;
            if (flag && !flag2) {
                int num = Math.Sign(to.Y - base.ExactPosition.Y);
                Vector2 value = (num != 1) ? base.TopLeft : base.BottomLeft;
                int num2 = 4;
                if (num == 1) {
                    num2 = Math.Min((int)base.Height - 12, 20);
                }

                int num3 = (int)base.Height;
                if (num == -1) {
                    num3 = Math.Max(16, (int)base.Height - 16);
                }

                if (base.Scene.CollideCheck<Solid>(value + new Vector2(-2f, num * -2))) {
                    for (int i = num2; i < num3; i += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopLeft + new Vector2(0f, (float)i + (float)num * 2f), (num == 1) ? (-(float)Math.PI / 4f) : ((float)Math.PI / 4f));
                    }
                }

                if (base.Scene.CollideCheck<Solid>(value + new Vector2(base.Width + 2f, num * -2))) {
                    for (int j = num2; j < num3; j += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopRight + new Vector2(-1f, (float)j + (float)num * 2f), (num == 1) ? ((float)Math.PI * -3f / 4f) : ((float)Math.PI * 3f / 4f));
                    }
                }
            } else {
                if (!flag2 || flag) {
                    return;
                }

                int num4 = Math.Sign(to.X - base.ExactPosition.X);
                Vector2 value2 = (num4 != 1) ? base.TopLeft : base.TopRight;
                int num5 = 4;
                if (num4 == 1) {
                    num5 = Math.Min((int)base.Width - 12, 20);
                }

                int num6 = (int)base.Width;
                if (num4 == -1) {
                    num6 = Math.Max(16, (int)base.Width - 16);
                }

                if (base.Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, -2f))) {
                    for (int k = num5; k < num6; k += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.TopLeft + new Vector2((float)k + (float)num4 * 2f, -1f), (num4 == 1) ? ((float)Math.PI * 3f / 4f) : ((float)Math.PI / 4f));
                    }
                }

                if (base.Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, base.Height + 2f))) {
                    for (int l = num5; l < num6; l += 8) {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, base.BottomLeft + new Vector2((float)l + (float)num4 * 2f, 0f), (num4 == 1) ? ((float)Math.PI * -3f / 4f) : (-(float)Math.PI / 4f));
                    }
                }
            }
        }

        private class ZipMoverPathRenderer : Entity {
            private static readonly Color ropeColor = Calc.HexToColor("4d3c22");
            private static readonly Color ropeLightColor = Calc.HexToColor("766c49");

            public BoomBoxZip zipMover;

            private MTexture cog;
            private Vector2 from;
            private Vector2 to;
            private Vector2 sparkAdd;
            private float sparkDirFromA;
            private float sparkDirFromB;
            private float sparkDirToA;
            private float sparkDirToB;

            public ZipMoverPathRenderer(BoomBoxZip zipMover) {
                base.Depth = 5000;
                this.zipMover = zipMover;
                from = this.zipMover.start + new Vector2(this.zipMover.Width / 2f, this.zipMover.Height / 2f);
                to = this.zipMover.target + new Vector2(this.zipMover.Width / 2f, this.zipMover.Height / 2f);
                sparkAdd = (from - to).SafeNormalize(5f).Perpendicular();
                float num = (from - to).Angle();
                sparkDirFromA = num + (float)Math.PI / 8f;
                sparkDirFromB = num - (float)Math.PI / 8f;
                sparkDirToA = num + (float)Math.PI - (float)Math.PI / 8f;
                sparkDirToB = num + (float)Math.PI + (float)Math.PI / 8f;
                cog = GFX.Game["objects/zipmover/cog"];
            }

            public void CreateSparks() {
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromB);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToB);
            }

            public override void Render() {
                DrawCogs(Vector2.UnitY, Color.Black);
                DrawCogs(Vector2.Zero);
            }

            private void DrawCogs(Vector2 offset, Color? colorOverride = null) {
                Vector2 vector = (to - from).SafeNormalize();
                Vector2 value = vector.Perpendicular() * 3f;
                Vector2 value2 = -vector.Perpendicular() * 4f;
                float rotation = zipMover.percent * (float)Math.PI * 2f;
                Draw.Line(from + value + offset, to + value + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
                Draw.Line(from + value2 + offset, to + value2 + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
                for (float num = 4f - zipMover.percent * (float)Math.PI * 8f % 4f; num < (to - from).Length(); num += 4f) {
                    Vector2 value3 = from + value + vector.Perpendicular() + vector * num;
                    Vector2 value4 = to + value2 - vector * num;
                    Draw.Line(value3 + offset, value3 + vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
                    Draw.Line(value4 + offset, value4 - vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
                }

                cog.DrawCentered(from + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
                cog.DrawCentered(to + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
            }
        }
    }
}
