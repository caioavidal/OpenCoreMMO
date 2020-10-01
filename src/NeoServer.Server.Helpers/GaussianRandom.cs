using System;

namespace NeoServer.Server.Helpers
{
    public class GaussianRandom : Random
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

        private static GaussianRandom Instance;

        public static GaussianRandom Random
        {
            get
            {
                if (Instance == null) Instance = new GaussianRandom();

                return Instance;
            }

        }
    }
}
