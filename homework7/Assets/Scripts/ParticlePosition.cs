using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class ParticlePosition
    {
        public float radius, angle, time;
        public bool clockDirection;
        public ParticlePosition(float radius, float angle, float time, bool clockDirection)
        {
            this.radius = radius;
            this.angle = angle;
            this.time = time;
            this.clockDirection = clockDirection;
        }
    }
}
