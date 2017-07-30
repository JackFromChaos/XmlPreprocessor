using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.utils
{
	public class DioRandom
	{
		protected static DioRandom _random = new DioRandom();
		public uint seed = 6;

		public int nextDice(int faces)
		{
			return (int)(next() * faces) + 1;
		}
		public float next(float min = 0, float max = 1f)
		{
			this.seed = (uint)(this.seed * 9301 + 49297) % 233280;
			float rnd = seed / 233280f;
			return min + rnd * (max - min);
		}
		public static float random(float min = 0, float max = 1f)
		{
			return _random.next(min, max);
		}
		public static void setRandomSeed(int seed)
		{
			_random.seed = (uint)seed;
		}
		//Math.seed = 6;Math.seededRandom = function(max, min) { max = max || 1; min = min || 0; Math.seed = (Math.seed * 9301 + 49297) % 233280; var rnd = Math.seed / 233280.0; return min + rnd * (max - min); };
	}
}
