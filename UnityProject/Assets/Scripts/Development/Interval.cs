using UnityEngine;
using System.Collections;

namespace SardineTools
{
	[System.Serializable]
	public class Interval
	{
		public enum ModifyLimit { Clamp, Push, Invert, Shift, Cancel }

		#region Variables

		[SerializeField]
		private float _min;
		[SerializeField]
		private float _max;

		#endregion Variables

		#region Constructors

		/// <summary>Creates a new Interval [a, b] ; or [b, a] if b < a </summary>
		public Interval(float a, float b)
		{
			_min = Mathf.Min(a, b);
			_max = Mathf.Max(a, b);
		}
		public Interval(Vector2 vec)
		{
			_min = Mathf.Min(vec.x, vec.y);
			_max = Mathf.Max(vec.x, vec.y);
		}

		#endregion Constructors

		#region Static Parameters and Methods

		/// <summary>[0, 1]</summary>
		public static Interval one { get { return new Interval(0, 1); } }
		/// <summary>[-1, 1]</summary>
		public static Interval trigo { get { return new Interval(-1, 1); } }
		/// <summary>[0, 360]</summary>
		public static Interval celsius { get { return new Interval(0, 360); } }
		/// <summary>[0, 2PI]</summary>
		public static Interval radians { get { return new Interval(0, 2 * Mathf.PI); } }
		/// <summary>[0, 0]. I don't know why you would use this.</summary>
		public static Interval zero { get { return new Interval(0, 0); } }
		/// <summary>[-3.40E+38, 3.40E+38]. It's a very large Interval. Not sure it is that useful.</summary>
		public static Interval infinite { get { return new Interval(float.MinValue, float.MaxValue); } }

		#endregion Static Parameters and Methods

		#region Parameters and Methods

		/// <summary>Returns a string representing this Interval.</summary>
		public override string ToString() { return "[" + min + ", " + max + "]"; }

		/// <summary>The lowest limit of the Interval. Modifiy with SetMin() or ExtendsTo().</summary>
		public float min { get { return _min; } }
		/// <summary>The highest limit of the Interval. Modifiy with SetMax() or ExtendsTo().</summary>
		public float max { get { return _max; } }
		/// <summary>The lowest int of the Interval.</summary>
		public int minInt { get { return Mathf.CeilToInt(min); } }
		/// <summary>The highest limit of the Interval.</summary>
		public int maxInt { get { return Mathf.FloorToInt(max); } }
		
		/// <summary>The center of the Interval.</summary>
		public float center { get { return (min + max) / 2.0f; } }

		/// <summary>Modify the lowest limit of the Interval. Returns true if 'newMin' is greater than 'max' (see 'mode' parameter).</summary>
		/// <param name="mode">SetMin(2) in [0,1] => Clamp [1,1] ; Push [2,2] ; Invert [1,2] ; Shift [2,3] ; Cancel [0,1]</param>
		public bool SetMin(float newMin, ModifyLimit mode = ModifyLimit.Clamp)
		{
			if (newMin <= max)
			{
				_min = newMin;
				return false;
			}
			else
			{
				switch (mode)
				{
				case ModifyLimit.Clamp:
					_min = max;
					break;
				case ModifyLimit.Push:
					_min = newMin;
					_max = newMin;
					break;
				case ModifyLimit.Invert:
					_min = _max;
					_max = newMin;
					break;
				case ModifyLimit.Shift:
					Shift(newMin - min);
					break;
				default:// ModifyLimit.Cancel:
					break;
				}
				return true;
			}
		}
		/// <summary>Modify the highest limit of the Interval. Returns true if "newMax" is smaller than "min" (see 'mode' parameter).</summary>
		/// <param name="mode">SetMax(-1) in [0,1] => Clamp [0,0] ; Push [-1,-1] ; Invert [-1,0] ; Shift [-2,-1] ; Cancel [0,1]</param>
		public bool SetMax(float newMax, ModifyLimit mode = ModifyLimit.Clamp)
		{
			if (newMax >= min)
			{
				_max = newMax;
				return false;
			}
			else
			{
				switch (mode)
				{
				case ModifyLimit.Clamp:
					_max = min;
					break;
				case ModifyLimit.Push:
					_max = newMax;
					_min = newMax;
					break;
				case ModifyLimit.Invert:
					_max = _min;
					_min = newMax;
					break;
				case ModifyLimit.Shift:
					Shift(newMax - max);
					break;
				default:// ModifyLimit.Cancel:
					break;
				}
				return true;
			}
		}

