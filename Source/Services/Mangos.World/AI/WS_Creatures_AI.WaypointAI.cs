﻿//
//  Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
//  
//  This program is free software. You can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation. either version 2 of the License, or
//  (at your option) any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY. Without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

using System;
using Mangos.Common.Enums.Global;
using Mangos.World.DataStores;
using Mangos.World.Objects;
using Microsoft.VisualBasic.CompilerServices;

namespace Mangos.World.AI
{
    public partial class WS_Creatures_AI
    {
        public class WaypointAI : DefaultAI
        {
            public int CurrentWaypoint;

            public WaypointAI(ref WS_Creatures.CreatureObject Creature)
                : base(ref Creature)
            {
                CurrentWaypoint = -1;
                IsWaypoint = true;
            }

            public override void Pause(int Time)
            {
                checked
                {
                    CurrentWaypoint--;
                    aiTimer = Time;
                }
            }

            public override void DoMove()
            {
                var distanceToSpawn = WorldServiceLocator._WS_Combat.GetDistance(aiCreature.positionX, aiCreature.SpawnX, aiCreature.positionY, aiCreature.SpawnY, aiCreature.positionZ, aiCreature.SpawnZ);
                checked
                {
                    if (aiTarget == null)
                    {
                        if (!WorldServiceLocator._WS_DBCDatabase.CreatureMovement.ContainsKey(aiCreature.WaypointID))
                        {
                            WorldServiceLocator._WorldServer.Log.WriteLine(LogType.CRITICAL, "Creature [{0:X}] is missing waypoints.", aiCreature.GUID - WorldServiceLocator._Global_Constants.GUID_UNIT);
                            aiCreature.ResetAI();
                            return;
                        }
                        try
                        {
                            CurrentWaypoint++;
                            if (!WorldServiceLocator._WS_DBCDatabase.CreatureMovement[aiCreature.WaypointID].ContainsKey(CurrentWaypoint))
                            {
                                CurrentWaypoint = 1;
                            }
                            var MovementPoint = WorldServiceLocator._WS_DBCDatabase.CreatureMovement[aiCreature.WaypointID][CurrentWaypoint];
                            aiTimer = aiCreature.MoveTo(MovementPoint.x, MovementPoint.y, MovementPoint.z) + MovementPoint.waittime;
                        }
                        catch (Exception ex2)
                        {
                            ProjectData.SetProjectError(ex2);
                            WorldServiceLocator._WorldServer.Log.WriteLine(LogType.CRITICAL, "Creature [{0:X}] waypoints are damaged.", aiCreature.GUID - WorldServiceLocator._Global_Constants.GUID_UNIT);
                            aiCreature.ResetAI();
                            ProjectData.ClearProjectError();
                        }
                    }
                    else
                    {
                        base.DoMove();
                    }
                }
            }
        }
    }
}
