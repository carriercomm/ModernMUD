using System;
using System.Collections.Generic;
using ModernMUD;

namespace MUDEngine
{
    class HashLink
    {
        public int Key;
        public HashLink Next;
        public Object Data;
    }

    class HashHeader
    {
        public int RecSize;
        public int TableSize;
        public int Klistsize;
        public int Klistlen;
        public LinkedList<int> Keylist;
        public LinkedList<HashLink> Buckets;
    }

    class HuntingData
    {
        public string Name;
        public CharData Victim;
    }

    class RoomQ
    {
        public int RoomNR;
        public RoomQ NextQ;
    }

    class Nodes
    {
        public int Visited;
        public int Ancestor;
    }

    /// <summary>
    /// Mob tracking code.
    /// 
    /// TODO: FIXME: BUG: Rewrite this. Everything commented out is broken. There are probably some
    /// better, clearer AI pathing algorithms that can be used for tracking.
    /// </summary>
    public class Track
    {
        // Hunting parameters.
        // Make sure area_last has the last room index number!
        const bool GO_OK_SMARTER = true;
        readonly int WORLD_SIZE = Room.Count;
        int HashKey( HashHeader ht, int key )
        {
            return ( ( ( (int)( key ) ) * 17 ) % ( ht ).TableSize );
        }

        static void InitHashTable( HashHeader ht, int recSize, int tableSize )
        {
            ht.RecSize = recSize;
            ht.TableSize = tableSize;
            ht.Buckets = new LinkedList<HashLink>();
            ht.Keylist = new LinkedList<int>();
            ht.Klistlen = 0;
        }

        void InitWorld( Room[] room_db )
        {
            /* zero out the world */
            room_db = null;
            return;
        }

        static void DestroyHashTable( HashHeader ht, Object gman )
        {
            ht.Buckets.Clear();
            ht.Keylist.Clear();
            return;
        }

        static void _hash_enter( HashHeader ht, int key, Object data )
        {
            /* precondition: there is no entry for <Key> yet */
            //HashLink temp;
            //int i;

            /*
            temp = new HashLink();
            temp.Key = key;
            temp.Next = ht.Buckets[HashKey(ht, key)];
            temp.Data = data;
            ht.Buckets[HashKey(ht, key)] = temp;
            if( ht.Klistlen >= ht.Klistsize )
            {
                //ht.Keylist = (int)realloc( ht.Keylist, sizeof( ht.Keylist ) * ( ht.Klistsize *= 2 ) );
            }
            for( i = ht.Klistlen; i >= 0; i-- )
            {
                if (ht.Keylist[i - 1] < key)
                {
                    ht.Keylist[i] = key;
                    break;
                }
                ht.Keylist[ i ] = ht.Keylist[ i - 1 ];
            }
            ht.Klistlen++;
            */
            return;
        }

        Room room_find( Room[] room_db, int key )
        {
            return ( ( key < WORLD_SIZE && key > -1 ) ? room_db[ key ] : null );
        }

        static Object hash_find( HashHeader ht, int key )
        {
            HashLink scan = null;
            //scan = ht.Buckets[HashKey(ht, key)];

            while (scan != null && scan.Key != key)
                scan = scan.Next;

            return scan != null ? scan.Data : null;
            return null;
        }

        int RoomEnter( Room[] rb, int key, Room rm )
        {
            Room temp = null;

            temp = room_find( rb, key );
            if( temp )
                return ( 0 );

            rb[ key ] = rm;
            return ( 1 );
        }

        int HashEnter( HashHeader ht, int key, Object data )
        {
            Object temp = hash_find( ht, key );
            if( temp )
                return 0;

            _hash_enter( ht, key, data );
            return 1;
        }

        Room RoomFindOrCreate( Room[] rb, int key )
        {
            Room rv = null;

            rv = room_find( rb, key );
            if( rv )
                return rv;

            rv = new Room();
            rb[ key ] = rv;

            return rv;
        }

        Object HashFindOrCreate( HashHeader ht, int key )
        {
            Object rval;

            rval = hash_find(ht, key);
            if( rval )
                return rval;

            //rval = new char[ ht.RecSize ];
            _hash_enter(ht, key, rval);

            return rval;
        }

        int RoomRemove( Room[] rb, int key )
        {
            Room tmp = null;

            tmp = room_find( rb, key );
            if( tmp )
            {
                rb[ key ] = null;
                tmp = null;
            }
            return ( 0 );
        }

