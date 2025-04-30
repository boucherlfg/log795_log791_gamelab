using GameLab.Enums;



namespace GameLab.ColorMode

{
    public enum ColorSource
    {
        PlayerPainter,
        ShadowPainter,
        ColorSplash
    }


    public interface IPaintable

    {
        public PlayerNumber Owner { get; }

        public int Score { get; }

        public bool CanReceiveCanonSplash {get;}
        public bool CanReceiveBumperSplash {get;}
        public bool CanReceiveGhostSplash {get;}
        public bool CanReceiveFallSplash {get;}
        public bool CanCollisionWithPlayer {get;}
        public bool CanCollisionWithGhost {get;}
        public bool CanBeRecolored {get;}



        void Paint(PlayerNumber playerNumber, bool canSplash, ColorSource source);

        void Reset();

    }

}