using System;

namespace NeoServer.Server.Helpers
{
    public class ServerRandom : Random
    {
        public double Next(double mu = 0, double sigma = 1)
        {
            var u1 = NextDouble();
            var u2 = NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = mu + sigma * rand_std_normal;

            return rand_normal;
        }

        private static ServerRandom Instance;

        public static ServerRandom Random
        {
            get
            {
                if (Instance == null) Instance = new ServerRandom();

                return Instance;
            }

        }

        /// <summary>
        /// Random value in a interval using gaussian
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double NextInRange(double min, double max)
        {
            var diff = max - min;
            var gaussian = Next(0.5f, 0.25f);

            double increment;
            if (gaussian < 0.0)
            {
                increment = diff / 2;
            }
            else if (gaussian > 1.0)
            {
                increment = (diff + 1) / 2;
            }
            else
            {
                increment = Math.Round(gaussian * diff);
            }
            return (double)(min + increment);
        }
    }
}
