using Fletch.Physics.Abstractions.Backends;
using Fletch.Physics.Components;
using Fletch.Physics.Model.Forces;
using System.Numerics;

namespace Fletch.Physics.Managers
{
    internal class PhysicsForcesQueue
    {
        private readonly Queue<ForceCommand> commandList = new Queue<ForceCommand>();

        private readonly IPhysicsBackend physicsBackend;

        public PhysicsForcesQueue(IPhysicsBackend physicsBackend)
        {
            this.physicsBackend = physicsBackend;
        }

        public void AddCommand(RigidBody body, ForceType forceType, Vector2? vectorForce = null, float angularForce = 0f, Vector2? point = null)
        {
            if (body.BodyHandle == null)
                return;

            ForceCommand forceCommand = new ForceCommand(body.BodyHandle, forceType, vectorForce, angularForce, point);

            commandList.Enqueue(forceCommand);
        }

        public void ApplyPendingForces()
        {
            int commandListCount = commandList.Count;

            for (int i = 0; i < commandListCount; i++)
            {
                ForceCommand command = commandList.Dequeue();

                switch (command.ForceType)
                {
                    case ForceType.Force:
                        if (command.VectorForce == null)
                            continue;

                        physicsBackend.ApplyForce(command.BodyHandle, command.VectorForce.Value);
                        break;

                    case ForceType.ForceAtPoint:
                        if (command.VectorForce == null || command.Point == null)
                            continue;

                        physicsBackend.ApplyForceAt(command.BodyHandle, command.VectorForce.Value, command.Point.Value);
                        break;

                    case ForceType.Impulse:
                        if (command.VectorForce == null)
                            continue;

                        physicsBackend.ApplyImpulse(command.BodyHandle, command.VectorForce.Value);
                        break;

                    case ForceType.ImpulseAtPoint:
                        if (command.VectorForce == null || command.Point == null)
                            continue;

                        physicsBackend.ApplyImpulseAt(command.BodyHandle, command.VectorForce.Value, command.Point.Value);
                        break;

                    case ForceType.Torque:
                        physicsBackend.ApplyTorque(command.BodyHandle, command.AngularForce);
                        break;

                    case ForceType.AngularImpulse:
                        physicsBackend.ApplyAngularImpulse(command.BodyHandle, command.AngularForce);
                        break;

                    default:
                        throw new Exception();
                }
            }
        }
    }
}
