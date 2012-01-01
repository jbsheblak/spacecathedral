using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OuterSpaceCathedral
{
    public class CollisionMessage
    {
    }

    public class BulletCollisionMessage : CollisionMessage
    {
        public BulletCollisionMessage(BulletFirer bulletFirer)
        {
            BulletFirer = bulletFirer;
        }

        public BulletFirer BulletFirer { private set; get; }
    }
}
