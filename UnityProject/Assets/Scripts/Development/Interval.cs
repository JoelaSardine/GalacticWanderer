using UnityEngine;
using System.Collections;

namespace SardineTools
{
	public struct Interval
	{
		public enum ModifyLimit { Clamp, Push, Invert, Cancel }

		#region Variables
		
		private float _min;
		private float _max;

		#endregion Variables

		#region Constructors

		/// <summary>Creates a new Interval [a, b] (or [b, a] if b < a)</summary>
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

		/// <summary>[0;1]</summary>
		public static Interval one { get { return new Interval(0, 1); } }
		/// <summary>[0;0]</summary>
		public static Interval zero { get { return new Interval(0, 0); } }

		#endregion Static Parameters and Methods

		#region Parameters and Methods

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

		/// <summary>Modify the lowest limit of the Interval. Returns true if 'newMin' > 'max' (see 'mode' parameter).</summary>
		/// <param name="mode">SetMin(2) in [0;1] => Clamp [1;1] ; Push [2;2] ; Invert [1;2] ; Cancel [0;1]</param>
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
				default:// ModifyLimit.Cancel:
					break;
				}
				return true;
			}
		}
		/// <summary>Modify the highest limit of the Interval. Returns true if 'newMax' < 'min' (see 'mode' parameter).</summary>
		/// <param name="mode">SetMax(-1) in [0;1] => Clamp [0;0] ; Push [-1;-1] ; Invert [-1;0] ; Cancel [0;1]</param>
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
				default:// ModifyLimit.Cancel:
					break;
					
				}
				return true;
			}
		}

		/// <summary>Shift the Interval by specified offset.</summary>
		public void Shift(float offset)
		{
			_min += offset;
			_max += offset;
		}
		/// <summary>Shift the Interval so the new 'min' is the specified one.</summary>
		public void ShiftToMin(float newMin) { Shift(newMin - min); }
		/// <summary>Shift the Interval so the new 'max' is the specified one.</summary>
		public void ShiftToMax(float newMax) { Shift(newMax - max); }
		/// <summary>Shift the Interval so the new 'center' is the specified one.</summary>
		public void ShiftToCenter(float newCenter) { Shift(newCenter - center); }

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

		/// <summary>Linearly interpolates between 'min' and 'max' by 't'.</summary>
		public float Lerp(float t)
		{
			return Mathf.Lerp(min, max, t);
		}

		/// <summary>Is 'f' contained in Interval ?</summary>
		public bool Contains(float f) { return f >= min && f <= max; }
		/// <summary>Is 'v' totally contained in Interval ?</summary>
		public bool Contains(Interval v) { return v.min >= min && v.max <= max; }

		/// <summary>Enlarge the Interval to the specified value. Return true if 'length' have changed.</summary>
		public bool ExtendsTo(float f)
		{
			if (f > max) _max = f;
			else if (f < min) _min = f;
			else return false;
			return true;
		}
		/// <summary>Enlarge the Interval to contain the specified one. Return true if 'length' have changed.</summary>
		public bool ExtendsTo(Interval v)
		{
			return ExtendsTo(v.min) | ExtendsTo(v.max);
		}
		
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

		public static bool operator ==(Interval u, Interval v)
		{
			return u.Equals(v);
		}
		public static bool operator !=(Interval u, Interval v)
		{
			return !u.Equals(v);
		}

		public static explicit operator Vector2(Interval u)
		{
			return new Vector2(u.min, u.max);
		}

		#endregion
	}
}
