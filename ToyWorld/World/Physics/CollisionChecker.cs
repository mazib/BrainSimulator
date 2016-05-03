﻿using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;
using World.GameActors.GameObjects;
using World.ToyWorldCore;

namespace World.Physics
{
    public interface ICollisionChecker
    {
        /// <summary>
        /// Checks whether given PhysicalEntity collides with any other PhysicalEntity.
        /// </summary>
        /// <param name="physicalEntity"></param>
        /// <returns>List of PhysicalEntities given entity collides with.</returns>
        List<List<IPhysicalEntity>> CollisionGroups();

        /// <summary>
        /// Checks whether given PhysicalEntity collides with any Tile.
        /// </summary>
        /// <param name="physicalEntity"></param>
        /// <returns></returns>
        bool CollidesWithTile(IPhysicalEntity physicalEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicalEntity"></param>
        /// <returns></returns>
        bool CollidesWithTile(IPhysicalEntity physicalEntity, float eps);

        /// <summary>
        /// Maximum size of an object. It is used in collisions check between PhysicalEntities.
        /// </summary>
        float MaximumGameObjectRadius { get; }

        /// <summary>
        /// Maximum speed of object. It is used in collisions check between PhysicalEntities.
        /// </summary>
        float MaximumGameObjectSpeed { get; }

        /// <summary>
        /// Check whether this physical entities collide with each other or tile.
        /// </summary>
        /// <param name="physicalEntities"></param>
        /// <returns></returns>
        bool Collides(List<IPhysicalEntity> physicalEntities);

        /// <summary>
        /// Check whether this physical entity collides with another PE or tile.
        /// </summary>
        /// <param name="physicalEntity"></param>
        /// <returns></returns>
        bool Collides(IPhysicalEntity physicalEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicalEntity"></param>
        /// <returns></returns>
        bool CollidesWithPhysicalEntity(IPhysicalEntity physicalEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicalEntities"></param>
        /// <returns></returns>
        bool CollidesWithEachOther(List<IPhysicalEntity> physicalEntities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <param name="physicalEntities"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        List<IPhysicalEntity> CollisionThreat(IPhysicalEntity targetEntity, List<IPhysicalEntity> physicalEntities,
            float eps = 0);
    }

    class CollisionChecker : ICollisionChecker
    {
        private readonly List<IPhysicalEntity> m_physicalEntities;
        private readonly IAtlas m_atlas;
        private readonly IObjectLayer m_objectLayer;

        private const float MAXIMUM_GAMEOBJECT_RADIUS = 5f;
        public float MaximumGameObjectRadius { get { return MAXIMUM_GAMEOBJECT_RADIUS; } }

        private const float MAXIMUM_OBJECT_SPEED = 1f;
        public float MaximumGameObjectSpeed { get { return MAXIMUM_OBJECT_SPEED; }}


        public CollisionChecker(IAtlas atlas)
        {
            m_atlas = atlas;
            m_objectLayer = atlas.GetLayer(LayerType.Object) as SimpleObjectLayer;
            if (m_objectLayer != null)
            {
                m_physicalEntities = m_objectLayer.GetPhysicalEntities();
            }
            else
            {
                throw new ArgumentException("ObjectLayer not found.");
            }
        }


        /// <summary>
        /// Search for all the objects that are or can be in collision with each other.
        /// </summary>
        /// <returns>List of </returns>
        public List<List<IPhysicalEntity>> CollisionGroups()
        {

            // TODO : groups (optimization)
/*            List<HashSet<IPhysicalEntity>> listOfSets = new List<HashSet<IPhysicalEntity>>();

            foreach (IPhysicalEntity physicalEntity in m_physicalEntities)
            {
                if (Collides(physicalEntity))
                {
                    if (Collides(physicalEntity))
                    {
                        var circle = new VRageMath.Circle(physicalEntity.Position, 2 * MaximumGameObjectRadius + MaximumGameObjectSpeed);
                        var physicalEntities = new HashSet<IPhysicalEntity>(m_objectLayer.GetPhysicalEntities(circle));
                        listOfSets.Add(physicalEntities);
                    }
                }
            }

            // consolidation
            for (int i = 0; i < listOfSets.Count - 1; i++)
            {
                for (int j = i + 1; j < listOfSets.Count; j++)
                {
                    
                }
            }*/

            List<List<IPhysicalEntity>> l = new List<List<IPhysicalEntity>>();
            l.Add(m_physicalEntities);

            return l;
        }

        public bool Collides(List<IPhysicalEntity> collisionGroup)
        {
            return collisionGroup.Any(CollidesWithTile) || CollidesWithEachOther(collisionGroup);
        }
        
        public bool Collides(IPhysicalEntity physicalEntity)
        {
            return CollidesWithTile(physicalEntity) || CollidesWithPhysicalEntity(physicalEntity);
        }

        public bool CollidesWithTile(IPhysicalEntity physicalEntity){
            List<Vector2I> coverTilesCoordinates = physicalEntity.CoverTiles();
            bool colliding = !coverTilesCoordinates.TrueForAll(x => !m_atlas.ContainsCollidingTile(x));
            return colliding;
        }

        
        public bool CollidesWithTile(IPhysicalEntity physicalEntity, float eps)
        {
            physicalEntity.Shape.Resize(eps);
            bool collides = CollidesWithTile(physicalEntity);
            physicalEntity.Shape.Resize(-eps);
            return collides;
        }

        public bool CollidesWithPhysicalEntity(IPhysicalEntity physicalEntity)
        {
            var circle = new VRageMath.Circle(physicalEntity.Position, 2 * MaximumGameObjectRadius);
            List<IPhysicalEntity> physicalEntities = m_objectLayer.GetPhysicalEntities(circle);
            return physicalEntities.Any(physicalEntity.CollidesWith);
        }


        public bool CollidesWithEachOther(List<IPhysicalEntity> physicalEntities)
        {
            for (int i = 0; i < physicalEntities.Count - 1; i++)
            {
                for (int j = i + 1; j < physicalEntities.Count; j++)
                {
                    if (physicalEntities[i].CollidesWith(physicalEntities[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<IPhysicalEntity> CollisionThreat(IPhysicalEntity targetEntity, List<IPhysicalEntity> physicalEntities, float eps = 0)
        {
            var list = new HashSet<IPhysicalEntity>();

            foreach (IPhysicalEntity physicalEntity in physicalEntities)
            {
                if (targetEntity == physicalEntity)
                {
                    continue;
                }
                if (targetEntity.CollidesWith(physicalEntity, eps))
                {
                    list.Add(physicalEntity);
                }
            }

            return list.ToList();
        }
    }
}