        Object HashRemove( HashHeader ht, int key )
        {
            HashLink scan = null;

            // scan = ht.Buckets + HashKey(ht, key);

            while (scan != null && (scan).Key != key)
                scan = ( scan ).Next;

            if( scan != null )
            {
                //HashLink temp = null;
                HashLink aux;
                int i = 0;

                //temp = (HashLink)( scan ).Data;
                aux = scan;
                scan = aux.Next;
                aux = null;

                //for( i = 0; i < ht.Klistlen; i++ )
                //    if (ht.Keylist[i] == key)
                //        break;

                if( i < ht.Klistlen )
                {
                    //memmove( ht.Keylist + i, ht.Keylist + i + 1, ( ht.Klistlen - i )
                    //         * sizeof( ht.Keylist ) );
                    ht.Klistlen--;
                }

                //return temp;
            }

            return null;
        }

        /*
        void room_iterate( RoomIndex *rb[], void ( *func ) ( ), void *cdata )
        {
        register int i;
 
        for ( i = 0; i < WORLD_SIZE; i++ )
        {
        RoomIndex *temp;
 
        temp = room_find( rb, i );
        if ( temp )
        ( *func ) ( i, temp, cdata );
        }
        }
        */

        /*
        void hash_iterate( HASH_HEADER *ht, void ( *func ) (  ), void *cdata )
        {
        int i;
 
        for ( i = 0; i < ht.Klistlen; i++ )
        {
        void          *temp;
        register int   Key;
 
        Key = ht.Keylist[i];
        temp = hash_find( ht, Key );
        ( *func )( Key, temp, cdata );
        if ( ht.Keylist[i] != Key )    // They must have deleted this room
        i--;        // Hit this slot again.
        }
        }
        */

        /// <summary>
        /// Checks whether an exit is valid.
        /// </summary>
        /// <param name="exit"></param>
        /// <returns></returns>
        static int ExitOk( Exit exit )
        {
            if( ( !exit ) || !( exit.TargetRoom ) )
            {
                return 0;
            }

            return 1;
        }

        public static Exit.Direction FindPath( int inRoomIndexNumber, int outRoomIndexNumber, CharData ch, int depth, bool inZone )
        {
            RoomTemplate herep;
            RoomTemplate startp;
            Exit exitp;
            RoomQ tmp_q;
            RoomQ q_head;
            RoomQ q_tail;
            HashHeader x_room = null;
            bool throughDoors;
            int i;
            int tmp_room;
            int count = 0;

            // TODO: Re-enable this.
            return Exit.Direction.invalid;

            if (depth < 0)
            {
                throughDoors = true;
                depth = -depth;
            }
            else
            {
                throughDoors = false;
            }

            startp = Room.GetRoom(inRoomIndexNumber);

            InitHashTable(x_room, sizeof(int), 2048);
            //HashEnter(x_room, inRoomIndexNumber, null);

            /* initialize queue */
            q_head = new RoomQ();
            q_tail = q_head;
            q_tail.RoomNR = inRoomIndexNumber;
            q_tail.NextQ = null;

            while (q_head != null)
            {
                herep = Room.GetRoom(q_head.RoomNR);
                /* for each room test all directions */
                if (herep.Area == startp.Area || inZone == false)
                {
                    /*
                    * only look in this zone...
                    * saves cpu time and makes world safer for players
                    */
                    for (i = 0; i < Limits.MAX_DIRECTION; i++)
                    {
                        exitp = herep.ExitData[i];
                        if (ExitOk(exitp) != 0 && (throughDoors ? GO_OK_SMARTER :
                            Macros.IsSet((int)Room.GetRoom(q_head.RoomNR).ExitData[i].ExitFlags, (int)Exit.ExitFlag.closed)))
                        {
                            /* next room */
                            tmp_room = herep.ExitData[i].TargetRoom.IndexNumber;
                            if (tmp_room != outRoomIndexNumber)
                            {
                                /*
                                * shall we add room to queue ?
                                * count determines total breadth and depth
                                */
                                if (!hash_find(x_room, tmp_room)
                                        && (count < depth))
                                /* && !IS_SET( RM_FLAGS(tmp_room), DEATH ) ) */
                                {
                                    ++count;
                                    /* mark room as visted and put on queue */

                                    tmp_q = new RoomQ();
                                    tmp_q.RoomNR = tmp_room;
                                    tmp_q.NextQ = null;
                                    q_tail.NextQ = tmp_q;
                                    q_tail = tmp_q;

                                    /* Ancestor for first layer is the direction */
                                    /*HashEnter(x_room, tmp_room,
                                                ((long)hash_find(x_room, q_head.RoomNR) == -1)
                                                ? (Object)(i + 1)
                                                : hash_find(x_room, q_head.RoomNR));*/
                                }
                            }
                            else
                            {
                                /* have reached our goal so free queue */
                                tmp_room = q_head.RoomNR;
                                for (; q_head != null; q_head = tmp_q)
                                {
                                    tmp_q = q_head.NextQ;
                                    q_head = null;
                                }
                                /* return direction if first layer */
                                /*if ((long)hash_find(x_room, tmp_room) == -1)
                                {
                                    if (x_room.Buckets)
                                    {
                                        // junk left over from a previous track
                                        DestroyHashTable(x_room, null);
                                    }
                                    return (i);*/
                                //}
                                //else
                                {
                                    /* else return the Ancestor */
                                    //long j;

                                    /*j = (long)hash_find(x_room, tmp_room);
                                    if (x_room.Buckets)
                                    {
                                        // junk left over from a previous track
                                        DestroyHashTable(x_room, null);
                                    }
                                    return (-1 + j);*/
                                }
                            }
                        }
                    }
                }

                /* free queue head and point to next entry */
                tmp_q = q_head.NextQ;
                q_head = null;
                q_head = tmp_q;
            }

            /* couldn't find path */
            /*if (x_room.Buckets)
            {
                // junk left over from a previous track
                DestroyHashTable(x_room, null);
            }*/
            return Exit.Direction.invalid;
        }

