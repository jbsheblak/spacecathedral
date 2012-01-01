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
        /// Game random
        /// </summary>
        public static Random Random    { get; private set; }

        /// <summary>
        /// Create a time comparison value from a time query.
        /// </summary>
        /// <param name="timeQuery">Time query string of form "HH:MM:SS", use miltary time (0-23 hours)</param>
        /// <returns>time value</returns>
        public static int GetQueryTimeValue( string time )
        {
            string [] timeTkns = time.Split( new char [] { ':' } );
            return MakeTimeValue( int.Parse(timeTkns[0]), int.Parse(timeTkns[1]), int.Parse(timeTkns[2]) );
        }

        /// <summary>
        /// Get a time comparison value for the current time.
        /// </summary>
        /// <returns>time value</returns>
        public static int GetCurrentTimeValue()
        {
            DateTime now = DateTime.Now;
            return MakeTimeValue( now.Hour, now.Minute, now.Second );
        }

        #region Private

        private static int MakeTimeValue(int hour, int minute, int second)
        {
            return hour * 60 * 60 + minute * 60 + second;
        }

        #endregion
    }
}
