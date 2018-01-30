using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Walls : MonoBehaviour
    {
        Vector3 size = 100f * Vector3.one;
        float wallWidth = 100f;

        Transform[] walls;

        public void Init (Vector3 _size, float _wallWidth, bool periodicBoundary)
        {
            size = _size;
            wallWidth = _wallWidth;
            CreateWalls( periodicBoundary );
        }

        void CreateWalls (bool periodicBoundary)
        {
            GameObject wallPrefab = Resources.Load( "Wall" ) as GameObject;
            if (wallPrefab == null)
            {
                Debug.LogWarning( "Wall prefab can't be found in Resources" );
                return;
            }

            walls = new Transform[6];
            for (int i = 0; i < 6; i++)
            {
                walls[i] = (Instantiate( wallPrefab, transform ) as GameObject).transform;
                if (periodicBoundary)
                {
                    walls[i].gameObject.AddComponent<PeriodicBoundary>();
                }
            }
            SetWalls();
        }

        void SetWalls ()
        {
            int w = 0;
            Vector3 extents = size + 2f * wallWidth * Vector3.one;
            Vector3 position, scale;
            for (int axis = 0; axis < 3; axis++)
            {
                position = Vector3.zero;
                position[axis] = (size[axis] + wallWidth) / 2f;

                scale = extents;
                scale[axis] = wallWidth;

                for (int i = 0; i < 2; i++)
                {
                    SetWall( w, i == 0 ? position : -position, scale );
                    w++;
                }
            }
        }

        void SetWall (int index, Vector3 position, Vector3 scale)
        {
            walls[index].localPosition = position;
            walls[index].localScale = scale;
            walls[index].name = "wall" + (index + 1);
        }
    }
}