        /// <summary>
        /// Tracking mob has found the person its after. Attack or react accordingly.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="victim"></param>
        public static void FoundPrey( CharData ch, CharData victim )
        {
            string victname = String.Empty;
            string text = String.Empty;
            string lbuf = String.Empty;

            if (!victim)
            {
                Log.Error("FoundPrey: null victim", 0);
                return;
            }

            if (!victim.InRoom)
            {
                Log.Error("FoundPrey: null victim._inRoom", 0);
                return;
            }
            ImmortalChat.SendImmortalChat(null, ImmortalChat.IMMTALK_HUNTING, 0, string.Format("{0}&n has found {1}.", ch.ShortDescription, victim.Name));

            if (ch.IsAffected(Affect.AFFECT_TRACK))
            {
                ch.RemoveAffect(Affect.AFFECT_TRACK);
                Combat.StopHunting(ch);
                return;
            }
            if (ch.IsAffected(Affect.AFFECT_JUSTICE_TRACKER))
            {
                /* Give Justice the ability to ground flying culprits */
                if (victim.FlightLevel != 0)
                {
                    SocketConnection.Act("$n&n forces you to land!", ch, null, victim, SocketConnection.MessageTarget.victim);
                    SocketConnection.Act("$n&n forces $N&n to land!", ch, null, victim, SocketConnection.MessageTarget.room_vict);
                    victim.FlightLevel = 0;
                }

                SocketConnection.Act("$n&n says, 'Stop, $N&n, you're under ARREST!'", ch, null, victim, SocketConnection.MessageTarget.character);
                SocketConnection.Act("$n&n says, 'Stop, $N&n, you're under ARREST!'", ch, null, victim, SocketConnection.MessageTarget.room);
                SocketConnection.Act("$n&n chains you up.", ch, null, victim, SocketConnection.MessageTarget.character);
                SocketConnection.Act("$n&n binds $N&n so $E can't move.", ch, null, victim, SocketConnection.MessageTarget.room);
                victim.SetAffectBit(Affect.AFFECT_BOUND);
                victim.RemoveFromRoom();

                if (ch.InRoom.Area.JailRoom != 0)
                {
                    victim.AddToRoom(Room.GetRoom(victim.InRoom.Area.JailRoom));
                }
                else
                {
                    victim.SendText("Justice is broken in this town - there is no jail and you're screwed.\r\n");
                }
                Combat.StopHunting(ch);
                return;
            }

            victname = victim.IsNPC() ? victim.ShortDescription : victim.Name;

            if (ch.FlightLevel != victim.FlightLevel)
            {
                if (ch.CanFly())
                {
                    if (ch.FlightLevel < victim.FlightLevel && ch.FlightLevel < CharData.FlyLevel.high)
                    {
                        Command.Fly(ch, new string[] { "up" });
                    }
                    else
                    {
                        Command.Fly(ch, new string[] { "down" });
                    }
                }
                else
                {
                    SocketConnection.Act("$n peers around looking for something.", ch, null, null, SocketConnection.MessageTarget.room);
                }
                return;
            }
            if (!CharData.CanSee(ch, victim))
            {
                if (MUDMath.NumberPercent() < 90)
                    return;
                switch (MUDMath.NumberBits(5))
                {
                    case 0:
                        text = String.Format("You can't hide forever, {0}!", victname);
                        Command.Say(ch, new string[]{text});
                        break;
                    case 1:
                        SocketConnection.Act("$n&n sniffs around the room.", ch, null, victim, SocketConnection.MessageTarget.room);
                        text = "I can smell your blood!";
                        Command.Say(ch, new string[]{text});
                        break;
                    case 2:
                        text = String.Format("I'm going to tear {0} apart!", victname);
                        Command.Yell(ch, new string[]{text});
                        break;
                    case 3:
                        Command.Say(ch, new string[]{"Just wait until I find you..."});
                        break;
                    default:
                        SocketConnection.Act("$p peers about looking for something.", ch, null, null, SocketConnection.MessageTarget.room);
                        break;
                }
                return;
            }

            if (ch.InRoom.HasFlag(RoomTemplate.ROOM_SAFE) && ch.IsNPC())
            {
                text = String.Format("Hunting mob {0} found a safe room {1}.", ch.MobileTemplate.IndexNumber, ch.InRoom.IndexNumber);
                Log.Trace(text);
                return;
            }

            if (ch.CurrentPosition > Position.kneeling)
            {

                switch (MUDMath.NumberBits(5))
                {
                    case 0:
                        text = String.Format("I will eat your heart, {0}!", victname);
                        Command.Say(ch, new string[]{text});
                        break;
                    case 1:
                        text = String.Format("You want a piece of me, {0}?", victname);
                        Command.Say(ch, new string[]{text});
                        break;
                    case 2:
                        text = String.Format("How does your flesh taste {0}, like chicken?", victname);
                        Command.Say(ch, new string[]{text});
                        break;
                    case 3:
                        SocketConnection.Act("$n&n howls gleefully and lunges at $N&n!", ch, null, victim, SocketConnection.MessageTarget.everyone_but_victim);
                        SocketConnection.Act("$n&n howls gleefully and lunges at you!", ch, null, victim, SocketConnection.MessageTarget.victim);
                        break;
                    case 4:
                        SocketConnection.Act("$n&n charges headlong into $N&n!", ch, null, victim, SocketConnection.MessageTarget.everyone_but_victim);
                        SocketConnection.Act("$n&n charges headlong into you!", ch, null, victim, SocketConnection.MessageTarget.victim);
                        break;
                    default:
                        break;
                }
                Combat.StopHunting(ch);
                Combat.CheckAggressive(victim, ch);
                if (ch.Fighting)
                    return;

                // Backstab if able, otherwise just kill them.
                // Kill if they don't have the skill or if they don't have a stabber.
                if (!ch.HasSkill("backstab"))
                {
                    Combat.CombatRound(ch, victim, String.Empty);
                }
                else if (!Combat.Backstab(ch, victim))
                {
                    Combat.CombatRound(ch, victim, String.Empty);
                }
            }
            return;
        }

