using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    public static partial class Game
    {
        public static float SimulationStep = 1/60f;

        public static World World;
        public static CustomPhysics Physics;
    }

}

