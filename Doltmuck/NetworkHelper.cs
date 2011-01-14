using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

public class NetworkHelper
{
    private NetworkSession session = null;  // The game session
    IAsyncResult AsyncSessionFind = null;
    public PacketWriter packetWriter = new PacketWriter();
    public PacketReader packetReader = new PacketReader();
    public Vector3 Player2;

    private int maximumGamers = 3;          // Only 2 players allowed
    private int maximumLocalPlayers = 1;    // No Split-Screen, only remote players;
    public bool gameStart = false;
    private String message = "Waiting for user command...";
    public float rot;
    public bool p1hit;
    public bool p2hit;

    public void SignInGamer()
    {
        if ((!Guide.IsVisible))
        {
            Guide.ShowSignIn(1, false);
        }
    }

    public void CreateSession()
    {
        if (session == null)
        {
            session = NetworkSession.Create(NetworkSessionType.SystemLink, maximumLocalPlayers, maximumGamers);
            session.AllowHostMigration = true;  // Switch hosts if the original goes out
            session.AllowJoinInProgress = true; // Allow players to join a game in progress
            session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
            session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
            session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);
        }
    }

    public void FindSession()
    {
        // All sessions found
        AvailableNetworkSessionCollection availableSessions;
        // The session we'll join
        AvailableNetworkSession availableSession = null;

        availableSessions = NetworkSession.Find(NetworkSessionType.SystemLink, maximumLocalPlayers, null);

        // Get a session with available gamer slots
        foreach (AvailableNetworkSession curSession in availableSessions)
        {
            Console.WriteLine("FindSession() foreach");
            int TotalSessionSlots = curSession.OpenPublicGamerSlots + curSession.OpenPrivateGamerSlots;

            if (TotalSessionSlots > curSession.CurrentGamerCount)
            {
                Console.WriteLine("FindSession() totalsessionslots");
                availableSession = curSession;
            }
        }

        // If session was found, connect to it
        if (availableSession != null)
        {
            Console.WriteLine("findsession() availablesession");
            message = "Found an available session at host " + availableSession.HostGamertag;
            session = NetworkSession.Join(availableSession);
            gameStart = true;
        }
        else
            message = "No sessions found.";
    }

    public void AsyncFindSession()
    {
        message = "Asynchronous search started.";
        Console.WriteLine("AsyncFindSession()");
        if (AsyncSessionFind == null)
        {
            Console.WriteLine("AsyncSessionFind == null");
            AsyncSessionFind = NetworkSession.BeginFind(NetworkSessionType.SystemLink,
                maximumLocalPlayers, null, new AsyncCallback(session_SessionFound), null);
        }
    }

    public void session_SessionFound(IAsyncResult result)
    {
        // All sessions found
        AvailableNetworkSessionCollection availableSessions;
        // The session we will join
        AvailableNetworkSession availableSession = null;

        if (AsyncSessionFind.IsCompleted)
        {
            Console.WriteLine("AsyncSessionFind.IsCompleted");
            availableSessions = NetworkSession.EndFind(result);

            // Look for a session with available gamer slots
            foreach (AvailableNetworkSession curSession in availableSessions)
            {
                Console.WriteLine("foreach");
                int TotalSessionSlots = curSession.OpenPublicGamerSlots + curSession.OpenPrivateGamerSlots;

                if (TotalSessionSlots > curSession.CurrentGamerCount)
                {
                    Console.WriteLine("TotalSessionSlots");
                    availableSession = curSession;
                }
            }

            // If a session was found, connect to it
            if (availableSession != null)
            {
                message = "Found an available session at host " + availableSession.HostGamertag;
                session = NetworkSession.Join(availableSession);
            }
            else
                message = "No sessions found.";
            // Reset the session finding result
            AsyncSessionFind = null;
        }
    }

    public void SendMessage(Vector3 player)
    {
        foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
        {
            packetWriter.Write(player);
            packetWriter.Write(Game1.camera.angle.Y);
            packetWriter.Write(p1hit);
            //GamePadState GamePad1 = GamePad.getState(PlayerIndex.One);
            // packetWriter.Write(GamePad1.Triggers.Left);
            // packetWriter.Write(GamePad1.Triggers.Right);
            // packetWriter.Write(GamePad1.ThumbSticks.Left);
            localPlayer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
            //message = "Sending message: " + key;
            p1hit = false;
        }
    }

    public NetworkSessionState SessionState
    {
        get
        {
            if (session == null)
                return NetworkSessionState.Ended;
            else
                return session.SessionState;
        }
    }

    public void ReceiveMessage()
    {
        NetworkGamer remotePlayer;  // The sender of the message

        foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
        {
            // While there is data available for us, keep reading
            while (localPlayer.IsDataAvailable)
            {
                localPlayer.ReceiveData(packetReader, out remotePlayer);
                // Ignore input from local players
                if (!remotePlayer.IsLocal)
                {
                    Player2 = packetReader.ReadVector3();
                    rot = packetReader.ReadSingle();
                    p2hit = packetReader.ReadBoolean();
                    //message = "Recieved message: " + packetReader.ReadString();
                    //remoteLeftTrigger = packetReader.ReadSingle();
                    // remoteRighttrigger = packetReader.ReadSingle();
                    // remoteThumbstick = packetReader.ReadVector2();
                }
            }
        }
    }
    public void SetPlayerready()
    {
        foreach (LocalNetworkGamer gamer in session.LocalGamers)
            gamer.IsReady = true;
    }

    public void Update()
    {
        if (session != null)
        {
            session.Update();
        }
    }
    void session_GamerJoined(object sender, GamerJoinedEventArgs e)
    {
        if (e.Gamer.IsHost)
        {
            message = "The Host started the session!";
        }
        else
        {
            message = "Gamer " + e.Gamer.Tag + " joined the session!";
            // Other player joined, start the game!
            session.StartGame();
            gameStart = true;
        }
    }
    void session_GamerLeft(object sender, GamerLeftEventArgs e)
    {
        message = "Gamer " + e.Gamer.Tag + " left the session!";
    }

    void session_GameStarted(object sender, GameStartedEventArgs e)
    {
        message = "Game Started";
    }

    void session_HostChanged(object sender, HostChangedEventArgs e)
    {
        message = "Host changed from " + e.OldHost.Tag + " to " + e.NewHost.Tag;
    }

    void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
    {
        message = "The session has ended";
    }

    void session_GameEnded(object sender, GameEndedEventArgs e)
    {
        message = "Game Over";
    }

    public String Message
    {
        get { return message; }
    }
}

