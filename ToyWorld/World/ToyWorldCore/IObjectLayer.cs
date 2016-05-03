﻿using System.Collections.Generic;
using World.GameActors.GameObjects;
using World.Physics;
using Circle = VRageMath.Circle;

namespace World.ToyWorldCore
{
    public interface IObjectLayer : ILayer<GameObject>
    {
        /// <summary>
        /// Returns objects that have intersection with giver rectangle.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        List<IGameObject> GetGameObjects(VRageMath.RectangleF rectangle);

        /// <summary>
        /// Gets all game objects.
        /// </summary>
        /// <returns></returns>
        List<IGameObject> GetGameObjects();

        /// <summary>
        /// Get all game objects in given circle.
        /// </summary>
        /// <returns></returns>
        List<IGameObject> GetGameObjects(Circle circle);

        List<IPhysicalEntity> GetPhysicalEntities(Circle circle);

        List<IPhysicalEntity> GetPhysicalEntities();

        List<IPhysicalEntity> GetPhysicalEntities(VRageMath.RectangleF rectangle);

        /// <summary>
        /// Adds game object to the layer. If object cannot be added, return false.
        /// </summary>
        /// <param name="gameObject">GameObject to add</param>
        /// <returns>True if object was successfully added.</returns>
        bool AddGameObject(GameObject gameObject);
    }
}
