using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ShellSpawner : MonoBehaviour 
	{
		public bool isDynamic = false;
		public GameObject prefab;
		public int number = 10;
		public Vector2 shellRadius = new Vector2( 500f, 1000f );
		public float crowding = 0.75f;
		public Vector2 scaleRange = new Vector2( 500f, 1000f );

//		AmbientSprite[] sprites;
//
//		void Start () 
//		{
//			SpawnAll();
//
//			if (isDynamic)
//			{
//				sprites = GetComponentsInChildren<AmbientSprite>();
//				foreach (AmbientSprite sprite in sprites)
//				{
//					sprite.radius = shellRadius.y;
//				}
//			}
//		}

		void SpawnAll ()
		{
			List<Vector3> positions = GetPointsOnSphere( Mathf.Max( number, Mathf.RoundToInt( number / Mathf.Clamp( crowding, 0.1f, 1f ) ) ) );
			for (int i = 0; i < number; i++)
			{
				int index = Random.Range( 0, positions.Count );
				SpawnObject( positions[index] );
				positions.RemoveAt( index );
			}
		}

		void SpawnObject (Vector3 position)
		{
			GameObject obj = Instantiate( prefab, transform );
			obj.transform.localPosition = position;
			obj.transform.localScale = Random.Range( scaleRange.x, scaleRange.y ) * Vector3.one;
		}

		List<Vector3> GetPointsOnSphere (int n)
		{
			List<Vector3> points = new List<Vector3>();
			float inc = Mathf.PI * (3f - Mathf.Sqrt( 5f ));
			float off = 2f / n;
			float y = 0, r = 0, phi = 0;

			for (var i = 0; i < n; i++)
			{
				y = i * off - 1f + (off / 2f);
				r = Mathf.Sqrt( 1f - y * y );
				phi = i * inc;

				points.Add( Random.Range( shellRadius.x, shellRadius.y ) * new Vector3( r * Mathf.Cos( phi ), y, r * Mathf.Sin( phi ) ) );
			}
			return points;
		}

//		void Update ()
//		{
//			if (isDynamic)
//			{
//				foreach (AmbientSprite sprite in sprites)
//				{
//					sprite.DoUpdate();
//				}
//			}
//		}
	}
}