using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class helper
{
	public static float random(float minInclusive, float maxInclusive) => UnityEngine.Random.Range(minInclusive, maxInclusive);
	public static int random(int minInclusive, int maxExclusive) => UnityEngine.Random.Range(minInclusive, maxExclusive);
	public static float random01() => random(0f, 1f);
	public static float random(float maxInclusive) => random(0, maxInclusive);
	public static int random(int maxExclusive) => random(0, maxExclusive);
	public static int randomRange(int start, int count) => random(start, start + count);

	public static bool isNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
	public static bool isNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

	public static bool invalid(this ICollection collection) => collection == null || collection.Count == 0;
	public static bool invalid(this ICollection collection, int index) => collection.invalid() || index < 0 || index >= collection.Count;
	public static bool valid(this ICollection collection) => !collection.invalid();
	public static bool valid(this ICollection collection, int index) => !collection.invalid(index);

	public static void forEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			action(item);
		}
	}

	public static void forEach<T>(this IList<T> source, Action<int, T> action)
	{
		for (var i = 0; i < source.Count; ++i)
		{
			action(i, source[i]);
		}
	}

	public static int randomIndex(this IList list) => list.valid() ? random(list.Count) : 0;

	public static T randomOrDefault<T>(this IList<T> list)
    {
        try
        {
            return list[random(list.Count)];
        }
        catch
        {
            return default(T);
        }
    }

	public static List<T> toShuffledList<T>(this List<T> list)
	{
		for (var i = list.Count - 1; i > 0; --i)
		{
			var r = random(i);
			(list[r], list[i]) = (list[i], list[r]);
		}

		return list;
	}

	public static List<T> toShuffledList<T>(this IEnumerable<T> source) => toShuffledList(source.ToList());

	public static Vector3 changeTo(this Vector3 v, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
	public static void set(this Vector3 v, float? x = null, float? y = null, float? z = null) => v = new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);

	public static Vector2 scale(this Vector2 v, Vector2 s) => new Vector2(v.x * s.x, v.y * s.y);
	public static Vector3 scale(this Vector3 v, Vector3 s) => new Vector3(v.x * s.x, v.y * s.y, v.z * s.z);

	public static float lerp(this float from, float to, float t) => Mathf.Lerp(from, to, t);
	public static Vector3 lerp(this Vector3 from, Vector3 to, float t) => Vector3.Lerp(from, to, t);
	public static Quaternion slerp(this Quaternion from, Quaternion to, float t) => Quaternion.Slerp(from, to, t);

	public static bool isZero(this float f) => Mathf.Abs(f) < float.Epsilon;

	public static void setPosition(this Transform t, float? x = null, float? y = null, float? z = null) => t.position = t.position.changeTo(x, y, z);
	public static void setLocalPosition(this Transform t, float? x = null, float? y = null, float? z = null) => t.localPosition = t.localPosition.changeTo(x, y, z);

	public static void lerpPosition(this Transform tf, Vector3 to, float t) => tf.position = tf.position.lerp(to, t);
	public static void lerpLocalPosition(this Transform tf, Vector3 to, float t) => tf.localPosition = tf.localPosition.lerp(to, t);

	public static void slerpRotation(this Transform tf, Quaternion to, float t) => tf.rotation = tf.rotation.slerp(to, t);
	public static void slerpLocalRotation(this Transform tf, Quaternion to, float t) => tf.localRotation = tf.localRotation.slerp(to, t);

	public static Vector3 inverse(this Vector3 v) => -v;
	public static Vector3 to(this Vector3 from, Vector3 to) => to + from.inverse();

	public static Quaternion inverse(this Quaternion q) => Quaternion.Inverse(q);
	public static Quaternion to(this Quaternion from, Quaternion to) => to * from.inverse();

	public static (Vector3 p, Quaternion r) offsetFrom(this Transform tf, Transform target)
	{
		return (target.position.to(tf.position), target.rotation.to(tf.rotation));
	}
	public static void applyOffset(this Transform tf, (Vector3 p, Quaternion r) offset)
	{
		tf.position = offset.p + tf.position;
		tf.rotation = offset.r * tf.rotation;
	}

	public static void copyFrom(this Transform dst, Transform src)
    {
        dst.SetPositionAndRotation(src.position, src.rotation);
    }

	public static void swap(this Transform t1, Transform t2)
    {
		(t1.position, t2.position) = (t2.position, t1.position);
		(t1.rotation, t2.rotation) = (t2.rotation, t1.rotation);
    }

	public static bool includes(this LayerMask layer, int target)
	{
		return (layer.value & 1 << target) > 0;
	}
}
