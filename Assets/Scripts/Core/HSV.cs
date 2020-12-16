using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class HSV
    {
        float hue;
        float sat;
        float val;

        public HSV(float hue, float sat, float val)
        {
            this.hue = hue;
            this.sat = sat;
            this.val = val;
        }

        public float Hue
        {
            get { return hue; }
            set { hue = value; }
        }
        public float Sat
        {
            get { return sat; }
            set { sat = value; }
        }
        public float Val
        {
            get { return val; }
            set { val = value; }
        }
    }
}
