﻿using GoodAI.ToyWorld.Control;
using VRageMath;
using World.Physics;

namespace World.GameActors.GameObjects
{
    public interface IAvatar : IAvatarControllable, ICharacter
    {
        int Id { get; }
        IUsable Tool { get; set; }
        new IForwardMovablePhysicalEntity PhysicalEntity { get; set; }
    }

    public class Avatar : Character, IAvatar
    {
        public int Id { get; private set; }
        public IUsable Tool { get; set; }

        public float DesiredSpeed { get; set; }
        public float DesiredRotation { get; set; }
        public bool Interact { get; set; }
        public bool Use { get; set; }
        public bool PickUp { get; set; }

        public Avatar(string name, int id, Vector2 initialPosition, Vector2 size, float direction = 0)
        {
            Name = name;
            Id = id;
            PhysicalEntity = new ForwardMovablePhysicalEntity(initialPosition, size, direction);
        }

        public void ResetControls()
        {
            DesiredSpeed = 0f;
            DesiredRotation = 0f;
            Interact = false;
            Use = false;
            PickUp = false;
        }
    }
}