        /// <summary>
        /// Tracking code.
        /// </summary>
        /// <param name="ch"></param>
        public static void HuntVictim( CharData ch )
        {
            if (!ch || !ch.Hunting || !ch.IsAffected(Affect.AFFECT_TRACK))
            {
                return;
            }

            if( ch.CurrentPosition != Position.standing )
            {
                if( ch.IsAffected( Affect.AFFECT_TRACK ) )
                {
                    ch.SendText( "You abort your tracking effort.\r\n" );
                    ch.RemoveAffect(Affect.AFFECT_TRACK);
                    Combat.StopHunting( ch );
                }
                return;
            }

            CharData tmp = null;

            try
            {
                /*
                * Make sure the victim still exists.
                */
                bool found = false;
                foreach (CharData it in Database.CharList)
                {
                    ch = it;
                    if (ch.Hunting != null && ch.Hunting.Who == tmp)
                        found = true;
                }

                if (!found || !CharData.CanSee(ch, ch.Hunting.Who))
                {
                    if (!ch.IsAffected(Affect.AFFECT_TRACK))
                        CommandType.Interpret(ch, "say Damn!  My prey is gone!");
                    else
                    {
                        ch.SendText("The trail seems to disappear.\r\n");
                        ch.RemoveAffect(Affect.AFFECT_TRACK);
                    }
                    Combat.StopHunting(ch);
                    return;
                }

                if (ch.InRoom == ch.Hunting.Who.InRoom)
                {
                    if (ch.Fighting)
                    {
                        return;
                    }
                    FoundPrey(ch, ch.Hunting.Who);
                    return;
                }

                ch.WaitState(Skill.SkillList["track"].Delay);
                Exit.Direction dir = FindPath(ch.InRoom.IndexNumber, ch.Hunting.Who.InRoom.IndexNumber, ch, -40000, true);

                if (dir == Exit.Direction.invalid)
                {
                    if (!ch.IsAffected(Affect.AFFECT_TRACK))
                    {
                        SocketConnection.Act("$n&n says 'Damn! Lost $M!'", ch, null, ch.Hunting.Who, SocketConnection.MessageTarget.room);
                    }
                    else
                    {
                        ch.SendText("You lose the trail.\r\n");
                        ch.RemoveAffect(Affect.AFFECT_TRACK);
                        Combat.StopHunting(ch);
                    }
                    return;
                }

                /*
                * Give a random direction if the mob misses the die roll.
                */
                if (MUDMath.NumberPercent() > 75)   /* @ 25% */
                {
                    do
                    {
                        dir = Database.RandomDoor();
                    }
                    while (!(ch.InRoom.ExitData[(int)dir]) || !(ch.InRoom.ExitData[(int)dir].TargetRoom));
                }

                if (ch.InRoom.ExitData[(int)dir].HasFlag(Exit.ExitFlag.closed))
                {
                    CommandType.Interpret(ch, "open " + dir.ToString());
                    return;
                }
                ImmortalChat.SendImmortalChat(null, ImmortalChat.IMMTALK_HUNTING, 0, String.Format("{0}&n leaves room {1} to the {2}.",
                    ch.ShortDescription, ch.InRoom.IndexNumber, dir.ToString()));
                if (ch.IsAffected(Affect.AFFECT_TRACK))
                {
                    SocketConnection.Act(String.Format("You sense $N&n's trail {0} from here...", dir.ToString()),
                        ch, null, ch.Hunting.Who, SocketConnection.MessageTarget.character);
                }
                ch.Move(dir);
                if (ch.IsAffected(Affect.AFFECT_TRACK))
                    SocketConnection.Act("$n&n peers around looking for tracks.", ch, null, null, SocketConnection.MessageTarget.room);

                if (!ch.Hunting)
                {
                    if (!ch.InRoom)
                    {
                        string text = String.Empty;
                        text = String.Format("Hunt_victim: no ch.in_room!  Mob #{0}, _name: {1}.  Placing mob in limbo (ch.AddToRoom()).",
                                  ch.MobileTemplate.IndexNumber, ch.Name);
                        Log.Error(text, 0);
                        ch.AddToRoom(Room.GetRoom(StaticRooms.GetRoomNumber("ROOM_NUMBER_LIMBO")));
                        text = String.Format("{0}&n has gone to limbo while hunting {1}.", ch.ShortDescription, ch.Hunting.Name);
                        ImmortalChat.SendImmortalChat(null, ImmortalChat.IMMTALK_HUNTING, 0, text);
                        return;
                    }
                    CommandType.Interpret(ch, "say Damn!  Lost my prey!");
                    return;
                }
                if (ch.InRoom == ch.Hunting.Who.InRoom)
                {
                    FoundPrey(ch, ch.Hunting.Who);
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in HuntVictim: " + ex.ToString());
            }
        }

        public static void ReturnToLoad( CharData ch )
        {
            if( !ch || !ch.InRoom )
                return;
            if( ch.InRoom.Area != Room.GetRoom( ch.LoadRoomIndexNumber ).Area )
                return;

            Exit.Direction dir = FindPath( ch.InRoom.IndexNumber, ch.LoadRoomIndexNumber, ch, -40000, true );

            if( dir == Exit.Direction.invalid )
            {
                return;
            }

            if( ch.InRoom.ExitData[ (int)dir ].HasFlag( Exit.ExitFlag.closed ) &&
                    !ch.IsAffected( Affect.AFFECT_PASS_DOOR ) && !ch.HasInnate( Race.RACE_PASSDOOR ) )
            {
                CommandType.Interpret(ch, "unlock " + dir.ToString());
                CommandType.Interpret(ch, "open " + dir.ToString());
                return;
            }

            ch.Move( dir );

            if( !ch.InRoom )
            {
                string text = "Return_to_load: no ch._inRoom!  Mob #" + ch.MobileTemplate.IndexNumber + ", _name: " +
                       ch.Name + ".  Placing mob in limbo (mob.AddToRoom()).";
                Log.Error( text, 0 );
                ch.AddToRoom( Room.GetRoom( StaticRooms.GetRoomNumber("ROOM_NUMBER_LIMBO") ) );
                ImmortalChat.SendImmortalChat(ch, ImmortalChat.IMMTALK_SPAM, 0, text);
                return;
            }
            return;
        }

    }
}