		/// <summary>Shift the Interval by specified offset, keeping same length.</summary>
		public void Shift(float offset) { _min += offset; _max += offset; }
		/// <summary>Shift the Interval so the new 'center' is the specified one, keeping same length.</summary>
		public void ShiftToCenter(float newCenter) { Shift(newCenter - center); }

		/// <summary>Change min and max but keeps the same center value. Length have to be positive.</summary>
		public void SetLength(float newLength)
		{
			float difference = newLength - length;
			SetMin(Mathf.Min(min - (difference) / 2.0f, center));
			SetMax(max + (difference) / 2.0f, ModifyLimit.Clamp);
		}
		/// <summary>Length of the Interval.</summary>
		public float length { get { return max - min; } }
		/// <summary>Length of the int limits of the Interval. Can be different from Interval.length casted to int.</summary>
		public int lengthInt { get { return maxInt - minInt; } }

		/// <summary>Gets random float in the Interval.</summary>
		public float random { get { return Random.Range(min, max); } }
		/// <summary>Gets random int in the Interval (min is inclusive, max is exclusive).</summary>
		public float randomInt { get { return Random.Range(minInt, maxInt); } }
		/// <summary>Gets random int in the Interval (both limits are inclusive).</summary>
		public float randomIntInclusive { get { return Random.Range(minInt, maxInt + 1); } }

		public float Clamp(float f) { return Mathf.Clamp(f, min, max); }
		/// <summary>Clamp an int to fit in the Interval.</summary>
		public int ClampInt(float i) { return (int)Mathf.Clamp(i, minInt, maxInt); }
		/// <summary>Linearly interpolates between 'min' and 'max' by 't'.</summary>
		public float Lerp(float t) { return Mathf.Lerp(min, max, t); }
		/// <summary>Repeats the value of 'f' in the Interval. Repeat(12) in [0;10] is 2.</summary>
		public float Repeat(float f) { return min + Mathf.Repeat(f - min, length); }
		/// <summary>PingPongs the value of 'f' in the Interval. PingPong(12) in [0;10] is 8.</summary>
		public float PingPong(float f) { return min + Mathf.PingPong(f - min, length); }

		/// <summary>Is 'f' contained in Interval ?</summary>
		public bool Contains(float f) { return f >= min && f <= max; }
		/// <summary>Is 'u' totally contained in Interval ?</summary>
		public bool Contains(Interval u) { return u.min >= min && u.max <= max; }
		/// <summary>Is 'u' partially contained in Interval ?</summary>
		public bool ContainsPartially(Interval u) { return Contains(u.min) || Contains(u.max) || u.Contains(min) || u.Contains(max); }
		
		/// <summary>Enlarge the Interval to the specified value. Return true if 'length' have changed.</summary>
		public bool ExtendsTo(float f)
		{
			if (f > max) _max = f;
			else if (f < min) _min = f;
			else return false;
			return true;
		}
		/// <summary>Enlarge the Interval to contain the specified one. Return true if 'length' have changed.</summary>
		public bool ExtendsTo(Interval u) { return ExtendsTo(u.min) | ExtendsTo(u.max); }
		
		#endregion Parameters and Methods

		#region Operators

		/// <summary>Are the two Intervals identical ?</summary>
		public override bool Equals(object obj)
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;

			Interval u = (Interval)obj;
			return (min == u.min && max == u.max);
		}
		public override int GetHashCode()
		{
			unchecked {
				int hash = min.GetHashCode() * 23;
				hash += max.GetHashCode() * 17;
				if (min + max == 2) return hash;
				hash += (min + max).GetHashCode() * 31;
				return hash;
			}
		}

		public static bool operator ==(Interval u, Interval v) { return u.Equals(v); }
		public static bool operator !=(Interval u, Interval v) { return !u.Equals(v); }

		public static explicit operator Vector2(Interval u) { return new Vector2(u.min, u.max); }

		#endregion
	}
}
