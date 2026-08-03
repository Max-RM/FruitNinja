using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework.GamerServices;

public enum MessageBoxIcon
{
    Alert = 0,
    Error = 1,
    Warning = 2,
    None = 3
}

public enum LeaderboardKey
{
    Default = 0
}

public class GameUpdateRequiredException : Exception
{
    public GameUpdateRequiredException() { }
    public GameUpdateRequiredException(string message) : base(message) { }
}

public class Gamer
{
    private static readonly SignedInGamerCollection SignedInGamersInternal = new();
    private static readonly LocalSignedInGamer LocalGamer = new();

    static Gamer()
    {
        SignedInGamersInternal.Add(LocalGamer);
    }

    public static SignedInGamerCollection SignedInGamers => SignedInGamersInternal;
    public string Gamertag { get; protected set; } = "Player";
}

public class SignedInGamerCollection : IEnumerable<SignedInGamer>
{
    private readonly List<SignedInGamer> gamers = new();
    public SignedInGamer this[PlayerIndex index]
    {
        get
        {
            if (gamers.Count == 0) return null;
            int i = (int)index;
            return i >= 0 && i < gamers.Count ? gamers[i] : gamers[0];
        }
    }
    public int Count => gamers.Count;
    internal void Add(SignedInGamer gamer) => gamers.Add(gamer);
    public IEnumerator<SignedInGamer> GetEnumerator() => gamers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class SignedInEventArgs : EventArgs
{
    public SignedInGamer Gamer { get; }
    public SignedInEventArgs(SignedInGamer gamer) { Gamer = gamer; }
}

public class SignedInGamer : Gamer
{
    public static event SignedInEventHandler SignedIn;
    public SignedInGamer() { Gamertag = "Player"; }
    internal static void RaiseSignedIn() => SignedIn?.Invoke(null, new SignedInEventArgs((SignedInGamer)Gamer.SignedInGamers[PlayerIndex.One]));
    public IEnumerable<Gamer> GetFriends() { yield return this; }
}

public delegate void SignedInEventHandler(object sender, SignedInEventArgs e);

internal sealed class LocalSignedInGamer : SignedInGamer { }

public static class Guide
{
    private static readonly Dictionary<IAsyncResult, int> MessageBoxResults = new();
    private static int messageBoxCounter;

    public static bool IsVisible { get; private set; }
    public static bool IsTrialMode { get; set; }
    public static bool IsScreenSaverEnabled { get; set; }

    public static void ShowMarketplace(PlayerIndex player) { }

    public static IAsyncResult BeginShowMessageBox(
        string title, string text, IEnumerable<string> buttons,
        int focusButton, MessageBoxIcon icon,
        AsyncCallback callback, object state)
    {
        IsVisible = true;
        int result = 0;
        try
        {
            var buttonList = new List<string>(buttons ?? Array.Empty<string>());
            if (buttonList.Count <= 1)
            {
                MessageBox.Show(text, title);
                result = 0;
            }
            else
            {
                var dialogResult = MessageBox.Show(text, title, MessageBoxButtons.YesNo);
                result = dialogResult == DialogResult.Yes ? 1 : 0;
            }
        }
        finally
        {
            IsVisible = false;
        }
        var asyncResult = new MessageBoxAsyncResult(++messageBoxCounter, state, result);
        MessageBoxResults[asyncResult] = result;
        callback?.Invoke(asyncResult);
        return asyncResult;
    }

    public static int? EndShowMessageBox(IAsyncResult result)
    {
        if (result is MessageBoxAsyncResult mbResult)
            return mbResult.SelectedButton;
        if (MessageBoxResults.TryGetValue(result, out int value))
        {
            MessageBoxResults.Remove(result);
            return value;
        }
        return 0;
    }

    private sealed class MessageBoxAsyncResult : IAsyncResult
    {
        public int Id { get; }
        public int SelectedButton { get; }
        public object AsyncState { get; }
        public bool IsCompleted { get; } = true;
        public bool CompletedSynchronously { get; } = true;
        public WaitHandle AsyncWaitHandle => null;

        public MessageBoxAsyncResult(int id, object asyncState, int selectedButton)
        {
            Id = id;
            AsyncState = asyncState;
            SelectedButton = selectedButton;
        }
    }
}

public class LeaderboardIdentity
{
    public static LeaderboardIdentity Create(LeaderboardKey key, int leaderboardId) =>
        new() { Key = key, Id = leaderboardId };
    public LeaderboardKey Key { get; private set; }
    public int Id { get; private set; }
}

public class LeaderboardEntry
{
    public Gamer Gamer { get; set; } = new();
    public long Rating { get; set; }
}
