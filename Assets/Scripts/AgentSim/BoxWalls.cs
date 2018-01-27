using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BoxWalls : MonoBehaviour
    {
        Vector3 size = 100f * Vector3.one;
        float wallWidth = 100f;
        Transform[] walls;

        public void Init (Vector3 _size, float _wallWidth)
        {
            size = _size;
            wallWidth = _wallWidth;
            CreateWalls();
        }

        void CreateWalls ()
        {
            walls = new Transform[6];
            for (int i = 0; i < 6; i++)
            {
                walls[i] = GameObject.CreatePrimitive( PrimitiveType.Cube ).transform;
                walls[i].GetComponent<MeshRenderer>().enabled = false;
                walls[i].parent = transform;
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