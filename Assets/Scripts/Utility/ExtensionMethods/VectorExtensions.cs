using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace ExtensionMethods {

	public static class VectorExtensions {
		public static Vector2 SetX(this Vector2 vec, float new_x) {
			vec.Set(new_x, vec.y);
			return vec;
		}
		public static Vector2 SetY(this Vector2 vec, float new_y) {
			vec.Set(vec.x, new_y);
			return vec;
		}

		public static Vector3 SetX(this Vector3 vec, float new_x) {
			vec.Set(new_x, vec.y, vec.z);
			return vec;
		}
		public static Vector3 SetY(this Vector3 vec, float new_y) {
			vec.Set(vec.x, new_y, vec.z);
			return vec;
		}
		public static Vector3 SetZ(this Vector3 vec, float new_z) {
			vec.Set(vec.x, vec.y, new_z);
			return vec;
		}

		public static Vector2 xy(this Vector2 vec) {
			return new Vector2(vec.x, vec.y);
		}
		public static Vector2 yx(this Vector2 vec) {
			return new Vector2 (vec.y, vec.x);
		}

		public static Vector2 xy(this Vector3 vec) {
			return new Vector2 (vec.x, vec.y);
		}
		public static Vector2 xz(this Vector3 vec) {
			return new Vector2 (vec.x, vec.z);
		}
		public static Vector2 yz(this Vector3 vec) {
			return new Vector2 (vec.y, vec.z);
		}
		public static Vector2 yx(this Vector3 vec) {
			return new Vector2 (vec.y, vec.x);
		}
		public static Vector2 zx(this Vector3 vec) {
			return new Vector2 (vec.z, vec.x);
		}
		public static Vector2 zy(this Vector3 vec) {
			return new Vector2 (vec.z, vec.y);
		}

		public static Vector3 xyz(this Vector3 vec) {
			return new Vector3(vec.x, vec.y, vec.z);
		}
		public static Vector3 xzy(this Vector3 vec) {
			return new Vector3 (vec.x, vec.z, vec.y);
		}
		public static Vector3 yxz(this Vector3 vec) {
			return new Vector3 (vec.y, vec.x, vec.z);
		}
		public static Vector3 yzx(this Vector3 vec) {
			return new Vector3 (vec.y, vec.z, vec.x);
		}
		public static Vector3 zxy(this Vector3 vec) {
			return new Vector3 (vec.z, vec.x, vec.y);
		}
		public static Vector3 zyx(this Vector3 vec) {
			return new Vector3 (vec.z, vec.y, vec.x);
		}

		public static Vector3 xy_(this Vector2 vec, float _) {
			return new Vector3 (vec.x, vec.y, _);
		}
		public static Vector3 x_y(this Vector2 vec, float _) {
			return new Vector3 (vec.x, _, vec.y);
		}
		public static Vector3 yx_(this Vector2 vec, float _) {
			return new Vector3 (vec.y, vec.x, _);
		}
		public static Vector3 y_x(this Vector2 vec, float _) {
			return new Vector3 (vec.y, _, vec.x);
		}
		public static Vector3 _xy(this Vector2 vec, float _) {
			return new Vector3 (_, vec.x, vec.y);
		}
		public static Vector3 _yx(this Vector2 vec, float _) {
			return new Vector3 (_, vec.y, vec.x);
		}

		public static float ToDegrees(this Vector2 vec) {
			return Mathf.Atan2 (vec.y, vec.x) * Mathf.Rad2Deg;
		}
		public static float ToRadians(this Vector2 vec) {
			return Mathf.Atan2 (vec.y, vec.x);
		}

		public static Vector2 FromRadians(this float rad) {
			return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad));
		}
		public static Vector2 FromRadians(this float rad, float radius) {
			return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad)) * radius;
		}
		
		public static Vector2 FromDegrees(this float deg) {
			return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad));
		}
		public static Vector2 FromDegrees(this float deg, float radius) {
			return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad)) * radius;
		}

		public static Vector2 FromRadians(this int rad) {
			return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad));
		}
		public static Vector2 FromRadians(this int rad, float radius) {
			return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad)) * radius;
		}

		public static Vector2 FromDegrees(this int deg) {
			return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad));
		}
		public static Vector2 FromDegrees(this int deg, float radius) {
			return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad)) * radius;
		}

		public static float MaxValue(this Vector3 vector) {
			return Mathf.Max(vector.x, vector.y, vector.z);
		}
		public static float MaxValue(this Vector2 vector) {
			return Mathf.Max(vector.x, vector.y);
		}

		public static float MinValue(this Vector3 vector) {
			return Mathf.Min(vector.x, vector.y, vector.z);
		}
		public static float MinValue(this Vector2 vector) {
			return Mathf.Min(vector.x, vector.y);
		}

		public static Vector3 EulerToVector(this Vector3 euler) {
			return Quaternion.Euler(euler) * Vector3.forward;
		}

		public static Vector3 VectorToEuler(this Vector3 vector) {
			return Quaternion.LookRotation(vector).eulerAngles;
		}

		public static Vector3 FlipX(this Vector3 vector)
		{
			return new Vector3(-vector.x, vector.y, vector.z);
		}

		public static Vector2 FlipX(this Vector2 vector)
		{
			return new Vector2(-vector.x, vector.y);
		}

		public static Vector3 FlipY(this Vector3 vector)
		{
			return new Vector3(vector.x, -vector.y, vector.z);
		}

		public static Vector2 FlipY(this Vector2 vector)
		{
			return new Vector2(vector.x, -vector.y);
		}

		public static Vector3 FlipZ(this Vector3 vector)
		{
			return new Vector3(vector.x, vector.y, -vector.z);
		}

	}
}
