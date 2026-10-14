using UnityEngine;

namespace StickmanParty.Core
{
    public enum PlayerSlot
    {
        Player1 = 0,
        Player2 = 1,
        Player3 = 2,
        Player4 = 3
    }

    [System.Serializable]
    public class PlayerConfig
    {
        public PlayerSlot slot;
        public string playerName;
        public Color playerColor;
        public bool isBot;
        public int currentStars;
        public int roundScore;

        // Custom Control Mappings (Local Single Keyboard / Controller)
        public KeyCode keyUp;
        public KeyCode keyDown;
        public KeyCode keyLeft;
        public KeyCode keyRight;
        public KeyCode keyAction; // Punch, Shoot, Kick, etc.

        public PlayerConfig(PlayerSlot slot, string name, Color color, bool isBot,
            KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode action)
        {
            this.slot = slot;
            this.playerName = name;
            this.playerColor = color;
            this.isBot = isBot;
            this.currentStars = 0;
            this.roundScore = 0;
            this.keyUp = up;
            this.keyDown = down;
            this.keyLeft = left;
            this.keyRight = right;
            this.keyAction = action;
        }

        public static PlayerConfig GetDefault(PlayerSlot slot)
        {
            switch (slot)
            {
                case PlayerSlot.Player1:
                    // Red Player: WASD + Space/J
                    return new PlayerConfig(slot, "Red", new Color(1f, 0.22f, 0.22f), false,
                        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space);

                case PlayerSlot.Player2:
                    // Blue Player: Arrow Keys + Enter/Numpad0
                    return new PlayerConfig(slot, "Blue", new Color(0.18f, 0.52f, 1f), true,
                        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Return);

                case PlayerSlot.Player3:
                    // Green Player: IJKL + U
                    return new PlayerConfig(slot, "Green", new Color(0.2f, 0.85f, 0.35f), true,
                        KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.U);

                case PlayerSlot.Player4:
                    // Yellow Player: Numpad 8-4-5-6 + NumpadPlus
                    return new PlayerConfig(slot, "Yellow", new Color(1f, 0.82f, 0.1f), true,
                        KeyCode.Keypad8, KeyCode.Keypad5, KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.KeypadPlus);

                default:
                    return new PlayerConfig(slot, "Player", Color.white, true,
                        KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None);
            }
        }
    }
}
