﻿using Ether.Network.Packets;
using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;
using System.Linq;
using Rhisis.Core.Data;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendToVisible(INetPacketStream packet, IEntity player)
        {
            IEnumerable<IPlayerEntity> visiblePlayers = from x in player.Object.Entities
                                                        where x.Type == WorldEntityType.Player
                                                        select x as IPlayerEntity;

            foreach (IPlayerEntity visiblePlayer in visiblePlayers)
                visiblePlayer.Connection.Send(packet);
        }

        public static void SendDestinationPosition(IMovableEntity movableEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(movableEntity.Id, SnapshotType.DESTPOS);
                packet.Write(movableEntity.MovableComponent.DestinationPosition.X);
                packet.Write(movableEntity.MovableComponent.DestinationPosition.Y);
                packet.Write(movableEntity.MovableComponent.DestinationPosition.Z);
                packet.Write<byte>(1);

                SendToVisible(packet, movableEntity);
            }
        }

        public static void SendFollowTarget(IEntity entity, IEntity targetEntity, float distance)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERSETDESTOBJ);
                packet.Write(targetEntity.Id);
                packet.Write(distance);

                SendToVisible(packet, entity);
            }
        }

        public static void SendDefinedText(IPlayerEntity entity, int textId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.DEFINEDTEXT);
                packet.Write(textId);
                packet.Write(0);

                entity.Connection.Send(packet);
            }
        }

        public static void SendDefinedText(IPlayerEntity entity, DefineText text) => SendDefinedText(entity, (int)text);

        public static void SendUpdateAttributes(IPlayerEntity entity, DefineAttributes attribute, int newValue)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOINTPARAM);
                packet.Write((int)attribute);
                packet.Write(newValue);

                entity.Connection.Send(packet);
                SendToVisible(packet, entity);
            }
        }
    }
}
