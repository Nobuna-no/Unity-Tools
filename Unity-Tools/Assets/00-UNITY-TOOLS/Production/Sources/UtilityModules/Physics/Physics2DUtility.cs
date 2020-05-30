using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Physics2DUtility
{
    public static class CollisionMatrixLayerMasks
    {
        private static Dictionary<int, int> masksByLayer;

        public static void Init()
        {
            masksByLayer = new Dictionary<int, int>();
            for (int i = 0; i < 32; i++)
            {
                int mask = 0;
                for (int j = 0; j < 32; j++)
                {
                    if (!Physics2D.GetIgnoreLayerCollision(i, j))
                    {
                        mask |= 1 << j;
                    }
                }
                masksByLayer.Add(i, mask);
                //Debug.Log("PhysicsCollisionMatrixLayerMasks Init["+ LayerMask.LayerToName(i) +"]" + string.Format("#{0:X}", mask));
            }

        }

        public static int MaskForLayer(int layer)
        {
            if (masksByLayer == null)
            {
                Init();
            }

            return masksByLayer[layer];
        }
    }


    /// <summary>
    /// Compute an horizontal raycast to try to detect a ground.
    /// </summary>
    /// <param name="origin">Origin of the boundaries.</param>
    /// <param name="refCollider">Reference collider for boundary calculations.</param>
    /// <param name="layerMask">Physics layer to detect.</param>
    /// <param name="drawDebug">May draw gizmos?</param>
    /// <param name="debugDuration">Duration of the drawn debug.</param>
    /// <returns></returns>
    public static Collider2D ComputeHorizontalRaycast(Vector2 origin, Collider2D refCollider, LayerMask layerMask, bool drawDebug = false, float debugDuration = 0f)
    {
        return ComputeHorizontalRaycast(origin, refCollider, layerMask, drawDebug, debugDuration, Color.red);
    }
    public static Collider2D ComputeHorizontalRaycast(Vector2 origin, Collider2D refCollider, LayerMask layerMask, bool DrawDebug, float debugDuration, Color debugColor)
    {
        float ratioX = 0.3f;
        origin.y -= refCollider.bounds.size.y * 0.55f - refCollider.offset.y;
        origin.x -= refCollider.bounds.size.x * ratioX;
        float distance = refCollider.bounds.size.x * ratioX * 2;

        //int layer = CollisionMatrixLayerMasks.MaskForLayer(objectLayer);
        if (DrawDebug)
        {
            //Debug.Log("[LOG]<Physics2DUtility>: PhysicsCollisionMatrixLayerMasks[" + LayerMask.LayerToName(objectLayer) + "]: " + string.Format("#{0:X}", layer));
            Debug.DrawRay(origin, Vector3.right * distance, debugColor, debugDuration);
        }

        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, Vector2.right, distance, layerMask);

        for (int i = 0; i < hitInfos.Length; ++i)
        {
            Collider2D current = hitInfos[i].collider;
            if (current && !current.isTrigger)
            {
                return current;
            }
        }
        return null;
    }

    /// <summary>
    /// Compute an horizontal raycast to try to detect a wall.
    /// </summary>
    /// <param name="origin">Origin of the boundaries.</param>
    /// <param name="refCollider">Reference collider for boundary calculations. (i.e. the player collider)</param>
    /// <param name="forward">The reference to the front of the place where raycast.</param>
    /// <param name="layer">Physics layer to detect.</param>
    /// <param name="drawDebug">May draw gizmos?</param>
    /// <param name="debugDuration">Duration of the drawn debug.</param>
    /// <returns></returns>
    public static Collider2D ComputeVerticalRay(Vector2 origin, Collider2D refCollider, float forward, LayerMask layer, bool drawDebug = false, float debugDuration = 0f)
    {
        return ComputeVerticalRay(origin, refCollider, forward, layer, drawDebug, debugDuration, Color.red);
    }
    public static Collider2D ComputeVerticalRay(Vector2 origin, Collider2D refCollider, float forward, LayerMask layer, bool DrawDebug, float debugDuration, Color debugColor)
    {
        float distance = refCollider.bounds.size.y * 0.75f;
        origin.x += forward * refCollider.bounds.size.x * 0.55f + (forward * 0.1f);
        origin.y += refCollider.bounds.size.y * 0.4f + refCollider.offset.y;
        
        if (DrawDebug)
        {
            //Debug.Log("[LOG]<Physics2DUtility>: PhysicsCollisionMatrixLayerMasks[" + LayerMask.LayerToName(objectLayer) + "]: " + string.Format("#{0:X}", layer));
            Debug.DrawRay(origin, Vector3.down * distance, debugColor, debugDuration);
        }

        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, Vector2.down, distance, layer);

        for (int i = 0; i < hitInfos.Length; ++i)
        {
            Collider2D current = hitInfos[i].collider;
            if (current && !current.isTrigger)
            {
                return current;
            }
        }
        return null;
    }


    /// <summary>
    /// Compute an horizontal raycast to try to detect a ground. This version allow the use of @objectLayer to work with the physics layer of the object as mask of the ray.
    /// </summary>
    /// <param name="origin">Origin of the boundaries.</param>
    /// <param name="refCollider">Reference collider for boundary calculations.</param>
    /// <param name="objectLayer">The object layer (gameObject.layer).</param>
    /// <param name="drawDebug">May draw gizmos?</param>
    /// <param name="debugDuration">Duration of the drawn debug.</param>
    /// <returns></returns>
    public static Collider2D ComputeHorizontalRaycast(Vector2 origin, Collider2D refCollider, int objectLayer, bool drawDebug = false, float debugDuration = 0f)
    {
        return ComputeHorizontalRaycast(origin, refCollider, objectLayer, drawDebug, debugDuration, Color.red);
    }
    public static Collider2D ComputeHorizontalRaycast(Vector2 origin, Collider2D refCollider, int objectLayer, bool drawDebug, float debugDuration, Color color)
    {
        origin.y -= refCollider.bounds.size.y * 0.55f - refCollider.offset.y;
        origin.x -= refCollider.bounds.size.x * 0.3f;
        float distance = refCollider.bounds.size.x * 0.8f;

        int layer = CollisionMatrixLayerMasks.MaskForLayer(objectLayer);
        if (drawDebug)
        {
            Debug.DrawRay(origin, Vector3.right * distance, color, debugDuration);
        }

        RaycastHit2D hitInfos = Physics2DUtility.RaycastAllWithEffector(origin, Vector2.right, distance, layer, drawDebug);

        return hitInfos ? hitInfos.collider : null;
    }

    public static Collider2D ComputeVerticalRayWithoutEffector(Vector2 origin, Collider2D refCollider, float forward, int objectLayer, bool drawDebug = false, float debugDuration = 0f)
    {
        float distance = refCollider.bounds.size.y * 0.75f;
        origin.x += forward * refCollider.bounds.size.x * 0.55f + (forward * 0.1f);
        origin.y += refCollider.bounds.size.y * 0.4f + refCollider.offset.y;

        int layer = CollisionMatrixLayerMasks.MaskForLayer(objectLayer);
        if(drawDebug)
        {
            Debug.DrawRay(origin, Vector3.down * distance, Color.red, debugDuration);
        }

        RaycastHit2D hitInfos = Physics2DUtility.RaycastAllWithoutEffector(origin, Vector2.down, distance, layer, drawDebug);

        return hitInfos ? hitInfos.collider : null;
    }
    public static Collider2D ComputeVerticalRayWithEffector(Vector2 origin, Collider2D refCollider, float forward, int objectLayer, bool drawDebug = false, float debugDuration = 0f)
    {
        float distance = refCollider.bounds.size.y * 0.75f;
        origin.x += forward * refCollider.bounds.size.x * 0.55f + (forward * 0.1f);
        origin.y += refCollider.bounds.size.y * 0.4f + refCollider.offset.y;

        int layer = CollisionMatrixLayerMasks.MaskForLayer(objectLayer);
        if (drawDebug)
        {
            Debug.DrawRay(origin, Vector3.down * distance, Color.red, debugDuration);
        }

        RaycastHit2D hitInfos = Physics2DUtility.RaycastAllWithEffector(origin, Vector2.down, distance, layer, drawDebug);

        return hitInfos ? hitInfos.collider : null;
    }


    /// <summary>
    /// Compute a raycast all and return the first collider non trigger found. Return an empty RaycastHit2D if nothing found.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static RaycastHit2D RaycastAll(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug)
    {
        return RaycastAll(origin, direction, distance, layerMask, debug, Color.magenta, 5f);
    }

    public static RaycastHit2D RaycastAll(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug, Color color, float duration = 0f)
    {
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        if (debug)
        {
            Debug.DrawRay(origin, direction * distance, color, duration);
        }

        for (int i = 0; i < hitInfos.Length; ++i)
        {
            Collider2D current = hitInfos[i].collider;
            if (current && !current.isTrigger )
            {
                return hitInfos[i];
            }
        }

        return new RaycastHit2D();
    }


    public static RaycastHit2D RaycastAllWithEffector(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug)
    {
        return RaycastAllWithEffector(origin, direction, distance, layerMask, debug, Color.magenta, 5f);
    }
    public static RaycastHit2D RaycastAllWithEffector(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug, Color color, float duration = 0f)
    {
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        if (debug)
        {
            Debug.DrawRay(origin, direction * distance, color, duration);
        }

        for (int i = 0; i < hitInfos.Length; ++i)
        {
            Collider2D current = hitInfos[i].collider;
            if (current && !current.isTrigger)
            {
                if (current.usedByEffector && direction.y >= 0)
                {
                    break;
                }
                 return hitInfos[i];
            }
        }

        return new RaycastHit2D();
    }

    public static RaycastHit2D RaycastAllWithoutEffector(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug)
    {
        return RaycastAllWithoutEffector(origin, direction, distance, layerMask, debug, Color.magenta, 5f);
    }
    public static RaycastHit2D RaycastAllWithoutEffector(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, bool debug, Color color, float duration = 0f)
    {
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        if (debug)
        {
            Debug.DrawRay(origin, direction * distance, color, duration);
        }

        for (int i = 0; i < hitInfos.Length; ++i)
        {
            Collider2D current = hitInfos[i].collider;
            if (current && !current.isTrigger && !current.usedByEffector)
            {
                return hitInfos[i];
            }
        }

        return new RaycastHit2D();
    }
}
