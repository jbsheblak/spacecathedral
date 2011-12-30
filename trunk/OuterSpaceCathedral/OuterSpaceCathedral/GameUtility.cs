using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    static internal class GameUtility
    {
        static GameUtility()
        {
            Random = new Random();
        }

        /// <summary>
        /// Rectangle representing render target dimensions.
        /// </summary>
        public static Random Random    { get; private set; }
    }
}
